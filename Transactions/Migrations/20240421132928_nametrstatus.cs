using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.Migrations
{
    /// <inheritdoc />
    public partial class nametrstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TransactionStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "TransactionStatuses");
        }
    }
}
