using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Migrations
{
    public class TemporalTablesMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public TemporalTablesMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
        }

        protected virtual void Generate(EnableTemporalTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            string _SchemaQualifiedTableName = 
                Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema);

            string _StartDate = operation.SysStartDate;
            string _EndDate = operation.SysEndDate;

            builder.AppendLine("IF NOT EXISTS(");

            using (builder.Indent())
            {
                builder
                    .AppendLine("SELECT NULL FROM sys.[columns]")
                    .AppendLine($"WHERE OBJECT_ID('{_SchemaQualifiedTableName}') = [object_id] AND [name] = '{_StartDate}'");
            }

            builder.AppendLine(") AND NOT EXISTS(");

            using (builder.Indent())
            {
                builder
                    .AppendLine("SELECT NULL FROM sys.[columns]")
                    .AppendLine($"WHERE OBJECT_ID('{_SchemaQualifiedTableName}') = [object_id] AND [name] = '{_EndDate}'");
            }

            builder
                .AppendLine(")")
                .AppendLine("BEGIN");

            using (builder.Indent())
            {
                builder
                    .Append("ALTER TABLE ")
                    .AppendLine(_SchemaQualifiedTableName)
                    .AppendLine("ADD ");


                using (builder.Indent())
                {
                    builder
                        .Append($"{_StartDate} DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN")
                        .AppendLine();

                    using (builder.Indent())
                    {
                        builder
                            .Append("CONSTRAINT ")
                            .Append($"DF_{operation.Name}_{_StartDate}")
                            .Append(" DEFAULT (SYSUTCDATETIME())")
                            .AppendLine(",");
                    }

                    builder
                        .Append($"{_EndDate} DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN")
                        .AppendLine();


                    using (builder.Indent())
                    {
                        builder
                            .Append("CONSTRAINT ")
                            .Append($"DF_{operation.Name}_{_EndDate}")
                            .Append(" DEFAULT('9999-12-31 23:59:59.9999999')")
                            .AppendLine(",");
                    }

                    builder.Append($"PERIOD FOR SYSTEM_TIME([{_StartDate}], [{_EndDate}])");
                }
            }

            builder
                .AppendLine()
                .AppendLine("END")
                .EndCommand();

            builder.AppendLine("IF NOT EXISTS(SELECT NULL FROM sys.[periods]");

            using (builder.Indent())
            {
                builder.AppendLine($"WHERE [object_id] = OBJECT_ID('{_SchemaQualifiedTableName}'))");
            }

            builder.AppendLine("BEGIN");

            using (builder.Indent())
            {
                builder.AppendLine($"EXEC('ALTER TABLE {_SchemaQualifiedTableName} ADD PERIOD FOR SYSTEM_TIME([{_StartDate}], [{_EndDate}])')");
            }

            builder
                .AppendLine("END")
                .EndCommand();


            builder
                .Append("ALTER TABLE ")
                .Append(_SchemaQualifiedTableName)
                .Append(" SET (SYSTEM_VERSIONING = ON ")
                .Append("(HISTORY_TABLE=")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.HistoryTable, operation.HistorySchema ?? TemporalAnnotationNames.DefaultSchema))
                .AppendLine("))")
                .EndCommand();

        }

        protected virtual void Generate(DisableTemporalTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            string _SchemaQualifiedTableName = Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name, operation.Schema);

            builder
                .Append($"ALTER TABLE {_SchemaQualifiedTableName}")
                .Append(" SET (SYSTEM_VERSIONING = OFF)")
                .AppendLine()
                .EndCommand();

            builder
                .Append($"ALTER TABLE {_SchemaQualifiedTableName}")
                .Append(" DROP PERIOD FOR SYSTEM_TIME")
                .AppendLine()
                .EndCommand();
        }

        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation is EnableTemporalTableOperation enableTemporalTableOperation)
            {
                Generate(enableTemporalTableOperation, model, builder);
            }
            else if (operation is DisableTemporalTableOperation disableTemporalTableOperation)
            {
                Generate(disableTemporalTableOperation, model, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }
    }
}
