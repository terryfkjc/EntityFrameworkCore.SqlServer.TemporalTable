using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalRelationalQueryableMethodTranslatingExpressionVisitor
        : RelationalQueryableMethodTranslatingExpressionVisitor
    {
        public TemporalRelationalQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {

        }

        private Expression HandleTemporalTableDateRange(MethodCallExpression methodCallExpression,
            Action<TemporalTableExpression, ParameterExpression, ParameterExpression> setter)
        {
            var _StartDateParameter = Visit(methodCallExpression.Arguments[1]) as ParameterExpression;
            var _EndDateParameter = Visit(methodCallExpression.Arguments[2]) as ParameterExpression;

            var visitMethodCall = Visit(methodCallExpression.Arguments[0]);
            if (visitMethodCall is ShapedQueryExpression shapedExpression)
            {
                var temporalTable = shapedExpression.FindTemporalTable();
                setter(temporalTable, _StartDateParameter, _EndDateParameter);
            }

            return visitMethodCall;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(DbSetExtensions))
            {
                switch (methodCallExpression.Method.Name)
                {
                    case nameof(DbSetExtensions.AsOf):
                        {
                            var _asOfDateParameter = Visit(methodCallExpression.Arguments[1]) as ParameterExpression;
                            var visitMethodCall = Visit(methodCallExpression.Arguments[0]);

                            if (visitMethodCall is ShapedQueryExpression shapedExpression)
                            {
                                var temporalTable = shapedExpression.FindTemporalTable();
                                temporalTable.SetAsOfDate(_asOfDateParameter);
                            }

                            return visitMethodCall;
                        }
                    case nameof(DbSetExtensions.BetweenAnd):
                        {
                            var visitMethodCall = 
                                HandleTemporalTableDateRange(methodCallExpression, (t, s, e) => t.SetBetweenAndDate(s, e));

                            return visitMethodCall;
                        }
                    case nameof(DbSetExtensions.FromTo):
                        {
                            var visitMethodCall =
                                HandleTemporalTableDateRange(methodCallExpression, (t, s, e) => t.SetFromToDate(s, e));

                            return visitMethodCall;
                        }
                    case nameof(DbSetExtensions.ContainedIn):
                        {
                            var visitMethodCall =
                                HandleTemporalTableDateRange(methodCallExpression, (t, s, e) => t.SetContainedInDate(s, e));

                            return visitMethodCall;
                        }
                    case nameof(DbSetExtensions.All):
                        {
                            var visitMethodCall = Visit(methodCallExpression.Arguments[0]);

                            if (visitMethodCall is ShapedQueryExpression shapedExpression)
                            {
                                var temporalTable = shapedExpression.FindTemporalTable();
                                temporalTable.SetQueryTypeToAll();
                            }

                            return visitMethodCall;
                        }
                }
            }

            return base.VisitMethodCall(methodCallExpression);
        }
    }
}
