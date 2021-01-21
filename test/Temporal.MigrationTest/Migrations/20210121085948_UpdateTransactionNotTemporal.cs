using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class UpdateTransactionNotTemporal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTemporalTable(
                table: "TransactionRecord");

            migrationBuilder.DropColumn(
                name: "NewValidTo",
                table: "TransactionRecord");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "TransactionRecord");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NewValidTo",
                table: "TransactionRecord",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "TransactionRecord",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.EnableTemporalTable(
                table: "TransactionRecord",
                historyTable: "TransactionRecordHistories",
                startColumn: "ValidFrom",
                endColumn: "NewValidTo");
        }
    }
}
