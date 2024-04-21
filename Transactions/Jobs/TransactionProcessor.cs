using DataAccess;
using Domain.Aggregates;
using Domain.Mapping;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;

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
            var transaction = new Transaction(senderAccountNumber: accSender,
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