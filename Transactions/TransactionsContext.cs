using Domain.Aggregates;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Transactions;

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

        modelBuilder.Entity<Account>().ToTable(nameof(Account));
        modelBuilder.Entity<Account>().HasKey(r => r.Id);
        modelBuilder.Entity<Account>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(accountId => accountId.Value, dbId => AccountId.Of(dbId));
        // modelBuilder.Entity<Account>().OwnsOne(
        //     x => x.Number,
        //     a =>
        //     {
        //         a.Property(p => p.Value)
        //             .HasColumnName(nameof(Account.Number))
        //             .IsRequired();
        //         
        //         a.HasIndex(p => p.Value);
        //     });
        modelBuilder.Entity<Account>().Property(p => p.Number)
            .HasConversion<long>(ac => ac.Value, acDb => AccountNumber.Of(acDb));
        modelBuilder.Entity<Account>().HasIndex(p => p.Number);
        
        modelBuilder.Entity<Transaction>().ToTable(nameof(Transaction));
        modelBuilder.Entity<Transaction>().HasKey(r => r.Id);
        modelBuilder.Entity<Transaction>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(trId => trId.Value, dbId => TransactionId.Of(dbId));
        
        modelBuilder.Entity<Customer>().ToTable(nameof(Customer));
        modelBuilder.Entity<Customer>().HasKey(r => r.Id);
        modelBuilder.Entity<Customer>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(trId => trId.Value, dbId => CustomerId.Of(dbId));
        
        modelBuilder.Entity<Customer>().HasKey(e => e.Id);
        modelBuilder.Entity<Transaction>().HasKey(e => e.Id);
        modelBuilder.Entity<TransactionStatus>().HasKey(e => e.Id);



        modelBuilder.Entity<Account>()
            .HasOne<Customer>()
            .WithMany(e => e.Accounts)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<Account>(e => e.SenderAccount)
            .WithMany(e=>e.OutgoingTransactions)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Transaction>()
            .HasOne<Account>(e => e.RecipientAccount)
            .WithMany(e=>e.IncomingTransactions)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<TransactionStatus>(e => e.Status)
            .WithMany();

        //modelBuilder.Entity<Account>().HasIndex(e => e.Number);
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