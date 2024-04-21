﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Transactions;

#nullable disable

namespace Transactions.Migrations
{
    [DbContext(typeof(TransactionsContext))]
    [Migration("20240421132132_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Aggregates.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("RecipientAccountId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SenderAccountId")
                        .HasColumnType("uuid");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("RecipientAccountId");

                    b.HasIndex("SenderAccountId");

                    b.HasIndex("StatusId");

                    b.ToTable("Transaction", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.TransactionStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TransactionStatuses");
                });

            modelBuilder.Entity("Domain.Aggregates.Account", b =>
                {
                    b.HasOne("Domain.Aggregates.Customer", null)
                        .WithMany("Accounts")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.OwnsOne("Domain.Aggregates.AccountNumber", "Number", b1 =>
                        {
                            b1.Property<Guid>("AccountId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("Number");

                            b1.HasKey("AccountId");

                            b1.HasIndex("Value");

                            b1.ToTable("Account");

                            b1.WithOwner()
                                .HasForeignKey("AccountId");
                        });

                    b.Navigation("Number")
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Aggregates.Transaction", b =>
                {
                    b.HasOne("Domain.Aggregates.Account", null)
                        .WithMany("OutgoingTransactions")
                        .HasForeignKey("AccountId");

                    b.HasOne("Domain.Aggregates.Account", "RecipientAccount")
                        .WithMany("IncomingTransactions")
                        .HasForeignKey("RecipientAccountId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Domain.Aggregates.Account", "SenderAccount")
                        .WithMany()
                        .HasForeignKey("SenderAccountId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("Domain.Aggregates.TransactionStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RecipientAccount");

                    b.Navigation("SenderAccount");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Domain.Aggregates.Account", b =>
                {
                    b.Navigation("IncomingTransactions");

                    b.Navigation("OutgoingTransactions");
                });

            modelBuilder.Entity("Domain.Aggregates.Customer", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
