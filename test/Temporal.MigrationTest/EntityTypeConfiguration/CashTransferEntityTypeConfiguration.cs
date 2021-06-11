using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Temporal.MigrationTest.Models;

namespace Temporal.MigrationTest.EntityTypeConfiguration
{
    class CashTransferEntityTypeConfiguration : IEntityTypeConfiguration<CashTransfer>
    {
        public void Configure(EntityTypeBuilder<CashTransfer> builder)
        {
            builder.HasTemporalTable();
            builder.ToTable("CashTransfers");
        }
    }
}
