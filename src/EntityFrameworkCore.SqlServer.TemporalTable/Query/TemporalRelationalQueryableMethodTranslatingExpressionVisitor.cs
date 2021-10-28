using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalRelationalQueryableMethodTranslatingExpressionVisitor
        : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        protected TemporalRelationalQueryableMethodTranslatingExpressionVisitor(
            TemporalRelationalQueryableMethodTranslatingExpressionVisitor parentVisitor)
            : base(parentVisitor)
        {

        }

        public TemporalRelationalQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {

        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            if (extensionExpression is TemporalQueryRootExpression _RootExpression)
            {
                var entityType = _RootExpression.EntityType;
                if (entityType.HasTemporalTable())
                {
                    TemporalTableExpression temporalTable =
                        new TemporalTableExpression(
                            _RootExpression.EntityType.GetTableName(),
                            _RootExpression.EntityType.GetSchema(),
                            _RootExpression.EntityType.GetTableName().Substring(0, 1));

                    switch (_RootExpression.TemporalQueryType)
                    {
                        case TemporalQueryType.None:
                            break;
                        case TemporalQueryType.AsOf:
                            temporalTable.SetAsOfDate(_RootExpression.AsOfDate);
                            break;
                        case TemporalQueryType.FromTo:
                            temporalTable.SetFromToDate(_RootExpression.StartDate, _RootExpression.EndDate);
                            break;
                        case TemporalQueryType.BetweenAnd:
                            temporalTable.SetBetweenAndDate(_RootExpression.StartDate, _RootExpression.EndDate);
                            break;
                        case TemporalQueryType.ContainedIn:
                            temporalTable.SetContainedInDate(_RootExpression.StartDate, _RootExpression.EndDate);
                            break;
                        case TemporalQueryType.All:
                            temporalTable.SetQueryTypeToAll();
                            break;
                        default:
                            break;
                    }

                    SelectExpression selectExpression = base.RelationalDependencies.SqlExpressionFactory
                        .Select(_RootExpression.EntityType, temporalTable);

                    return new ShapedQueryExpression(
                        selectExpression,
                        new RelationalEntityShaperExpression(
                            _RootExpression.EntityType,
                            new ProjectionBindingExpression(
                                selectExpression,
                                new ProjectionMember(),
                                typeof(ValueBuffer)),
                            false));
                }
            }

            return base.VisitExtension(extensionExpression);
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        {
            return new TemporalRelationalQueryableMethodTranslatingExpressionVisitor(this);
        }
    }
}
