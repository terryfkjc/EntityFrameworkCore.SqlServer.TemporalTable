using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class EnableTemporalOnSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                table: "Subscription",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "'9999-12-31 23:59:59.9999999'");

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                table: "Subscription",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.EnableTemporalTable(
                table: "Subscription",
                historySchema: "dbo",
                historyTable: "SubscriptionHistories",
                startColumn: "SysStartTime",
                endColumn: "SysEndTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTemporalTable(
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                table: "Subscription");
        }
    }
}
