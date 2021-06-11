using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Operations;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder EnableTemporalTable(
            this MigrationBuilder migrationBuilder,
            string table, string schema = null,
            string historyTable = null, string historySchema = null,
            string startColumn = null, string endColumn = null,
            bool dataConsistencyCheck = false)
        {
            startColumn = startColumn ?? TemporalAnnotationNames.DefaultStartTime;
            endColumn = endColumn ?? TemporalAnnotationNames.DefaultEndTime;

            EnableTemporalTableOperation operation = new EnableTemporalTableOperation()
            {
                Name = table,
                Schema = schema,
                IsDestructiveChange = false,
                HistorySchema = historySchema,
                HistoryTable = historyTable,
                SysStartDate = startColumn,
                SysEndDate = endColumn,
                DataConsistencyCheck = dataConsistencyCheck
            };

            migrationBuilder.Operations.Add(operation);

            return migrationBuilder;
        }

        public static MigrationBuilder DisableTemporalTable(
            this MigrationBuilder migrationBuilder,
            string table, string schema = null)
        {
            DisableTemporalTableOperation operation = new DisableTemporalTableOperation()
            {
                Name = table,
                Schema = schema
            };

            migrationBuilder.Operations.Add(operation);

            return migrationBuilder;
        }
    }
}
