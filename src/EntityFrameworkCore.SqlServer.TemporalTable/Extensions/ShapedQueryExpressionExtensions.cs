using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Extensions
{
    internal static class ShapedQueryExpressionExtensions
    {
        public static TemporalTableExpression FindTemporalTable(this ShapedQueryExpression shapedQuery)
        {
            if (shapedQuery.QueryExpression is SelectExpression select)
            {
                return select.Tables.OfType<TemporalTableExpression>().FirstOrDefault();
            }

            return null;
        }
    }
}
