using Api.Dto;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;

namespace Transactions.Features.Queries;

public record GetCustomerQuery(string? Id, string? Name) : IRequest<CustomerDto>;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerDto>
{
    private readonly TransactionsContext _context;

    public GetCustomerQueryHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var custId = request.Id;
        var custName = request.Name;
        var cust = await _context.Customers.AsNoTrackingWithIdentityResolution()
            .Include(customer => customer.Accounts)
            .ThenInclude(account => account.OutgoingTransactions).Include(customer => customer.Accounts)
            .ThenInclude(account => account.IncomingTransactions)
            .FirstOrDefaultAsync(e => e.Id == custId || e.Name == custName, cancellationToken: cancellationToken);
        
        if (cust == null)
        {
            var specErr = request.Id == default ? "Name=" + custName : "Id=" + custId;
            return ErrorDtoCreator.Create<CustomerDto>($"Customer with {specErr} not found");
        }
        
        return new CustomerDto
        {
            ErrorInfo = null,
            Id = cust.Id,
            Name = cust.Name,
            Accounts = cust.Accounts?.Select(e => new AccountDto
            {
                ErrorInfo = null,
                AccountNumber = e.AccountNumber.ToString(),
                OwnerId = cust.Id,
                Amount = e.Amount,
                OutgoingTransactionIds = e.OutgoingTransactions?.Select(tr=> tr.Id.ToString()!).ToList(),
                IncomingTransactionIds = e.IncomingTransactions?.Select(tr=> tr.Id.ToString()!).ToList(),
            }).ToList()
        };
        throw new NotImplementedException();
    }
}