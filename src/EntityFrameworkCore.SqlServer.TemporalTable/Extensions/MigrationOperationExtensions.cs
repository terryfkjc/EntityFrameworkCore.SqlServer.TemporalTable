using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;

namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    public static class MigrationOperationExtensions
    {
        public static bool IsTemporalTable(this TableOperation migrationOperation)
        {
            var temporalAnnotation = migrationOperation.FindAnnotation(TemporalAnnotationNames.IsTemporal);
            if (temporalAnnotation != null)
            {
                return temporalAnnotation.Value as bool? ?? false;
            }

            return false;
        }

        public static string GetHistoryTableName(this TableOperation tableOperation)
        {
            var temporalAnnotation = tableOperation.FindAnnotation(TemporalAnnotationNames.HistoryTable);
            return temporalAnnotation?.Value as string ?? tableOperation.Name + "History";
        }

        public static string GetHistoryTableSchema(this TableOperation tableOperation)
        {
            var temporalAnnotation = tableOperation.FindAnnotation(TemporalAnnotationNames.HistorySchema);
            return temporalAnnotation?.Value as string;
        }

        public static string GetSysStartColumnName(this TableOperation tableOperation)
        {
            var temporalAnnotation = tableOperation.FindAnnotation(TemporalAnnotationNames.SysStartDate);
            return temporalAnnotation?.Value as string ?? TemporalAnnotationNames.DefaultStartTime;
        }

        public static string GetSysEndColumnName(this TableOperation tableOperation)
        {
            var temporalAnnotation = tableOperation.FindAnnotation(TemporalAnnotationNames.SysEndDate);
            return temporalAnnotation?.Value as string ?? TemporalAnnotationNames.DefaultEndTime;
        }
    }
}
