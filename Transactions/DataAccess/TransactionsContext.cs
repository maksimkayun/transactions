//using Domain.Aggregates;

using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess.Models;

namespace Transactions.DataAccess;

public class TransactionsContext : DbContext
{
    private IMediator _mediator;


    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<TransactionStatus> TransactionStatuses { get; set; }

    protected TransactionsContext()
    {
    }

    public TransactionsContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Account>().HasKey(e => new
        {
            e.Id,
            e.AccountNumber
        });
        modelBuilder.Entity<Customer>().HasKey(e => e.Id);
        modelBuilder.Entity<Transaction>().HasKey(e => e.Id);
        modelBuilder.Entity<TransactionStatus>().HasKey(e => e.Id);

        modelBuilder.Entity<Account>()
            .HasOne<Customer>(e => e.Owner)
            .WithMany(e => e.Accounts)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<Account>(e => e.SenderAccount)
            .WithMany(e => e.OutgoingTransactions)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<Account>(e => e.RecipientAccount)
            .WithMany(e => e.IncomingTransactions)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<TransactionStatus>(e => e.TransactionStatus);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch Domain Events collection.
        await DispatchEvents(cancellationToken);

        return result;
    }

    private async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.GetDomainEvents() != null && x.Entity.GetDomainEvents().Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }
}