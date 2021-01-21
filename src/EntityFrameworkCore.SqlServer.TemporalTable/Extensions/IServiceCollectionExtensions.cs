using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Design;
using EntityFrameworkCore.SqlServer.TemporalTable.Query;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTemporalTablesForDatabase(
            this IServiceCollection services)
        {
            services.AddScoped<IMigrationsSqlGenerator, TemporalTablesMigrationsSqlGenerator>();
            services.AddScoped<IMigrationsModelDiffer, TemporalMigrationsModelDiffer>();

            services.AddSingleton<IQuerySqlGeneratorFactory, TemporalQuerySqlGeneratorFactory>();
            services.AddSingleton<IQueryableMethodTranslatingExpressionVisitorFactory, TemporalQueryableMethodTranslatingExpressionVisitorFactory>();
            services.AddSingleton<ISqlExpressionFactory, TemporalQueryExpressionFactory>();
            services.AddSingleton<IRelationalParameterBasedSqlProcessorFactory, TemporalRelationalParameterBasedSqlProcessorFactory>();


            return services;
        }
    }
}
