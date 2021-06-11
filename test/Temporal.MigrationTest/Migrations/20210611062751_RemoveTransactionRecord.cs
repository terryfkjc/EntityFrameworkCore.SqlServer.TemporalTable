﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Temporal.MigrationTest.Migrations
{
    public partial class RemoveTransactionRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTemporalTable(
                table: "TransactionRecord");

            migrationBuilder.DropTable(
                name: "TransactionRecord");

            migrationBuilder.DropTable(
                name: "TransactionRecordHistories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRecord", x => x.Id);
                });

            migrationBuilder.EnableTemporalTable(
                table: "TransactionRecord",
                historyTable: "TransactionRecordHistories",
                startColumn: "ValidFrom",
                endColumn: "NewValidTo",
                dataConsistencyCheck: true);
        }
    }
}
