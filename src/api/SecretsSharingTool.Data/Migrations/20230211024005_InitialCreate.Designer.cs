﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecretsSharingTool.Data;

#nullable disable

namespace SecretsSharingtool.Data.Migrations
{
    [DbContext(typeof(AppUnitOfWork))]
    [Migration("20230211024005_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SecretsSharingTool.Core.Models.Secret", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("EncryptedMessage")
                        .HasColumnType("longblob");

                    b.Property<byte[]>("EncryptedSymmetricKey")
                        .HasColumnType("longblob");

                    b.Property<long>("ExpiryMinutes")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte[]>("Iv")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<int>("NumberOfFailedAccesses")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Secrets");
                });

            modelBuilder.Entity("SecretsSharingTool.Core.Models.SecretAccessAudit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ClientIpAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ClientUserAgent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("FailureReason")
                        .HasColumnType("int");

                    b.Property<Guid>("SecretId")
                        .HasColumnType("char(36)");

                    b.Property<DateTimeOffset>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });
#pragma warning restore 612, 618
        }
    }
}