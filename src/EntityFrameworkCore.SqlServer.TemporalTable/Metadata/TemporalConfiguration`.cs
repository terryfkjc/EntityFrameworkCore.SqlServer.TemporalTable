using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Metadata
{
    public class TemporalConfiguration<TEntity> : TemporalConfiguration where TEntity : class
    {
        public TemporalConfiguration(EntityTypeBuilder<TEntity> entityTypeBuilder)
            : base(entityTypeBuilder)
        {
            
        }


        private int _Retention;
        private RetentionPeriod _RetentionPeriod;

        protected new EntityTypeBuilder<TEntity> EntityTypeBuilder => base.EntityTypeBuilder as EntityTypeBuilder<TEntity>;
    }
}
