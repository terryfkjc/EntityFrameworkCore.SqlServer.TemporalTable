using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

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

        public virtual TemporalConfiguration<TEntity> StartDateColumn(Expression<Func<TEntity, DateTime>> propertyExpression)
        {
            var member = propertyExpression.GetMemberAccess();

            this.EntityTypeBuilder.Ignore(TemporalAnnotationNames.DefaultStartTime);

            this.EntityTypeBuilder
                .Property(propertyExpression)
                .HasConversion(DateToDatabase, DateFromDatabase)
                .SetStartDateColumn(member.GetSimpleMemberName())
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }

        public virtual TemporalConfiguration<TEntity> EndDateColumn(Expression<Func<TEntity, DateTime>> propertyExpression)
        {
            var member = propertyExpression.GetMemberAccess();

            this.EntityTypeBuilder.Ignore(TemporalAnnotationNames.DefaultEndTime);

            this.EntityTypeBuilder
                .Property(propertyExpression)
                .HasConversion(DateToDatabase, DateFromDatabase)
                .SetEndDateColumn(member.GetSimpleMemberName())
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }
    }
}
