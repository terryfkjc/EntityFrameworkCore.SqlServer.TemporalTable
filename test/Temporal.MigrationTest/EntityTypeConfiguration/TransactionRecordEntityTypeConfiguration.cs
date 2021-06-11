using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Temporal.MigrationTest.Models;

namespace Temporal.MigrationTest.EntityTypeConfiguration
{
    class TransactionRecordEntityTypeConfiguration : IEntityTypeConfiguration<TransactionRecord>
    {
        public void Configure(EntityTypeBuilder<TransactionRecord> builder)
        {
            builder.HasTemporalTable();
            builder.ToTable("Transactions");
        }
    }
}
