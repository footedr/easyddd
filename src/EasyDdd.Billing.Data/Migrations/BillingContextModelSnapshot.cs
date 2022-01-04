﻿// <auto-generated />
using System;
using EasyDdd.Billing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EasyDdd.Billing.Data.Migrations
{
    [DbContext(typeof(BillingContext))]
    partial class BillingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.HasSequence("StatementIdentifiers", "billing")
                .StartsAt(10000L);

            modelBuilder.Entity("EasyDdd.Billing.Core.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("TotalCost")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("Identifier")
                        .IsUnique();

                    b.HasIndex("Status");

                    b.ToTable("Shipments", "billing");
                });

            modelBuilder.Entity("EasyDdd.Billing.Core.Statement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BillToAccount")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BillToLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CustomerCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("ProcessedAt")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("BillToAccount");

                    b.HasIndex("BillToLocation");

                    b.HasIndex("CustomerCode");

                    b.HasIndex("Identifier");

                    b.HasIndex("ProcessedAt");

                    b.ToTable("Statements", "billing");
                });

            modelBuilder.Entity("EasyDdd.Billing.Core.Shipment", b =>
                {
                    b.OwnsOne("EasyDdd.Billing.Core.Carrier", "Carrier", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("int");

                            b1.Property<string>("Code")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments", "billing");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsOne("EasyDdd.Billing.Core.DispatchInfo", "DispatchInfo", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DispatchDateTime")
                                .HasColumnType("datetime2");

                            b1.Property<string>("DispatchNumber")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PickupNote")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PickupNumber")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ReferenceNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments", "billing");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsOne("EasyDdd.Billing.Core.Address", "Consignee", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Line1")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("StateAbbreviation")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments", "billing");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsOne("EasyDdd.Billing.Core.Address", "Shipper", b1 =>
                        {
                            b1.Property<int>("ShipmentId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Line1")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("StateAbbreviation")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("Shipments", "billing");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.OwnsMany("EasyDdd.Billing.Core.ShipmentDetail", "Details", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<string>("Class")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("HandlingUnitCount")
                                .HasColumnType("int");

                            b1.Property<bool>("IsHazardous")
                                .HasColumnType("bit");

                            b1.Property<int>("ShipmentId")
                                .HasColumnType("int");

                            b1.Property<int>("Weight")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("ShipmentId");

                            b1.ToTable("ShipmentDetails", "billing");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId");
                        });

                    b.Navigation("Carrier");

                    b.Navigation("Consignee")
                        .IsRequired();

                    b.Navigation("Details");

                    b.Navigation("DispatchInfo");

                    b.Navigation("Shipper")
                        .IsRequired();
                });

            modelBuilder.Entity("EasyDdd.Billing.Core.Statement", b =>
                {
                    b.OwnsOne("EasyDdd.Billing.Core.BillingPeriod", "BillingPeriod", b1 =>
                        {
                            b1.Property<int>("StatementId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("End")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("Start")
                                .HasColumnType("datetime2");

                            b1.HasKey("StatementId");

                            b1.ToTable("Statements", "billing");

                            b1.WithOwner()
                                .HasForeignKey("StatementId");
                        });

                    b.OwnsMany("EasyDdd.Billing.Core.StatementLine", "Lines", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(12,4)");

                            b1.Property<string>("Class")
                                .HasMaxLength(450)
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int?>("HandlingUnits")
                                .HasColumnType("int");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.Property<int>("StatementId")
                                .HasColumnType("int");

                            b1.Property<string>("TmsNumber")
                                .IsRequired()
                                .HasMaxLength(450)
                                .HasColumnType("nvarchar(450)");

                            b1.Property<DateTime>("TransactionDate")
                                .HasColumnType("datetime2");

                            b1.Property<double?>("Weight")
                                .HasColumnType("float");

                            b1.HasKey("Id");

                            b1.HasIndex("StatementId");

                            b1.ToTable("StatementLines", "billing");

                            b1.WithOwner()
                                .HasForeignKey("StatementId");
                        });

                    b.Navigation("BillingPeriod")
                        .IsRequired();

                    b.Navigation("Lines");
                });
#pragma warning restore 612, 618
        }
    }
}
