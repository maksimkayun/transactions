using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record CloseAccountCommand(long AccountNumber, long RecipientAccountNumber)
    : IRequest<CloseAccountCommandResult>;

public record CloseAccountCommandResult(Account Account, ErrorInfo? ErrorInfo);

public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, CloseAccountCommandResult>
{
    private readonly TransactionsContext _context;
    private readonly IMediator _mediator;

    public CloseAccountCommandHandler(TransactionsContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<CloseAccountCommandResult> Handle(CloseAccountCommand request,
        CancellationToken cancellationToken)
    {
        var accountNumber = request.AccountNumber;
        var account =
            await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);

        if (account == null || account.IsDeleted)
        {
            throw new Exception("Account not found or already closed");
        }

        var sendMoneyCommand = new SendMoneyCommand(request.AccountNumber.ToString(),
            request.RecipientAccountNumber.ToString(), account.Amount);
        var sendMoneyResult = await _mediator.Send(sendMoneyCommand, cancellationToken);

        if (sendMoneyResult.ErrorInfo != null)
        {
            throw new Exception($"Error transferring funds: {sendMoneyResult.ErrorInfo.Message}");
        }

        account.IsDeleted = true;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);

        return new CloseAccountCommandResult(MappingService.AccountFromDb(account), null);
    }
}