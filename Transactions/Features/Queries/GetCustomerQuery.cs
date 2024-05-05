using Api.Dto;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Queries;

public record GetCustomerQuery(string? Id, string? Name, int? Skip = 0, int? Take = 0) : IRequest<List<CustomerDto>>;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, List<CustomerDto>>
{
    private readonly TransactionsContext _context;

    public GetCustomerQueryHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var custId = request.Id;
        var custName = request.Name;
        var searchWithSkipTake = request.Skip != null && request.Skip >= 0 && request.Take != null && request.Take > 0;

        var custs = !string.IsNullOrWhiteSpace(custId) || !string.IsNullOrWhiteSpace(custName)
            ? await _context.Customers.AsNoTrackingWithIdentityResolution()
                .Include(customer => customer.Accounts.Where(a=>!a.IsDeleted))
                .ThenInclude(account => account.OutgoingTransactions)
                .Include(customer => customer.Accounts.Where(a=>!a.IsDeleted))
                .ThenInclude(account => account.IncomingTransactions)
                .Where(e => (e.Id == custId || e.Name == custName) && !e.IsDeleted)
                .ToListAsync(cancellationToken: cancellationToken)
            : searchWithSkipTake
                ? await _context.Customers.AsNoTrackingWithIdentityResolution()
                    .Include(customer => customer.Accounts.Where(a=>!a.IsDeleted))
                    .ThenInclude(account => account.OutgoingTransactions)
                    .Include(customer => customer.Accounts.Where(a=>!a.IsDeleted))
                    .ThenInclude(account => account.IncomingTransactions)
                    .Where(e=>!e.IsDeleted)
                    .Skip((int) request.Skip)
                    .Take((int) request.Take)
                    .ToListAsync(cancellationToken)
                : default;
        
        if (custs == null)
        {
            var specErr = "";
            var fullMessage = "";
            if (request.Id != default)
            {
                specErr = $"Id={request.Id}";
            } 
            else if (request.Name != default)
            {
                specErr = $"Name={request.Name}";
            }
            else if (searchWithSkipTake)
            {
                fullMessage = "Customers not found";
            }
            else
            {
                fullMessage = "Search by the specified parameters is not possible";
            }
            return new List<CustomerDto>() {ErrorDtoCreator.Create<CustomerDto>(string.IsNullOrWhiteSpace(fullMessage) ? $"Customer with {specErr} not found" : fullMessage) };
        }

        return custs.Select(c => new CustomerDto
        {
            ErrorInfo = null,
            Id = c.Id,
            Name = c.Name,
            Accounts = c.Accounts?.Select(acc => new AccountDto
            {
                ErrorInfo = null,
                AccountNumber = acc.AccountNumber.ToString(),
                OwnerId = c.Id,
                Amount = acc.Amount,
                OutgoingTransactionIds = acc.OutgoingTransactions?.Select(t => t.Id)?.ToList() ?? new List<string>(),
                IncomingTransactionIds = acc.IncomingTransactions?.Select(t => t.Id)?.ToList() ?? new List<string>()
            })?.ToList() ?? new List<AccountDto>()
        }).ToList();
    }
}