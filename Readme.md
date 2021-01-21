
# Setup


1. Add the following class in Startup project.

```c#
class EnhancedDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        services.AddSingleton<ICSharpMigrationOperationGenerator, EnhancedCSharpMigrationOperationGenerator>();
    }
}
```

2. Use the following method to configure DbContext. UseInternalServiceProvider need to be called to let DbContext resolve depencdencies with the provider we have configure.

```c#
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
```

# Entity Configuration


1. Enable temporal table for entity


```c#
modelBuilder.Entity<User>(b =>
{
      b.ToTable("Users");
      b.HasTemporalTable();
});
```

2. Enable temporal table with custom start/end date column name

```c#
modelBuilder.Entity<TransactionRecord>(b =>
{
          b.HasTemporalTable(config =>
          {
              config.StartDateColumn("ValidFrom");
              config.EndDateColumn("NewValidTo");
          });
});
```

3. Optional. You may configure the history table name

```c#
modelBuilder.Entity<User>(b =>
{
      b.ToTable("Users");
      b.HasTemporalTable(config =>
      {
            config.HistorySchema("history");
            config.HistoryTable("UserHistories");
      });
});
```

# Temporal Query

Example: 

```c#
var transactions = context.Set<TransactionRecord>()
                    .FromTo(_InititalDate, _IncrementedDate)
                    .Select(t => new
                    {
                        Amount = t.Amount,
                        CreatedDate = t.CreatedDate,
                        SysStartDate = EF.Property<DateTime>(t, "ValidFrom"),
                        SysEndDate = EF.Property<DateTime>(t, "ValidFrom")
                    });
```

Supported Temporal Queries:

- AS OF <date_time>
- FROM <start_date_time> TO <end_date_time>
- BETWEEN <start_date_time> AND <end_date_time>
- CONTAINED IN (<start_date_time> , <end_date_time>)
- ALL