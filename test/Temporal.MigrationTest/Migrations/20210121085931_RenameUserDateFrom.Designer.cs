﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Temporal.MigrationTest;

namespace Temporal.MigrationTest.Migrations
{
    [DbContext(typeof(MigrationTest.TemporalTestDbContext))]
    [Migration("20210121085931_RenameUserDateFrom")]
    partial class RenameUserDateFrom
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

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
                        .HasColumnName("NewValidTo")
                        .HasAnnotation("Relational:SysEndDate", true);

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("ValidFrom")
                        .HasAnnotation("Relational:SysStartDate", true);

                    b.HasKey("Id");

                    b.ToTable("TransactionRecord");

                    b
                        .HasAnnotation("dbo", "dbo")
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

                    b.Property<DateTime>("SysEndTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime")
                        .HasAnnotation("Relational:SysEndDate", true);

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("DateFrom")
                        .HasAnnotation("Relational:SysStartDate", true);

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b
                        .HasAnnotation("dbo", "dbo")
                        .HasAnnotation("Relational:IsTemporal", true);
                });
#pragma warning restore 612, 618
        }
    }
}
