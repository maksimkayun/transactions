using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record SendMoneyCommand(string SenderAccountNumber, string RecipientAccountNumber, decimal Amount) : IRequest<SendMoneyCommandResult>;

public record SendMoneyCommandResult(Transaction Transaction, ErrorInfo? ErrorInfo);

public class SendMoneyCommandHandler : IRequestHandler<SendMoneyCommand, SendMoneyCommandResult>
{
    private TransactionsContext _context;
    private EventsExecutor _executor;

    public SendMoneyCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<SendMoneyCommandResult> Handle(SendMoneyCommand request, CancellationToken cancellationToken)
    {
        var senderNum = long.Parse(request.SenderAccountNumber);
        var recNum = long.Parse(request.RecipientAccountNumber);
        var accsDb = await
            _context.Accounts.Include(x=>x.Owner)
                .Where(x => (x.AccountNumber == senderNum || x.AccountNumber == recNum) && !x.IsDeleted && !x.Owner.IsDeleted ).ToListAsync(cancellationToken);

        if (accsDb.Count != 2)
        {
            throw new InvalidTransactionException($"The transaction is not possible");
        }

        var sender = MappingService.AccountFromDb(accsDb.First(a => a.AccountNumber == senderNum));
        var recipient = MappingService.AccountFromDb(accsDb.First(a => a.AccountNumber == recNum));
        var tr = Transaction.Create(sender, recipient, request.Amount);

        var transaction = MappingService.TransactionMapAggregateToDb(tr);
        transaction.TransactionStatus =
            await _context.TransactionStatuses.FirstAsync(x => x.Id == transaction.TransactionStatus.Id, cancellationToken);
        transaction.SenderAccount = accsDb.First(a => a.AccountNumber == senderNum);
        transaction.RecipientAccount = accsDb.First(a => a.AccountNumber == recNum);
        
        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _executor.ExecuteEvents(tr, cancellationToken);

        return new SendMoneyCommandResult(tr, null);
    }
}