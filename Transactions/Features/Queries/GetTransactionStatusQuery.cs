using Api.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;

namespace Transactions.Features.Queries;

public record GetTransactionStatusQuery(string? Id) : IRequest<TransactionDto>;

public class GetTransactionStatusQueryHandler : IRequestHandler<GetTransactionStatusQuery, TransactionDto>
{
    private readonly TransactionsContext _context;

    public GetTransactionStatusQueryHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<TransactionDto> Handle(GetTransactionStatusQuery request, CancellationToken cancellationToken)
    {
        var tr = await _context.Transactions
            .Include(transaction => transaction.SenderAccount)
            .Include(transaction => transaction.RecipientAccount)
            .Include(transaction => transaction.TransactionStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (tr == default)
        {
            return ErrorDtoCreator.Create<TransactionDto>($"Transaction with ID={request.Id} does not exist");
        }

        return new TransactionDto
        {
            ErrorInfo = null,
            Id = tr.Id,
            SenderAccountNumber = tr.SenderAccount.AccountNumber.ToString(),
            RecipientAccountNumber = tr.RecipientAccount.AccountNumber.ToString(),
            Amount = tr.Amount,
            Status = tr.TransactionStatus.Description
        };
    }
}