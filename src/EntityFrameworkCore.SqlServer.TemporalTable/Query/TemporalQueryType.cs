using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal enum TemporalQueryType
    {
        None = 0,
        AsOf = 1,
        FromTo = 2,
        BetweenAnd = 3,
        ContainedIn = 4,
        All = 5
    }
}
