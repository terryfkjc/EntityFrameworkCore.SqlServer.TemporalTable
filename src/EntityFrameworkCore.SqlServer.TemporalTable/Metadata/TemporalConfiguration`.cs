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

        public new TemporalConfiguration<TEntity> StartDateColumn(string column)
        {
            this.EntityTypeBuilder
                .Property<DateTime>(TemporalAnnotationNames.DefaultStartTime)
                .SetStartDateColumn(column)
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }

        public new TemporalConfiguration<TEntity> EndDateColumn(string column)
        {
            this.EntityTypeBuilder
                .Property<DateTime>(TemporalAnnotationNames.DefaultEndTime)
                .SetEndDateColumn(column)
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }

        //public TemporalConfiguration<TEntity> StartDateColumn(Expression<Func<TEntity, DateTime>> propertyExpression)
        //{
        //    var identifier = StoreObjectIdentifier.Create(this.entityTypeBuilder.Metadata, StoreObjectType.Table).Value;

        //    foreach (var property in this.entityTypeBuilder.Metadata.GetProperties())
        //    {
        //        if (property.IsStartDateColumn())
        //        {
        //            property.RemoveStartDateColumn();
        //            break;
        //        }
        //    }

        //    this.entityTypeBuilder.Property(propertyExpression).SetStartDateColumn();
        //    return this;
        //}

        //public TemporalConfiguration<TEntity> EndDateColumn(Expression<Func<TEntity, DateTime>> propertyExpression)
        //{
        //    var identifier = StoreObjectIdentifier.Create(this.entityTypeBuilder.Metadata, StoreObjectType.Table).Value;

        //    foreach (var property in this.entityTypeBuilder.Metadata.GetProperties())
        //    {
        //        if (property.IsEndDateColumn())
        //        {
        //            property.RemoveEndDateColumn();
        //            break;
        //        }
        //    }

        //    this.entityTypeBuilder.Property<DateTime>(propertyExpression).SetEndDateColumn();
        //    return this;
        //}

        //public bool DataConsistencyCheck { get; set; }

        //public void HasInfiniteRetentionPeriod()
        //{
        //    _Retention = -1;
        //    _RetentionPeriod = 0;
        //}

        //public void HasRetentionPeriod(int number, RetentionPeriod retentionPeriod)
        //{
        //    if (number <= 0)
        //    {
        //        throw new ArgumentException(nameof(number));
        //    }

        //    if (Enum.IsDefined(typeof(RetentionPeriod), retentionPeriod) == false)
        //    {
        //        throw new ArgumentException("Invalid RetentionPeriod value");
        //    }

        //    _Retention = number;
        //    _RetentionPeriod = retentionPeriod;
        //}
    }
}
