using Domain.Aggregates;
using Domain.Aggregates.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Transactions.DataAccess;
using Transactions.Features;
using Transactions.Utils;

namespace Transactions.Jobs;

public class TransactionProcessor : IJob
{
    private TransactionsContext _context;
    public static readonly JobKey Key = new JobKey("TransactionProcessor");
    private readonly IMediator _mediator;

    public TransactionProcessor(TransactionsContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var notWorkingTransactions = _context.Transactions
            .Include(t => t.TransactionStatus)
            .Include(t => t.RecipientAccount)
            .Include(t => t.SenderAccount)
            .AsNoTrackingWithIdentityResolution()
            .Where(tr => tr.TransactionStatus.Id == TransactionStatus.Created.Id).ToList();

        foreach (var tr in notWorkingTransactions)
        {
            var trAggr = MappingService.TransactionFromDb(tr);
            await _mediator.Publish(new TransactionCreatedDomainEvent(trAggr));
        }
    }
}