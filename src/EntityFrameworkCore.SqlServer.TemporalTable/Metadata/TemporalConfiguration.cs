﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Metadata
{
    public class TemporalConfiguration
    {
        public TemporalConfiguration(EntityTypeBuilder entityTypeBuilder)
        {
            EntityTypeBuilder = entityTypeBuilder;

            entityTypeBuilder.Metadata.SetAnnotation(TemporalAnnotationNames.IsTemporal, true);
            this.StartDateColumn(TemporalAnnotationNames.DefaultStartTime);
            this.EndDateColumn(TemporalAnnotationNames.DefaultEndTime);
            this.HistorySchema(TemporalAnnotationNames.DefaultSchema);
        }

        private int _Retention;
        private RetentionPeriod _RetentionPeriod;

        public virtual TemporalConfiguration HistoryTable(string table)
        {
            this.EntityTypeBuilder.Metadata.SetHistoryTable(table);
            return this;
        }

        public virtual TemporalConfiguration HistorySchema(string schema)
        {
            this.EntityTypeBuilder.Metadata.SetHistoryTableSchema(schema);
            return this;
        }

        public virtual TemporalConfiguration StartDateColumn(string column)
        {
            this.EntityTypeBuilder
                .Property<DateTime>(TemporalAnnotationNames.DefaultStartTime)
                .SetStartDateColumn(column)
                //.HasDefaultValueSql("SYSUTCDATETIME()")
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }

        public virtual TemporalConfiguration EndDateColumn(string column)
        {
            this.EntityTypeBuilder
                .Property<DateTime>(TemporalAnnotationNames.DefaultEndTime)
                .SetEndDateColumn(column)
                //.HasDefaultValueSql("'9999-12-31 23:59:59.9999999'")
                .ValueGeneratedOnAddOrUpdate();

            return this;
        }

        protected EntityTypeBuilder EntityTypeBuilder { get; }

        public void HasInfiniteRetentionPeriod()
        {
            _Retention = -1;
            _RetentionPeriod = 0;
        }

        public void HasRetentionPeriod(int number, RetentionPeriod retentionPeriod)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            if (Enum.IsDefined(typeof(RetentionPeriod), retentionPeriod) == false)
            {
                throw new ArgumentException("Invalid RetentionPeriod value");
            }

            _Retention = number;
            _RetentionPeriod = retentionPeriod;
        }

        public void DataConsistencyCheck(bool check)
        {
            if (check)
            {
                this.EntityTypeBuilder.Metadata.SetAnnotation(TemporalAnnotationNames.DataConsistencyCheck, true);
            }
            else
            {
                this.EntityTypeBuilder.Metadata.RemoveAnnotation(TemporalAnnotationNames.DataConsistencyCheck);
            }
        }
    }
}
