﻿// <auto-generated />
using System;
using MonitoringConfig.Infrastructure.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FacilityMonitoring.Infrastructure.Migrations
{
    [DbContext(typeof(FacilityContext))]
    [Migration("20211208193325_addMongoDbid")]
    partial class addMongoDbid
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ChannelFacilityZone", b =>
                {
                    b.Property<int>("ChannelsId")
                        .HasColumnType("int");

                    b.Property<int>("ZonesId")
                        .HasColumnType("int");

                    b.HasKey("ChannelsId", "ZonesId");

                    b.HasIndex("ZonesId");

                    b.ToTable("ChannelZones", (string)null);
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Alert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Bypass")
                        .HasColumnType("bit");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<int?>("FacilityActionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FacilityActionId");

                    b.ToTable("Alerts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Alert");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Connected")
                        .HasColumnType("bit");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ModbusDeviceId")
                        .HasColumnType("int");

                    b.Property<int>("MongoDBid")
                        .HasColumnType("int");

                    b.Property<int>("SystemChannel")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ModbusDeviceId");

                    b.ToTable("Channels");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Channel");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ActionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ActionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("FacilityActions");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<string>("ZoneName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ZoneParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZoneParentId");

                    b.ToTable("FacilityZones");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("BypassAlarms")
                        .HasColumnType("bit");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ReadInterval")
                        .HasColumnType("float");

                    b.Property<double>("SaveInterval")
                        .HasColumnType("float");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ModbusDevices");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ModbusDevice");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ChannelCount")
                        .HasColumnType("int");

                    b.Property<int>("ModuleChannel")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Slot")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Modules");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ChannelCount = 16,
                            ModuleChannel = 1,
                            Name = "P1-16ND3",
                            Slot = 1
                        },
                        new
                        {
                            Id = 2,
                            ChannelCount = 16,
                            ModuleChannel = 1,
                            Name = "P1-16ND3",
                            Slot = 2
                        },
                        new
                        {
                            Id = 3,
                            ChannelCount = 8,
                            ModuleChannel = 0,
                            Name = "P1-08ADL-1",
                            Slot = 3
                        },
                        new
                        {
                            Id = 4,
                            ChannelCount = 8,
                            ModuleChannel = 0,
                            Name = "P1-08ADL-1",
                            Slot = 4
                        },
                        new
                        {
                            Id = 5,
                            ChannelCount = 8,
                            ModuleChannel = 2,
                            Name = "P1-08TD2",
                            Slot = 5
                        });
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Factor")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Offset")
                        .HasColumnType("float");

                    b.Property<double>("Slope")
                        .HasColumnType("float");

                    b.Property<string>("Units")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sensors");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "H2 Gas Detector",
                            Factor = 1.0,
                            Name = "H2 Detector-PPM",
                            Offset = -250.0,
                            Slope = 62.5,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 2,
                            Description = "O2 Gas Detector",
                            DisplayName = "O2",
                            Factor = 1.0,
                            Name = "O2 Detector",
                            Offset = -18.75,
                            Slope = 4.6900000000000004,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 3,
                            Description = "NH3 Gas Detector",
                            DisplayName = "NH3",
                            Factor = 1.0,
                            Name = "NH3 Detector",
                            Offset = -6.25,
                            Slope = 1.5600000000000001,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 4,
                            Description = "N2 Gas Detector",
                            DisplayName = "N2",
                            Factor = 1.0,
                            Name = "N2 Detector",
                            Offset = -140.0,
                            Slope = 5.0,
                            Units = "PPM"
                        },
                        new
                        {
                            Id = 5,
                            Description = "H2 Explosion Gas Detector",
                            DisplayName = "H2-LEL",
                            Factor = 1.0,
                            Name = "H2 LEL Detector",
                            Offset = -25.0,
                            Slope = 6.25,
                            Units = "LEL"
                        });
                });

            modelBuilder.Entity("FacilityZoneModbusDevice", b =>
                {
                    b.Property<int>("ModbusDevicesId")
                        .HasColumnType("int");

                    b.Property<int>("ZonesId")
                        .HasColumnType("int");

                    b.HasKey("ModbusDevicesId", "ZonesId");

                    b.HasIndex("ZonesId");

                    b.ToTable("DeviceZones", (string)null);
                });

            modelBuilder.Entity("ModuleMonitoringBox", b =>
                {
                    b.Property<int>("ModulesId")
                        .HasColumnType("int");

                    b.Property<int>("MonitoringBoxesId")
                        .HasColumnType("int");

                    b.HasKey("ModulesId", "MonitoringBoxesId");

                    b.HasIndex("MonitoringBoxesId");

                    b.ToTable("BoxModules", (string)null);
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.AnalogAlert", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.Alert");

                    b.Property<int?>("AnalogInputId")
                        .HasColumnType("int");

                    b.Property<double>("SetPoint")
                        .HasColumnType("float");

                    b.HasIndex("AnalogInputId");

                    b.HasDiscriminator().HasValue("AnalogAlert");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.Channel");

                    b.Property<int?>("SensorId")
                        .HasColumnType("int");

                    b.HasIndex("SensorId");

                    b.HasDiscriminator().HasValue("AnalogInput");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.DiscreteAlert", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.Alert");

                    b.Property<int?>("DiscreteInputId")
                        .HasColumnType("int");

                    b.Property<int>("TriggerOn")
                        .HasColumnType("int");

                    b.HasIndex("DiscreteInputId")
                        .IsUnique()
                        .HasFilter("[DiscreteInputId] IS NOT NULL");

                    b.HasDiscriminator().HasValue("DiscreteAlert");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.DiscreteInput", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.Channel");

                    b.HasDiscriminator().HasValue("DiscreteInput");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.DiscreteOutput", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.Channel");

                    b.Property<int>("ChannelState")
                        .HasColumnType("int");

                    b.Property<int>("StartState")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("DiscreteOutput");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.MonitoringBox", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice");

                    b.HasDiscriminator().HasValue("MonitoringBox");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.VirtualInput", b =>
                {
                    b.HasBaseType("MonitoringConfig.Infrastructure.Data.Model.DiscreteInput");

                    b.HasDiscriminator().HasValue("VirtualInput");
                });

            modelBuilder.Entity("ChannelFacilityZone", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.Channel", null)
                        .WithMany()
                        .HasForeignKey("ChannelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", null)
                        .WithMany()
                        .HasForeignKey("ZonesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Alert", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.FacilityAction", "FacilityAction")
                        .WithMany("Alerts")
                        .HasForeignKey("FacilityActionId");

                    b.Navigation("FacilityAction");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Channel", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice", "ModbusDevice")
                        .WithMany("Channels")
                        .HasForeignKey("ModbusDeviceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.ChannelAddress", "ChannelAddress", b1 =>
                        {
                            b1.Property<int>("ChannelId")
                                .HasColumnType("int");

                            b1.Property<int>("Channel")
                                .HasColumnType("int");

                            b1.Property<int>("ModuleSlot")
                                .HasColumnType("int");

                            b1.HasKey("ChannelId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("ChannelId");
                        });

                    b.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.ModbusAddress", "ModbusAddress", b1 =>
                        {
                            b1.Property<int>("ChannelId")
                                .HasColumnType("int");

                            b1.Property<int>("Address")
                                .HasColumnType("int");

                            b1.Property<int>("RegisterLength")
                                .HasColumnType("int");

                            b1.HasKey("ChannelId");

                            b1.ToTable("Channels");

                            b1.WithOwner()
                                .HasForeignKey("ChannelId");
                        });

                    b.Navigation("ChannelAddress");

                    b.Navigation("ModbusAddress");

                    b.Navigation("ModbusDevice");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.OwnsMany("MonitoringConfig.Infrastructure.Data.Model.ActionOutput", "ActionOutputs", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<int>("OffLevel")
                                .HasColumnType("int");

                            b1.Property<int>("OnLevel")
                                .HasColumnType("int");

                            b1.Property<int?>("OutputId")
                                .HasColumnType("int");

                            b1.Property<int>("OwnerId")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("OutputId");

                            b1.HasIndex("OwnerId");

                            b1.ToTable("ActionOutput");

                            b1.HasOne("MonitoringConfig.Infrastructure.Data.Model.DiscreteOutput", "Output")
                                .WithMany()
                                .HasForeignKey("OutputId");

                            b1.WithOwner()
                                .HasForeignKey("OwnerId");

                            b1.Navigation("Output");
                        });

                    b.Navigation("ActionOutputs");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", "ZoneParent")
                        .WithMany("SubZones")
                        .HasForeignKey("ZoneParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.Location", "Location", b1 =>
                        {
                            b1.Property<int>("FacilityZoneId")
                                .HasColumnType("int");

                            b1.Property<double>("XCoord")
                                .HasColumnType("float");

                            b1.Property<double>("YCoord")
                                .HasColumnType("float");

                            b1.HasKey("FacilityZoneId");

                            b1.ToTable("FacilityZones");

                            b1.WithOwner()
                                .HasForeignKey("FacilityZoneId");
                        });

                    b.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.ZoneSize", "ZoneSize", b1 =>
                        {
                            b1.Property<int>("FacilityZoneId")
                                .HasColumnType("int");

                            b1.Property<double>("Height")
                                .HasColumnType("float");

                            b1.Property<double>("Width")
                                .HasColumnType("float");

                            b1.HasKey("FacilityZoneId");

                            b1.ToTable("FacilityZones");

                            b1.WithOwner()
                                .HasForeignKey("FacilityZoneId");
                        });

                    b.Navigation("Location");

                    b.Navigation("ZoneParent");

                    b.Navigation("ZoneSize");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.NetworkConfiguration", "NetworkConfiguration", b1 =>
                        {
                            b1.Property<int>("ModbusDeviceId")
                                .HasColumnType("int");

                            b1.Property<string>("DNS")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Gateway")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("IPAddress")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("MAC")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Port")
                                .HasColumnType("int");

                            b1.Property<int>("SlaveAddress")
                                .HasColumnType("int");

                            b1.HasKey("ModbusDeviceId");

                            b1.ToTable("ModbusDevices");

                            b1.WithOwner()
                                .HasForeignKey("ModbusDeviceId");

                            b1.OwnsOne("MonitoringConfig.Infrastructure.Data.Model.ModbusConfig", "ModbusConfig", b2 =>
                                {
                                    b2.Property<int>("NetworkConfigurationModbusDeviceId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Coils")
                                        .HasColumnType("int");

                                    b2.Property<int>("DiscreteInputs")
                                        .HasColumnType("int");

                                    b2.Property<int>("InputRegisters")
                                        .HasColumnType("int");

                                    b2.HasKey("NetworkConfigurationModbusDeviceId");

                                    b2.ToTable("ModbusDevices");

                                    b2.WithOwner()
                                        .HasForeignKey("NetworkConfigurationModbusDeviceId");
                                });

                            b1.Navigation("ModbusConfig");
                        });

                    b.Navigation("NetworkConfiguration");
                });

            modelBuilder.Entity("FacilityZoneModbusDevice", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice", null)
                        .WithMany()
                        .HasForeignKey("ModbusDevicesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", null)
                        .WithMany()
                        .HasForeignKey("ZonesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModuleMonitoringBox", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.Module", null)
                        .WithMany()
                        .HasForeignKey("ModulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.MonitoringBox", null)
                        .WithMany()
                        .HasForeignKey("MonitoringBoxesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.AnalogAlert", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.AnalogInput", "AnalogInput")
                        .WithMany("AnalogAlerts")
                        .HasForeignKey("AnalogInputId");

                    b.Navigation("AnalogInput");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.Sensor", "Sensor")
                        .WithMany("AnalogInputs")
                        .HasForeignKey("SensorId");

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.DiscreteAlert", b =>
                {
                    b.HasOne("MonitoringConfig.Infrastructure.Data.Model.DiscreteInput", "DiscreteInput")
                        .WithOne("DiscreteAlert")
                        .HasForeignKey("MonitoringConfig.Infrastructure.Data.Model.DiscreteAlert", "DiscreteInputId");

                    b.Navigation("DiscreteInput");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityAction", b =>
                {
                    b.Navigation("Alerts");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.FacilityZone", b =>
                {
                    b.Navigation("SubZones");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.ModbusDevice", b =>
                {
                    b.Navigation("Channels");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.Sensor", b =>
                {
                    b.Navigation("AnalogInputs");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.AnalogInput", b =>
                {
                    b.Navigation("AnalogAlerts");
                });

            modelBuilder.Entity("MonitoringConfig.Infrastructure.Data.Model.DiscreteInput", b =>
                {
                    b.Navigation("DiscreteAlert")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
