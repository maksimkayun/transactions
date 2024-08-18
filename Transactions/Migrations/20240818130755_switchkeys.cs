using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.Migrations
{
    /// <inheritdoc />
    public partial class switchkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_RecipientAccountId_RecipientAccountAc~",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId_SenderAccountAccountN~",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RecipientAccountId_RecipientAccountAccountNumb~",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SenderAccountId_SenderAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RecipientAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SenderAccountAccountNumber",
                table: "Transactions");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecipientAccountId",
                table: "Transactions",
                column: "RecipientAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_RecipientAccountId",
                table: "Transactions",
                column: "RecipientAccountId",
                principalTable: "Accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId",
                principalTable: "Accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_RecipientAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RecipientAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SenderAccountId",
                table: "Transactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Accounts_AccountNumber",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.AddColumn<long>(
                name: "RecipientAccountAccountNumber",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SenderAccountAccountNumber",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                columns: new[] { "id", "AccountNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecipientAccountId_RecipientAccountAccountNumb~",
                table: "Transactions",
                columns: new[] { "RecipientAccountId", "RecipientAccountAccountNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SenderAccountId_SenderAccountAccountNumber",
                table: "Transactions",
                columns: new[] { "SenderAccountId", "SenderAccountAccountNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_RecipientAccountId_RecipientAccountAc~",
                table: "Transactions",
                columns: new[] { "RecipientAccountId", "RecipientAccountAccountNumber" },
                principalTable: "Accounts",
                principalColumns: new[] { "id", "AccountNumber" },
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId_SenderAccountAccountN~",
                table: "Transactions",
                columns: new[] { "SenderAccountId", "SenderAccountAccountNumber" },
                principalTable: "Accounts",
                principalColumns: new[] { "id", "AccountNumber" },
                onDelete: ReferentialAction.SetNull);
        }
    }
}
