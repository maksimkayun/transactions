using DataAccess;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Transactions.Mapping;

namespace Transactions.Jobs;

public class TransactionProcessor : IJob
{
    private TransactionsContext _context;
    public static readonly JobKey Key = new JobKey("TransactionProcessor");

    public TransactionProcessor(TransactionsContext context)
    {
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var transactions = _context.Transactions.Include(e => e.TransactionStatus)
            .Include(e=>e.SenderAccount)
            .Include(e=>e.RecipientAccount)
            .Where(e => e.TransactionStatus.Id == 1)?.ToList();
        if (transactions == null || !transactions.Any())
        {
            return;
        }
        foreach (var t in transactions)
        {
            var accSender = Mapper.MapToAggregate(t.SenderAccount);
            var accRecip = Mapper.MapToAggregate(t.RecipientAccount);
            var transaction = new Aggregates.Transaction(senderAccountNumber: accSender,
                recipientAccountNumber: accRecip, amount: t.Amount, transactionId: t.Id);
                
            await transaction.MakeTransaction(CancellationToken.None);
            var result = transaction.GetResult().WithCheck();

            t.SenderAccount.Amount = accSender.GetAmount();
            t.RecipientAccount.Amount = accRecip.GetAmount();
            
            var trStatusDb = await _context.TransactionStatuses.FirstAsync(e => e.Id == result.TransactionStatus.Id, CancellationToken.None);
            t.TransactionStatus = trStatusDb;
            _context.Transactions.Update(t);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}

public class TransactionScheduler
{
    public static async Task Start()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();
 
        IJobDetail job = JobBuilder.Create<TransactionProcessor>().Build();
 
        ITrigger trigger = TriggerBuilder.Create()  
            .WithIdentity("trigger1", "group1")     
            .StartNow()                           
            .WithSimpleSchedule(x => x         
                .WithIntervalInSeconds(2)    
                .RepeatForever())                   
            .Build();                              
 
        await scheduler.ScheduleJob(job, trigger);        
    }
}