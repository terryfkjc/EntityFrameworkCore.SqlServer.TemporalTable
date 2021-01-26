using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    public static class TemporalRelationalExtensions
    {
        #region IEntityType
        public static bool HasTemporalTable(this IEntityType entityType)
        {
            return entityType[TemporalAnnotationNames.IsTemporal] as bool? ?? false;
        }

        public static bool DataConsistencyCheck(this IEntityType entityType)
        {
            return entityType[TemporalAnnotationNames.DataConsistencyCheck] as bool? ?? false;
        }

        public static string GetHistoryTableName(this IEntityType entityType)
        {
            return entityType[TemporalAnnotationNames.HistoryTable] as string ?? entityType.GetTableName() + "Histories";
        }

        public static string GetHistoryTableSchema(this IEntityType entityType)
        {
            return entityType[TemporalAnnotationNames.HistorySchema] as string;
        }

        public static string GetStartDateColumnName(this IEntityType entityType)
        {
            if (entityType.HasTemporalTable() == false)
            {
                return null;
            }

            foreach (var property in entityType.GetProperties())
            {
                var a = property.FindAnnotation(TemporalAnnotationNames.SysStartDate);
                if (a != null)
                {
                    var identifier = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
                    return property.GetColumnName(identifier.Value);
                }
            }

            return TemporalAnnotationNames.DefaultStartTime;
        }

        public static string GetEndDateColumnName(this IEntityType entityType)
        {
            if (entityType.HasTemporalTable() == false)
            {
                return null;
            }

            foreach (var property in entityType.GetProperties())
            {
                var a = property.FindAnnotation(TemporalAnnotationNames.SysEndDate);
                if (a != null)
                {
                    var identifier = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
                    return property.GetColumnName(identifier.Value);
                }
            }

            return TemporalAnnotationNames.DefaultEndTime;
        }
        #endregion

        #region IMutableProperty
        public static IMutableProperty SetStartDateColumn(this IMutableProperty property, string column)
        {
            property.SetAnnotation(TemporalAnnotationNames.SysStartDate, true);
            property.SetColumnName(column);
            return property;
        }

        public static bool IsStartDateColumn(this IMutableProperty property)
        {
            return property.FindAnnotation(TemporalAnnotationNames.SysStartDate) != null;
        }

        public static IMutableProperty RemoveStartDateColumn(this IMutableProperty property)
        {
            property.RemoveAnnotation(TemporalAnnotationNames.SysStartDate);
            return property;
        }

        public static IMutableProperty SetEndDateColumn(this IMutableProperty property, string column)
        {
            property.SetAnnotation(TemporalAnnotationNames.SysEndDate, true);
            property.SetColumnName(column);
            return property;
        }

        public static bool IsEndDateColumn(this IMutableProperty property)
        {
            return property.FindAnnotation(TemporalAnnotationNames.SysEndDate) != null;
        }

        public static IMutableProperty RemoveEndDateColumn(this IMutableProperty property)
        {
            property.RemoveAnnotation(TemporalAnnotationNames.SysEndDate);
            return property;
        }
        #endregion

        #region IMutableEntityType
        public static IMutableEntityType SetHistoryTable(this IMutableEntityType mutableEntityType, string table)
        {
            mutableEntityType.SetAnnotation(TemporalAnnotationNames.HistoryTable, table);
            return mutableEntityType;
        }

        public static IMutableEntityType SetHistoryTableSchema(this IMutableEntityType mutableEntityType, string schema)
        {
            mutableEntityType.SetAnnotation(TemporalAnnotationNames.HistorySchema, schema);
            return mutableEntityType;
        }
        #endregion

        #region PropertyBuilder
        public static PropertyBuilder SetStartDateColumn(this PropertyBuilder property, string column)
        {
            property.Metadata.SetStartDateColumn(column);
            return property;
        }

        public static PropertyBuilder SetEndDateColumn(this PropertyBuilder property, string column)
        {
            property.Metadata.SetEndDateColumn(column);
            return property;
        }
        #endregion

        #region IModel
        public static IEntityType FindEntity(this IModel model, string table, string schema)
        {
            foreach (var entity in model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                var schemaName = entity.GetSchema();

                if (table == tableName && ((schema == null && schemaName == null) || schema == schemaName))
                {
                    return entity;
                }
            }

            return null;
        } 
        #endregion
    }
}
