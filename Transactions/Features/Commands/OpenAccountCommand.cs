using Api.Dto;
using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Features.Commands;

public record OpenAccountCommand(CustomerDto CustomerDto, decimal StartAmount) : IRequest<OpenAccountResult>;

public record OpenAccountResult(Account Account);

public class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, OpenAccountResult>
{
    private readonly TransactionsContext _context;

    public OpenAccountCommandHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<OpenAccountResult> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        var custId = CustomerId.Of(request.CustomerDto.Id);
        var cust = _context.Customers.FirstOrDefault(e => e.Id == custId);

        var lastNumber = _context.Accounts.Any()
            ? await _context.Accounts.AsNoTrackingWithIdentityResolution().Select(e => e.Number)
                .MaxAsync(e => e.Value, cancellationToken)
            : AccountNumber.START_VALUE;
        var openedAcc = cust.OpenAccount(lastNumber + 1, request.StartAmount);

        _context.Accounts.Add(openedAcc);
        await _context.SaveChangesAsync(cancellationToken);

        return new OpenAccountResult(openedAcc);
    }
}