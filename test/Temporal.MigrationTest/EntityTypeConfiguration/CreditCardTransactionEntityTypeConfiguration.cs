using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Temporal.MigrationTest.Models;

namespace Temporal.MigrationTest.EntityTypeConfiguration
{
    class CreditCardTransactionEntityTypeConfiguration : IEntityTypeConfiguration<CreditCardTransaction>
    {
        public void Configure(EntityTypeBuilder<CreditCardTransaction> builder)
        {
            builder.HasTemporalTable();
            builder.ToTable("CreditCardTransactions");
        }
    }
}
