﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;

namespace Microsoft.EntityFrameworkCore
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<TEntity> HasTemporalTable<TEntity>(this EntityTypeBuilder<TEntity> builder) 
            where TEntity : class
        {
            TemporalConfiguration temporalConfiguration = new TemporalConfiguration(builder);
            return builder;
        }

        public static EntityTypeBuilder<TEntity> HasTemporalTable<TEntity>(
            this EntityTypeBuilder<TEntity> builder,
            Action<TemporalConfiguration<TEntity>> configuration)
            where TEntity : class
        {
            TemporalConfiguration<TEntity> temporalConfiguration = new TemporalConfiguration<TEntity>(builder);
            configuration?.Invoke(temporalConfiguration);
            return builder;
        }

        public static EntityTypeBuilder HasTemporalTable(this EntityTypeBuilder builder)
        {
            TemporalConfiguration temporalConfiguration = new TemporalConfiguration(builder);
            return builder;
        }

        public static EntityTypeBuilder HasTemporalTable(
            this EntityTypeBuilder builder,
            Action<TemporalConfiguration> configuration)
        {
            TemporalConfiguration temporalConfiguration = new TemporalConfiguration(builder);
            configuration?.Invoke(temporalConfiguration);
            return builder;
        }
    }
}
