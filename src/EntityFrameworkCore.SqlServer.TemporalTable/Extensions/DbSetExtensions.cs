using EntityFrameworkCore.SqlServer.TemporalTable.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbSetExtensions
    {
        private sealed class TemporalIncludableQueryable<TEntity, TProperty> :
            IIncludableQueryable<TEntity, TProperty>, IAsyncEnumerable<TEntity>
        {
            private readonly IQueryable<TEntity> _queryable;

            public TemporalIncludableQueryable(IQueryable<TEntity> queryable)
            {
                _queryable = queryable;
            }

            public Expression Expression
                => _queryable.Expression;

            public Type ElementType
                => _queryable.ElementType;

            public IQueryProvider Provider
                => _queryable.Provider;

            public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => ((IAsyncEnumerable<TEntity>)_queryable).GetAsyncEnumerator(cancellationToken);

            public IEnumerator<TEntity> GetEnumerator()
                => _queryable.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

        }

        internal static readonly MethodInfo IncludeAsOfMethodInfo
            = typeof(DbSetExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(IncludeAsOf))
                .Single();

        internal static readonly MethodInfo AsOfMethodInfo
          = typeof(DbSetExtensions).GetTypeInfo()
            .GetDeclaredMethod(nameof(AsOf));

        internal static readonly MethodInfo BetweenMethodInfo
          = typeof(DbSetExtensions).GetTypeInfo()
            .GetDeclaredMethod(nameof(BetweenAnd));

        internal static readonly MethodInfo FromToMethodInfo
          = typeof(DbSetExtensions).GetTypeInfo()
            .GetDeclaredMethod(nameof(FromTo));

        internal static readonly MethodInfo ContainedInMethodInfo
          = typeof(DbSetExtensions).GetTypeInfo()
            .GetDeclaredMethod(nameof(ContainedIn));

        internal static readonly MethodInfo AllMethodInfo
          = typeof(DbSetExtensions).GetTypeInfo()
            .GetDeclaredMethod(nameof(All));

        public static IIncludableQueryable<TEntity, TProperty> IncludeAsOf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath,
            DateTime date)
            where TEntity : class
        {
            return new TemporalIncludableQueryable<TEntity, TProperty>(
                source.Provider is EntityQueryProvider
                    ? source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: IncludeAsOfMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
                            arguments: new[] { source.Expression,
                                Expression.Quote(navigationPropertyPath),
                                Expression.Constant(date)})
                        )
                    : source);
        }

        public static IQueryable<TEntity> TemporalAsOf<TEntity>(this DbSet<TEntity> source, DateTime date)
            where TEntity : class
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)queryableSource.Expression;
            var entityType = queryRootExpression.EntityType;

            return queryableSource.Provider.CreateQuery<TEntity>(
                new TemporalQueryRootExpression(queryRootExpression.QueryProvider, entityType, Expression.Constant(date))
                )
                .AsNoTracking();
        }

        public static IQueryable<TEntity> TemporalContainedIn<TEntity>(
            this DbSet<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)queryableSource.Expression;
            var entityType = queryRootExpression.EntityType;

            return queryableSource.Provider.CreateQuery<TEntity>(
                new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider, entityType,
                    Expression.Constant(startDate), Expression.Constant(endDate), TemporalQueryType.ContainedIn)
                ).AsNoTracking();
        }

        public static IQueryable<TEntity> TemporalBetween<TEntity>(
            this DbSet<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)queryableSource.Expression;
            var entityType = queryRootExpression.EntityType;

            return queryableSource.Provider.CreateQuery<TEntity>(
                new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider, entityType,
                    Expression.Constant(startDate), Expression.Constant(endDate), TemporalQueryType.BetweenAnd)
                ).AsNoTracking();
        }

        public static IQueryable<TEntity> TemporalFromTo<TEntity>(
            this DbSet<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)queryableSource.Expression;
            var entityType = queryRootExpression.EntityType;

            return queryableSource.Provider.CreateQuery<TEntity>(
                new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider, entityType,
                    Expression.Constant(startDate), Expression.Constant(endDate), TemporalQueryType.FromTo)
                ).AsNoTracking();
        }

        public static IQueryable<TEntity> TemporalAll<TEntity>(
            this DbSet<TEntity> source)
            where TEntity : class
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)queryableSource.Expression;
            var entityType = queryRootExpression.EntityType;

            return queryableSource.Provider.CreateQuery<TEntity>(
                new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider, entityType)
                ).AsNoTracking();
        }

        public static IQueryable<TEntity> AsOf<TEntity>(this IQueryable<TEntity> source, DateTime date)
            where TEntity : class
        {
            DbSet<TEntity> queryableSource = source as DbSet<TEntity>;
            return queryableSource != null ? queryableSource.TemporalAsOf(date) : source;
        }

        public static IQueryable<TEntity> BetweenAnd<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            DbSet<TEntity> queryableSource = source as DbSet<TEntity>;
            return queryableSource != null ? queryableSource.TemporalBetween(startDate, endDate) : source;
        }

        public static IQueryable<TEntity> FromTo<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            DbSet<TEntity> queryableSource = source as DbSet<TEntity>;
            return queryableSource != null ? queryableSource.FromTo(startDate, endDate) : source;
        }

        public static IQueryable<TEntity> ContainedIn<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            DbSet<TEntity> queryableSource = source as DbSet<TEntity>;
            return queryableSource != null ? queryableSource.TemporalContainedIn(startDate, endDate) : source;
        }

        public static IQueryable<TEntity> All<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class
        {
            DbSet<TEntity> queryableSource = source as DbSet<TEntity>;
            return queryableSource != null ? queryableSource.TemporalAll() : source;
        }
    }
}

