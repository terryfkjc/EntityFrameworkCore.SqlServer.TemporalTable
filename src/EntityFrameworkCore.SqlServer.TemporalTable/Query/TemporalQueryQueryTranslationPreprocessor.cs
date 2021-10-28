using EntityFrameworkCore.SqlServer.TemporalTable.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalQueryQueryTranslationPreprocessor : RelationalQueryTranslationPreprocessor
    {
        public TemporalQueryQueryTranslationPreprocessor(
            QueryTranslationPreprocessorDependencies dependencies,
            RelationalQueryTranslationPreprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            RelationalQueryCompilationContext = (RelationalQueryCompilationContext)queryCompilationContext;
        }

        public RelationalQueryCompilationContext RelationalQueryCompilationContext { get; }

        public override Expression Process(Expression query)
        {
            query = new InvocationExpressionRemovingExpressionVisitor().Visit(query);
            query = NormalizeQueryableMethod(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);
            query = new SubqueryMemberPushdownExpressionVisitor(QueryCompilationContext.Model).Visit(query);

            var temporalVisitor = new TemporalNavigationExpandingExpressionVisitor(
                this, QueryCompilationContext, Dependencies.EvaluatableExpressionFilter);
            query = temporalVisitor.Expand(query);

            query = new QueryOptimizingExpressionVisitor().Visit(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);

            query = this.RelationalQueryCompilationContext.QuerySplittingBehavior == QuerySplittingBehavior.SplitQuery
                ? new SplitIncludeRewritingExpressionVisitor().Visit(query)
                : query;

            var replacer = new TemporalQueryRootReplacer(temporalVisitor);
            query = replacer.Visit(query);

            return query;
        }

        class TemporalQueryRootReplacer : ExpressionVisitor
        {
            public TemporalQueryRootReplacer(TemporalNavigationExpandingExpressionVisitor tee)
            {
                NavigationExpandingExpressionVisitor = tee;
            }

            public TemporalNavigationExpandingExpressionVisitor NavigationExpandingExpressionVisitor { get; }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is QueryRootExpression queryRootExpression)
                {
                    if (queryRootExpression is TemporalQueryRootExpression temporal)
                    {
                        return temporal;
                    }

                    if (NavigationExpandingExpressionVisitor.TemporalRoots
                        .TryGetValue(queryRootExpression.EntityType, out var _Mapping))
                    {
                        return _Mapping;
                    }

                    var _BaseRoot = NavigationExpandingExpressionVisitor.TemporalBaseRoot;
                    if (_BaseRoot != null)
                    {
                        switch (_BaseRoot.TemporalQueryType)
                        {
                            case TemporalQueryType.None:
                                break;
                            case TemporalQueryType.AsOf:
                                return new TemporalQueryRootExpression(queryRootExpression.EntityType, _BaseRoot.AsOfDate);
                            case TemporalQueryType.FromTo:
                            case TemporalQueryType.BetweenAnd:
                            case TemporalQueryType.ContainedIn:
                                return new TemporalQueryRootExpression(
                                    queryRootExpression.EntityType, _BaseRoot.StartDate,
                                    _BaseRoot.EndDate, _BaseRoot.TemporalQueryType);
                            case TemporalQueryType.All:
                                return new TemporalQueryRootExpression(queryRootExpression.EntityType);
                            default:
                                break;
                        }
                    }
                }

                return base.VisitExtension(node);
            }
        }
    }
}
