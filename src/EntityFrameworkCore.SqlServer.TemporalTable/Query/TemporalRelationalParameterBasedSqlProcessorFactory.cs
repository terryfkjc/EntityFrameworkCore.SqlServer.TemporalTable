using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalRelationalParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
    {
        private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;

        public TemporalRelationalParameterBasedSqlProcessorFactory(
            RelationalParameterBasedSqlProcessorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        {
            return new TemporalParameterBasedSqlProcessor(_dependencies, useRelationalNulls);
        }
    }
}
