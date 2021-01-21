using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Metadata
{
    public static class TemporalAnnotationNames
    {
        public const string IsTemporal = "Relational:IsTemporal";
        public const string HistoryTable = "Relational:HistoryTable";
        public const string HistorySchema = "Relational:HistorySchema";
        public const string SysStartDate = "Relational:SysStartDate";
        public const string SysEndDate = "Relational:SysEndDate";
        public const string DataConsistencyCheck = "Relational:DataConsistencyCheck";

        public const string DefaultStartTime = "SysStartTime";
        public const string DefaultEndTime = "SysEndTime";
        public const string DefaultSchema = "dbo";

    }
}
