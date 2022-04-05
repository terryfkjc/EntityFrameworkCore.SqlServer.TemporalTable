﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Temporal.MigrationTest;

namespace Temporal.MigrationTest.Migrations
{
    [DbContext(typeof(MigrationTest.TemporalTestDbContext))]
    partial class TemporalTestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Temporal.MigrationTest.Models.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime")
                        .HasAnnotation("Relational:SysEndDate", true);

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime")
                        .HasAnnotation("Relational:SysStartDate", true);

                    b.HasKey("Id");

                    b.ToTable("Subscription");

                    b
                        .HasAnnotation("Relational:HistorySchema", "dbo")
                        .HasAnnotation("Relational:IsTemporal", true);
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.TransactionRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime")
                        .HasAnnotation("Relational:SysEndDate", true);

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime")
                        .HasAnnotation("Relational:SysStartDate", true);

                    b.HasKey("Id");

                    b.ToTable("Transactions");

                    b
                        .HasAnnotation("Relational:HistorySchema", "dbo")
                        .HasAnnotation("Relational:IsTemporal", true);
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.CashTransfer", b =>
                {
                    b.HasBaseType("Temporal.MigrationTest.Models.TransactionRecord");

                    b.Property<string>("Receiver")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sender")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("CashTransfers");

                    b
                        .HasAnnotation("Relational:HistorySchema", "dbo")
                        .HasAnnotation("Relational:IsTemporal", true);
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.CreditCardTransaction", b =>
                {
                    b.HasBaseType("Temporal.MigrationTest.Models.TransactionRecord");

                    b.Property<string>("CardNumber")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("CreditCardTransactions");

                    b
                        .HasAnnotation("Relational:HistorySchema", "dbo")
                        .HasAnnotation("Relational:IsTemporal", true);
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.CashTransfer", b =>
                {
                    b.HasOne("Temporal.MigrationTest.Models.TransactionRecord", null)
                        .WithOne()
                        .HasForeignKey("Temporal.MigrationTest.Models.CashTransfer", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Temporal.MigrationTest.Models.CreditCardTransaction", b =>
                {
                    b.HasOne("Temporal.MigrationTest.Models.TransactionRecord", null)
                        .WithOne()
                        .HasForeignKey("Temporal.MigrationTest.Models.CreditCardTransaction", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
