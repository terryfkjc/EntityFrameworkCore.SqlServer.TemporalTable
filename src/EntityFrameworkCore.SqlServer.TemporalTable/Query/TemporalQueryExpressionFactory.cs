using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal class TemporalQueryExpressionFactory : SqlExpressionFactory
    {
        public TemporalQueryExpressionFactory(SqlExpressionFactoryDependencies dependencies)
            : base(dependencies)
        {

        }

        public override SelectExpression Select(IEntityType entityType)
        {
            if (entityType.HasTemporalTable())
            {
                var _TemporalTableExpression = new TemporalTableExpression(
                    entityType.GetTableName(),
                    entityType.GetSchema(),
                    entityType.GetTableName().ToLower().Substring(0, 1));

                return base.Select(entityType, _TemporalTableExpression);
            }

            return base.Select(entityType);
        }
    }
}
