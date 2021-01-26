using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temporal.MigrationTest.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;

namespace Temporal.MigrationTest
{
    [TestClass]
    public class MigrationTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task Can_Migrate_From_Empty_Database()
        {
            var factory = new TemporalTestDesignTimeDbContextFactory();
            TemporalTestDbContext context = factory.CreateDbContext(null);

            await context.Database.EnsureDeletedAsync();
            var migrations = await context.Database.GetPendingMigrationsAsync();

            Assert.IsTrue(migrations.Any(), "At least one migration is needed.");
            await context.Database.MigrateAsync();


            var a = context.Users.ToQueryString();
            var result = context.Users.ToArray();

            var query = from u in context.Users select u;
            var b = query.ToQueryString(); ;


            a.ToArray();

        }

        [TestMethod]
        public async Task Temporal_Query_Test()
        {
            var factory = new TemporalTestDesignTimeDbContextFactory();

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                await context.Database.EnsureDeletedAsync();
                var migrations = await context.Database.GetPendingMigrationsAsync();

                Assert.IsTrue(migrations.Any(), "At least one migration is needed.");
                await context.Database.MigrateAsync();
            }

            DateTime _CreatedDate = DateTime.UtcNow;

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                TransactionRecord[] transactions = new TransactionRecord[4];

                
                for (int i = 0; i < transactions.Length; i++)
                {
                    transactions[i] = new TransactionRecord()
                    {
                        Amount = 10,
                        CreatedDate = _CreatedDate,
                        LastModifiedDate = DateTime.MaxValue
                    };
                }

                context.AddRange(transactions);
                await context.SaveChangesAsync();
            }

            DateTime _InititalDate = DateTime.UtcNow;

            await Task.Delay(TimeSpan.FromSeconds(5));

            DateTime _IncrementDate = DateTime.UtcNow;

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var transactions = await context.Set<TransactionRecord>().ToArrayAsync();

                for (int i = 0; i < transactions.Length; i++)
                {
                    transactions[i].Amount *= (i + 1) * 10;
                    transactions[i].LastModifiedDate = _IncrementDate;
                }

                await context.SaveChangesAsync();
            }

            DateTime _IncrementedDate = DateTime.UtcNow;

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var query1 = context
                    .Set<TransactionRecord>()
                    .AsOf(_InititalDate);

                var transactions1 = await query1.ToArrayAsync();
                var sql1 = query1.ToQueryString();

                Assert.IsTrue(sql1.Contains("FOR SYSTEM_TIME AS OF"));
                Assert.AreEqual(4, transactions1.Length, "Expected transaction count is 4");
                Assert.IsTrue(transactions1.All(t => t.Amount == 10), "Initial transaction amount not matched.");


                var query2 = context
                    .Set<TransactionRecord>()
                    .AsOf(_IncrementedDate);

                var transactions2 = await query2.ToArrayAsync();
                var sql2 = query2.ToQueryString();

                Assert.IsTrue(sql2.Contains("FOR SYSTEM_TIME AS OF"));
                Assert.AreEqual(4, transactions2.Length, "Expected transaction count is 4");

                for (int i = 0; i < transactions2.Length; i++)
                {
                    Assert.AreEqual((i + 1) * 100, transactions2[i].Amount);
                }

            }

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var query1 = context
                    .Set<TransactionRecord>()
                    .All();

                var transactions1 = await query1.ToArrayAsync();
                var sql1 = query1.ToQueryString();

                Assert.IsTrue(sql1.Contains("FOR SYSTEM_TIME ALL"));
                Assert.AreEqual(8, transactions1.Length, "Expected transaction count is 8");
            }

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var query1 = context
                    .Set<TransactionRecord>()
                    .FromTo(_InititalDate, _IncrementedDate);

                var projection = query1.Select(t => new
                {
                    ValidFrom = EF.Property<DateTime>(t, TemporalAnnotationNames.DefaultStartTime),
                    ValidTo = EF.Property<DateTime>(t, TemporalAnnotationNames.DefaultEndTime)
                });

                var transactions1 = await projection.ToArrayAsync();
                
                var sql1 = projection.ToQueryString();

                var transactions = context.Set<TransactionRecord>()
                                    .FromTo(_InititalDate, _IncrementedDate)
                                    .Select(t => new
                                    {
                                        Amount = t.Amount,
                                        CreatedDate = t.CreatedDate,
                                        SysStartDate = EF.Property<DateTime>(t, TemporalAnnotationNames.DefaultStartTime),
                                        SysEndDate = EF.Property<DateTime>(t, TemporalAnnotationNames.DefaultEndTime)
                                    });

                var sql2 = transactions.ToQueryString();

                Assert.IsTrue(sql1.Contains("FOR SYSTEM_TIME FROM"));
                Assert.AreEqual(8, transactions1.Length, "Expected transaction count is 8");

                for (int i = 0; i < transactions1.Length; i++)
                {
                    Console.WriteLine("{0,-25} -> {1,25}", transactions1[i].ValidFrom, transactions1[i].ValidTo);
                }
            }

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var query1 = context
                    .Set<TransactionRecord>()
                    .ContainedIn(_CreatedDate, DateTime.UtcNow);

                var transactions1 = await query1.ToArrayAsync();
                var sql1 = query1.ToQueryString();

                Assert.IsTrue(sql1.Contains("FOR SYSTEM_TIME CONTAINED IN"));
                Assert.AreEqual(4, transactions1.Length, "Expected transaction count is 4");
            }
        }

        [TestMethod]
        public async Task Temporal_Insert_Test()
        {
            var factory = new TemporalTestDesignTimeDbContextFactory();

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                await context.Database.EnsureDeletedAsync();
                var migrations = await context.Database.GetPendingMigrationsAsync();

                Assert.IsTrue(migrations.Any(), "At least one migration is needed.");
                await context.Database.MigrateAsync();
            }

            using (TemporalTestDbContext context = factory.CreateDbContext(null))
            {
                var transaction = new TransactionRecord()
                {
                    Amount = 10,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };

                context.Add(transaction);
                await context.SaveChangesAsync();
            }
        }

        public class TemporalTestDbContext : DbContext
        {
            public TemporalTestDbContext(DbContextOptions<TemporalTestDbContext> dbContextOptions)
                : base(dbContextOptions)
            {

            }

            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<User>(b =>
                {
                    b.ToTable("Users");
                    //b.HasTemporalTable();

                    //b.HasTemporalTable(config =>
                    //{
                    //    config.StartDateColumn("DateFrom");
                    //});
                });

                modelBuilder.Entity<TransactionRecord>(b =>
                {
                    //b.HasTemporalTable();

                    b.HasTemporalTable(config =>
                    {
                        config.StartDateColumn("ValidFrom");
                        config.EndDateColumn("NewValidTo");
                        config.DataConsistencyCheck(true);
                    });
                });
            }
        }
    }
}
