using Api.Dto;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Features.Commands;

public record SendMoneyCommand(string SenderAccountNumber, string RecipientAccountNumber, decimal Amount) : IRequest<SendMoneyCommandResult>;

public record SendMoneyCommandResult(TransactionDto Transaction);

public class SendMoneyCommandHandler : IRequestHandler<SendMoneyCommand, SendMoneyCommandResult>
{
    private TransactionsContext _context;

    public SendMoneyCommandHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<SendMoneyCommandResult> Handle(SendMoneyCommand request, CancellationToken cancellationToken)
    {
        var numbSender = AccountNumber.Of(request.SenderAccountNumber);
        var numbRecipient = AccountNumber.Of(request.RecipientAccountNumber);
        
        var senderAcc = await _context.Accounts.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(e => e.Number == numbSender, cancellationToken);
        var recipientAcc = await _context.Accounts.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(e => e.Number == numbRecipient, cancellationToken);

        var tr = Transaction.Create(senderAcc, recipientAcc, request.Amount);
        tr.Impoverish();
        tr = _context.Transactions.Add(tr).Entity;

        await _context.SaveChangesAsync(cancellationToken);

        return new SendMoneyCommandResult(new TransactionDto
        {
            ErrorInfo = null,
            Id = tr.Id.Value.ToString(),
            SenderAccountNumber = tr.SenderAccount.Number.Value.ToString(),
            RecipientAccountNumber = tr.RecipientAccount.Number.Value.ToString(),
            Amount = tr.Amount,
            Status = tr.Status.Description
        });
    }
}