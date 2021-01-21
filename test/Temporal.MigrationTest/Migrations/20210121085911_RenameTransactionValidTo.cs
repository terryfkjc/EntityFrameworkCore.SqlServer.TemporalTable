using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class RenameTransactionValidTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "TransactionRecord",
                newName: "NewValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewValidTo",
                table: "TransactionRecord",
                newName: "ValidTo");
        }
    }
}
