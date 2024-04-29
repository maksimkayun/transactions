﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Transactions.DataAccess;

#nullable disable

namespace Transactions.Migrations
{
    [DbContext(typeof(TransactionsContext))]
    [Migration("20240424093425_Initial")]
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

            modelBuilder.Entity("Transactions.DataAccess.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<long>("AccountNumber")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id", "AccountNumber");

                    b.HasIndex("OwnerId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Customer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<long>("RecipientAccountAccountNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("RecipientAccountId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SenderAccountAccountNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("SenderAccountId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TransactionStatusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TransactionStatusId");

                    b.HasIndex("RecipientAccountId", "RecipientAccountAccountNumber");

                    b.HasIndex("SenderAccountId", "SenderAccountAccountNumber");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.TransactionStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TransactionStatuses");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Account", b =>
                {
                    b.HasOne("Transactions.DataAccess.Models.Customer", "Owner")
                        .WithMany("Accounts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Transaction", b =>
                {
                    b.HasOne("Transactions.DataAccess.Models.TransactionStatus", "TransactionStatus")
                        .WithMany()
                        .HasForeignKey("TransactionStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Transactions.DataAccess.Models.Account", "RecipientAccount")
                        .WithMany("IncomingTransactions")
                        .HasForeignKey("RecipientAccountId", "RecipientAccountAccountNumber")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("Transactions.DataAccess.Models.Account", "SenderAccount")
                        .WithMany("OutgoingTransactions")
                        .HasForeignKey("SenderAccountId", "SenderAccountAccountNumber")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("RecipientAccount");

                    b.Navigation("SenderAccount");

                    b.Navigation("TransactionStatus");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Account", b =>
                {
                    b.Navigation("IncomingTransactions");

                    b.Navigation("OutgoingTransactions");
                });

            modelBuilder.Entity("Transactions.DataAccess.Models.Customer", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
