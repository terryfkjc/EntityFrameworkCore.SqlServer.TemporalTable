using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Operations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Design
{
    public class TemporalCSharpMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public TemporalCSharpMigrationOperationGenerator(
            CSharpMigrationOperationGeneratorDependencies dependencies)
            : base(dependencies)
        {

        }

        private ICSharpHelper Code
            => Dependencies.CSharpHelper;

        public override void Generate(
            string builderName,
            IReadOnlyList<MigrationOperation> operations,
            IndentedStringBuilder builder)
        {
            
            var first = true;
            foreach (var operation in operations)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder
                        .AppendLine()
                        .AppendLine();
                }

                builder.Append(builderName);
                Generate((dynamic)operation, builder);
                builder.Append(";");
            }
        }

        protected virtual void Generate(DisableTemporalTableOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($".{nameof(MigrationBuilderExtensions.DisableTemporalTable)}(");

            using (builder.Indent())
            {
                builder
                   .Append("table: ")
                   .Append(Code.Literal(operation.Name));

                if (string.IsNullOrEmpty(operation.Schema) == false)
                {
                    builder.AppendLine(",");

                    builder
                        .Append("schema: ")
                        .Append(Code.Literal(operation.Schema));
                }
            }

            builder.Append(")");
        }

        protected virtual void Generate(EnableTemporalTableOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($".{nameof(MigrationBuilderExtensions.EnableTemporalTable)}(");

            using (builder.Indent())
            {
                builder
                   .Append("table: ")
                   .Append(Code.Literal(operation.Name));

                if (operation.Schema != null)
                {
                    builder
                        .AppendLine(",")
                        .Append("schema: ")
                        .Append(Code.Literal(operation.Schema));
                }

                if (operation.HistorySchema != null)
                {

                    builder
                        .AppendLine(",")
                        .Append("historySchema: ")
                        .Append(Code.Literal(operation.HistorySchema));
                }

                if (operation.HistoryTable != null)
                {
                    builder
                        .AppendLine(",")
                        .Append("historyTable: ")
                        .Append(Code.Literal(operation.HistoryTable));
                }

                if (operation.SysStartDate != null)
                {
                    builder
                        .AppendLine(",")
                        .Append("startColumn: ")
                        .Append(Code.Literal(operation.SysStartDate));
                }

                if (operation.SysEndDate != null)
                {
                    builder
                        .AppendLine(",")
                        .Append("endColumn: ")
                        .Append(Code.Literal(operation.SysEndDate));
                }

                if (operation.DataConsistencyCheck)
                {
                    builder
                        .AppendLine(",")
                        .Append("dataConsistencyCheck: ")
                        .Append(Code.Literal(operation.DataConsistencyCheck));
                }
            }

            builder.Append(")");
        }

        protected override void Generate(CreateTableOperation operation, IndentedStringBuilder builder)
        {
            base.Generate(operation, builder);

            if (operation.IsTemporalTable())
            {
                builder.AppendLine(";");
                builder.AppendLine();

                //"migrationBuilder" is hardcoded name in CSharpMigrationsGenerator class
                builder.Append("migrationBuilder");

                EnableTemporalTableOperation enableTemporalTableOperation = new EnableTemporalTableOperation()
                {
                    Name = operation.Name,
                    Schema = operation.Schema,
                    HistoryTable = operation.GetHistoryTableName(),
                    HistorySchema = operation.GetHistoryTableSchema(),
                    SysStartDate = operation.GetSysStartColumnName(),
                    SysEndDate = operation.GetSysEndColumnName(),
                    DataConsistencyCheck = operation.DataConsistencyCheck()
                };

                this.Generate(enableTemporalTableOperation, builder);
            }
        }
    }
}
