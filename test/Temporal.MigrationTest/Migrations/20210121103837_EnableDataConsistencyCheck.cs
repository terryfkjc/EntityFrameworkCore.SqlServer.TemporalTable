using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class EnableDataConsistencyCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnableTemporalTable(
                table: "TransactionRecord",
                historyTable: "TransactionRecordHistories",
                startColumn: "ValidFrom",
                endColumn: "NewValidTo",
                dataConsistencyCheck: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnableTemporalTable(
                table: "TransactionRecord",
                historyTable: "TransactionRecordHistories",
                startColumn: "ValidFrom",
                endColumn: "NewValidTo");
        }
    }
}
