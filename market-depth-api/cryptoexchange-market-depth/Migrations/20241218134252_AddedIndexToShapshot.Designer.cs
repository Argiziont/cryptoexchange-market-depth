﻿// <auto-generated />
using System;
using CryptoexchangeMarketDepth.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoexchangeMarketDepth.Migrations
{
    [DbContext(typeof(OrderBookDbContext))]
    [Migration("20241218134252_AddedIndexToShapshot")]
    partial class AddedIndexToShapshot
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.Ask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Amount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderBookSnapshotId")
                        .HasColumnType("int");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderBookSnapshotId");

                    b.ToTable("Asks");
                });

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.Bid", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Amount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderBookSnapshotId")
                        .HasColumnType("int");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderBookSnapshotId");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.OrderBookSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AcquiredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("MarketSymbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Microtimestamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AcquiredAt")
                        .HasDatabaseName("IX_Snapshots_AcquiredAt");

                    b.ToTable("Snapshots");
                });

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.Ask", b =>
                {
                    b.HasOne("CryptoexchangeMarketDepth.Models.OrderBookSnapshot", "Snapshot")
                        .WithMany("Asks")
                        .HasForeignKey("OrderBookSnapshotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Snapshot");
                });

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.Bid", b =>
                {
                    b.HasOne("CryptoexchangeMarketDepth.Models.OrderBookSnapshot", "Snapshot")
                        .WithMany("Bids")
                        .HasForeignKey("OrderBookSnapshotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Snapshot");
                });

            modelBuilder.Entity("CryptoexchangeMarketDepth.Models.OrderBookSnapshot", b =>
                {
                    b.Navigation("Asks");

                    b.Navigation("Bids");
                });
#pragma warning restore 612, 618
        }
    }
}
