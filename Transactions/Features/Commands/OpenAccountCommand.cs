using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record OpenAccountCommand(CustomerDto CustomerDto, decimal StartAmount) : IRequest<OpenAccountResult>;

public record OpenAccountResult(Account Account, ErrorInfo? ErrorInfo);

public class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, OpenAccountResult>
{
    private readonly TransactionsContext _context;
    private EventsExecutor _executor;

    public OpenAccountCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<OpenAccountResult> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        var cust = await _context.Customers
            .Include(customer => customer.Accounts)
            .FirstOrDefaultAsync(e => e.Id == request.CustomerDto.Id, cancellationToken);
        if (cust is null)
        {
            throw new InvalidCustomerIdException(request.CustomerDto.Id);
        }

        // Проверяем, есть ли у пользователя закрытый счет
        var closedAccount = cust.Accounts.FirstOrDefault(a => a.IsDeleted);
        Account account;
        if (closedAccount != null)
        {
            // Если есть закрытый счет, переоткрываем его
            account = MappingService.AccountFromDb(closedAccount);
            account.ReopenAccount(request.StartAmount);
            
            closedAccount.IsDeleted = account.IsDeleted;
            closedAccount.Amount = account.Amount;
            closedAccount.OpenDate = account.OpenDate;
            _context.Accounts.Update(closedAccount);
        }
        else
        {
            // Если нет закрытого счета, создаем новый
            var lastNumber = _context.Accounts.Any()
                ? await _context.Accounts.MaxAsync(e => e.AccountNumber, cancellationToken)
                : AccountNumber.START_VALUE - 1;
            account = Account.Create(AccountId.Of(NewId.NextGuid()), AccountNumber.Of(lastNumber + 1),
                request.StartAmount);

            var accDb = MappingService.AccountMapAggregateToDb(account, cust);
            await _context.Accounts.AddAsync(accDb, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _executor.ExecuteEvents(account, cancellationToken);

        return new OpenAccountResult(account, null);
    }
}