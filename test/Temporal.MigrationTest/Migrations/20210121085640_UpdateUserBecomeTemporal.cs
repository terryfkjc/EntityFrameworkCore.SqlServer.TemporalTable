using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class UpdateUserBecomeTemporal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SysEndTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SysStartTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.EnableTemporalTable(
                table: "Users",
                historyTable: "UsersHistories",
                startColumn: "SysStartTime",
                endColumn: "SysEndTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTemporalTable(
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SysEndTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SysStartTime",
                table: "Users");
        }
    }
}
