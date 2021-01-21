using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EntityFrameworkCore.SqlServer.TemporalTable.Extensions;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Design;
using static Temporal.MigrationTest.MigrationTest;

namespace Temporal.MigrationTest
{
    class EnhancedDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddSingleton<ICSharpMigrationOperationGenerator, TemporalCSharpMigrationOperationGenerator>();
        }
    }

    class TemporalTestDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TemporalTestDbContext>
    {
        public TemporalTestDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<TemporalTestDbContext> optionsBuilder = 
                new DbContextOptionsBuilder<TemporalTestDbContext>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<TemporalTestDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseInternalServiceProvider(provider);
            });

            services.AddEntityFrameworkSqlServer();
            services.RegisterTemporalTablesForDatabase();
            var provider = services.BuildServiceProvider();

            return provider.GetService<TemporalTestDbContext>();
        }
    }
}
