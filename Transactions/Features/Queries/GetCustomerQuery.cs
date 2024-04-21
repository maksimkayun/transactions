using Api.Dto;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions;

namespace Domain.Events.Queries;

public record GetCustomerQuery(string Id) : IRequest<CustomerDto>;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerDto>
{
    private readonly TransactionsContext _context;

    public GetCustomerQueryHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var custId = CustomerId.Of(request.Id);
        var cust = await _context.Customers.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(e => e.Id == custId, cancellationToken: cancellationToken);

        if (cust == null)
        {
            return ErrorDtoCreator.Create<CustomerDto>($"Customer with Id={request.Id} not found");
        }

        return new CustomerDto
        {
            ErrorInfo = null,
            Id = cust.Id.Value.ToString(),
            Name = cust.Name,
            Accounts = cust.Accounts?.Select(e => new AccountDto
            {
                ErrorInfo = null,
                AccountNumber = e.Number.ToString()!,
                OwnerId = cust.Id.ToString()!,
                Amount = e.Amount,
                OutgoingTransactionIds = e.OutgoingTransactions?.Select(tr=> tr.Id.ToString()!).ToList(),
                IncomingTransactionIds = e.IncomingTransactions?.Select(tr=> tr.Id.ToString()!).ToList(),
            }).ToList()
        };
    }
}