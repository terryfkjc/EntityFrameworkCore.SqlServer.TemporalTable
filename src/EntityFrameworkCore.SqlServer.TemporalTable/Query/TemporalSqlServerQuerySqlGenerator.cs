using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalSqlServerQuerySqlGenerator : SqlServerQuerySqlGenerator
    {

        private ISqlGenerationHelper SqlGenerationHelper => base.Dependencies.SqlGenerationHelper;
        private IRelationalCommandBuilder CommandBuilder { get; }

        public TemporalSqlServerQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
            : base(new QuerySqlGeneratorDependencies(
                    new SingletonRelationalCommandBuilderFactory(
                        dependencies.RelationalCommandBuilderFactory
                    ),
                    dependencies.SqlGenerationHelper
                    )
              )
        {
            this.CommandBuilder = Dependencies.RelationalCommandBuilderFactory.Create();
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            switch (extensionExpression)
            {
                case TemporalTableExpression tableExpression:
                    {
                        return VisitTemporalTable(tableExpression);
                    }
            }

            return base.VisitExtension(extensionExpression);
        }

        protected virtual Expression VisitTemporalTable(TemporalTableExpression tableExpression)
        {
            Sql.Append(SqlGenerationHelper.DelimitIdentifier(tableExpression.Name, tableExpression.Schema));

            switch (tableExpression.TemporalQueryType)
            {
                case TemporalQueryType.None:
                    break;
                case TemporalQueryType.AsOf:
                    {
                        var _AsOfDate = ((ParameterExpression)tableExpression.AsOfDate).Name;

                        Sql.Append(" FOR SYSTEM_TIME AS OF ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_AsOfDate));

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _AsOfDate))
                        {
                            CommandBuilder.AddParameter(_AsOfDate, _AsOfDate);
                        }
                        break;
                    }

                case TemporalQueryType.FromTo:
                    {
                        var _StartDate = ((ParameterExpression)tableExpression.StartDate).Name;
                        var _EndDate = ((ParameterExpression)tableExpression.EndDate).Name;

                        Sql.Append(" FOR SYSTEM_TIME FROM ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_StartDate));
                        Sql.Append(" TO ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_EndDate));

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _StartDate))
                        {
                            CommandBuilder.AddParameter(_StartDate, _StartDate);
                        }

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _EndDate))
                        {
                            CommandBuilder.AddParameter(_EndDate, _EndDate);
                        }

                        break;
                    }
                case TemporalQueryType.BetweenAnd:
                    {
                        var _StartDate = ((ParameterExpression)tableExpression.StartDate).Name;
                        var _EndDate = ((ParameterExpression)tableExpression.EndDate).Name;

                        Sql.Append(" FOR SYSTEM_TIME BETWEEN ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_StartDate));
                        Sql.Append(" AND ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_EndDate));

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _StartDate))
                        {
                            CommandBuilder.AddParameter(_StartDate, _StartDate);
                        }

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _EndDate))
                        {
                            CommandBuilder.AddParameter(_EndDate, _EndDate);
                        }

                        break;
                    }
                case TemporalQueryType.ContainedIn:
                    {
                        var _StartDate = ((ParameterExpression)tableExpression.StartDate).Name;
                        var _EndDate = ((ParameterExpression)tableExpression.EndDate).Name;

                        Sql.Append(" FOR SYSTEM_TIME CONTAINED IN(");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_StartDate));
                        Sql.Append(", ");
                        Sql.Append(SqlGenerationHelper.GenerateParameterNamePlaceholder(_EndDate));
                        Sql.Append(")");

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _StartDate))
                        {
                            CommandBuilder.AddParameter(_StartDate, _StartDate);
                        }

                        if (CommandBuilder.Parameters.All(p => p.InvariantName != _EndDate))
                        {
                            CommandBuilder.AddParameter(_EndDate, _EndDate);
                        }

                        break;
                    }
                case TemporalQueryType.All:
                    {
                        Sql.Append(" FOR SYSTEM_TIME ALL");
                        break;
                    }
                default:
                    break;
            }

            Sql
                .Append(AliasSeparator)
                .Append(SqlGenerationHelper.DelimitIdentifier(tableExpression.Alias));

            return tableExpression;
        }
    }
}
