using Microsoft.EntityFrameworkCore.Migrations;

namespace SampleMvcApplication.Migrations
{
    public partial class AddTemporal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnableTemporalTable(
                table: "Students",
                historyTable: "StudentsHistories",
                startColumn: "SysStartTime",
                endColumn: "SysEndTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DisableTemporalTable(
                table: "Students");
        }
    }
}
