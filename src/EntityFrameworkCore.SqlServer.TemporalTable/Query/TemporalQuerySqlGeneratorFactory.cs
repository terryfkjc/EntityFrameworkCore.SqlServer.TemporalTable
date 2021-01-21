using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Query;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal class TemporalQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;

        public TemporalQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public QuerySqlGenerator Create()
        {
            return new TemporalSqlServerQuerySqlGenerator(_dependencies);
        }
    }
}
