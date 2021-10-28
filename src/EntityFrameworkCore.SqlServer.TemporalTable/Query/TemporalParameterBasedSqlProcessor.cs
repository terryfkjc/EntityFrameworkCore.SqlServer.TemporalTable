﻿using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalParameterBasedSqlProcessor : SqlServerParameterBasedSqlProcessor
    {
        public TemporalParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
            : base(dependencies, useRelationalNulls)
        {
        }

        protected override SelectExpression ProcessSqlNullability(SelectExpression selectExpression, IReadOnlyDictionary<string, object> parametersValues, out bool canCache)
        {
            return new TemporalSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);
        }
    }
}
