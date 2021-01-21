using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Operations
{
    public class EnableTemporalTableOperation : TableOperation
    {
        public string HistorySchema { get; set; }
        public string HistoryTable { get; set; }
        public string SysStartDate { get; set; }
        public string SysEndDate { get; set; }
        public bool DataConsistencyCheck { get; set; }
    }
}
