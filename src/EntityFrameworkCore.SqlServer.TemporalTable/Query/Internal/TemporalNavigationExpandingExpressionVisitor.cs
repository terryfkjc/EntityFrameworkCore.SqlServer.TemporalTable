using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query.Internal
{
    internal class TemporalNavigationExpandingExpressionVisitor : NavigationExpandingExpressionVisitor
    {
        internal static readonly MethodInfo EFIncludeMethodInfo
           = typeof(EntityFrameworkQueryableExtensions)
               .GetTypeInfo().GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.Include))
               .Single(
                   mi =>
                       mi.GetGenericArguments().Count() == 2
                       && mi.GetParameters().Any(
                           pi => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));

        private Dictionary<IEntityType, QueryRootExpression> _TemporalRoots =
            new Dictionary<IEntityType, QueryRootExpression>();

        private TemporalQueryRootExpression _TemporalRoot;

        public TemporalNavigationExpandingExpressionVisitor(
            QueryTranslationPreprocessor queryTranslationPreprocessor,
            QueryCompilationContext queryCompilationContext,
            IEvaluatableExpressionFilter evaluatableExpressionFilter) :
            base(queryTranslationPreprocessor, queryCompilationContext, evaluatableExpressionFilter)
        {
            QueryCompilationContext = queryCompilationContext;
        }

        public IReadOnlyDictionary<IEntityType, QueryRootExpression> TemporalRoots => _TemporalRoots;
        public TemporalQueryRootExpression TemporalBaseRoot => _TemporalRoot;

        protected QueryCompilationContext QueryCompilationContext { get; }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            var method = methodCallExpression.Method;

            if (method.DeclaringType == typeof(DbSetExtensions))
            {
                if (method.Name == nameof(DbSetExtensions.IncludeAsOf))
                {
                    Type[] _GenericArguments = method.GetGenericArguments();

                    var _EFBase = Expression.Call(
                        instance: null,
                        method: EFIncludeMethodInfo.MakeGenericMethod(_GenericArguments),
                        arguments: new[] {
                            methodCallExpression.Arguments[0],
                            methodCallExpression.Arguments[1] });

                    var dateparam = Visit(methodCallExpression.Arguments[2]);

                    var _EntityType = QueryCompilationContext.Model
                        .FindEntityType(_GenericArguments[1].GetGenericArguments()[0]);

                    TemporalQueryRootExpression temporalQueryRootExpression =
                        new TemporalQueryRootExpression(_EntityType, dateparam);

                    var _BaseResult = base.VisitMethodCall(_EFBase);

                    //if (this.TemporalBaseRoot != null)
                    {
                        _TemporalRoots[_EntityType] = temporalQueryRootExpression;
                    }

                    return _BaseResult;
                }
            }

            return base.VisitMethodCall(methodCallExpression);
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            if (extensionExpression is TemporalQueryRootExpression temporalQueryRootExpression)
            {
                if (_TemporalRoot != null)
                {
                    throw new InvalidProgramException("More than one TemporalQueryRootExpression found in query expression.");
                }
                _TemporalRoot = temporalQueryRootExpression;
            }

            return base.VisitExtension(extensionExpression);
        }
    }
}
