using Microsoft.EntityFrameworkCore;
using Temporal.MigrationTest.Models;

namespace Temporal.MigrationTest
{
    public partial class MigrationTest
    {
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
                modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

                modelBuilder.Entity<User>(b =>
                {
                    b.ToTable("Users");
                    //b.HasTemporalTable();

                    //b.HasTemporalTable(config =>
                    //{
                    //    config.StartDateColumn("DateFrom");
                    //});
                });

                //modelBuilder.Entity<TransactionRecord>(b =>
                //{
                //    //b.HasTemporalTable();

                //    b.HasTemporalTable(config =>
                //    {
                //        config.StartDateColumn("ValidFrom");
                //        config.EndDateColumn("NewValidTo");
                //        config.DataConsistencyCheck(true);
                //    });
                //});

                modelBuilder.Entity<Subscription>(e =>
                {
                    e.HasTemporalTable();
                });
            }
        }
    }
}
