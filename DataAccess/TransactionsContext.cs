﻿using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class TransactionsContext : DbContext
{
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<TransactionStatus> TransactionStatuses { get; set; }

    protected TransactionsContext()
    {
    }

    public TransactionsContext(DbContextOptions options) : base(options)
    {
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
            .WithMany(e=>e.OutgoingTransactions)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<Account>(e => e.RecipientAccount)
            .WithMany(e => e.IncomingTransactions)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne<TransactionStatus>(e => e.TransactionStatus);

        modelBuilder.Entity<Account>().HasIndex(e => e.AccountNumber);
    }
}