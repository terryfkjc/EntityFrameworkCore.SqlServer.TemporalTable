using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbSetExtensions
    {
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

        public static IQueryable<TEntity> AsOf<TEntity>(this IQueryable<TEntity> source, DateTime date)
            where TEntity : class
        {
            return
              source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                  Expression.Call(
                    instance: null,
                    method: AsOfMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    arg0: source.Expression,
                    arg1: Expression.Constant(date)))
                .AsNoTracking()
                : source;
        }

        public static IQueryable<TEntity> BetweenAnd<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            return
              source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                  Expression.Call(
                    instance: null,
                    method: BetweenMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    arg0: source.Expression,
                    arg1: Expression.Constant(startDate), 
                    arg2: Expression.Constant(endDate)))
                .AsNoTracking()
                : source;
        }

        public static IQueryable<TEntity> FromTo<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            return
              source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                  Expression.Call(
                    instance: null,
                    method: FromToMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    arg0: source.Expression,
                    arg1: Expression.Constant(startDate),
                    arg2: Expression.Constant(endDate)))
                .AsNoTracking()
                : source;
        }

        public static IQueryable<TEntity> ContainedIn<TEntity>(this IQueryable<TEntity> source, DateTime startDate, DateTime endDate)
            where TEntity : class
        {
            return
              source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                  Expression.Call(
                    instance: null,
                    method: ContainedInMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    arg0: source.Expression,
                    arg1: Expression.Constant(startDate),
                    arg2: Expression.Constant(endDate)))
                .AsNoTracking()
                : source;
        }

        public static IQueryable<TEntity> All<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class
        {
            return
              source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(
                  Expression.Call(
                    AllMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    source.Expression))
                .AsNoTracking()
                : source;
        }
    }
}

