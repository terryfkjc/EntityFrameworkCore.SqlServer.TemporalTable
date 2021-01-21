using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class SingletonRelationalCommandBuilderFactory : IRelationalCommandBuilderFactory
    {
        private Lazy<IRelationalCommandBuilder> _builder;

        public SingletonRelationalCommandBuilderFactory(IRelationalCommandBuilderFactory innerFactory)
        {
            _builder = new Lazy<IRelationalCommandBuilder>(() => innerFactory.Create());
        }

        public IRelationalCommandBuilder Create()
        {
            return _builder.Value;
        }
    }
}
