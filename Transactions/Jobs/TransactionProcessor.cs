using MediatR;
using Quartz;

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
        
    }
}