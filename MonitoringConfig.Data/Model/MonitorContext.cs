﻿using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Shared.Data;
using Microsoft.EntityFrameworkCore.Design;
namespace MonitoringConfig.Data.Model; 

public class MonitorContext:DbContext {
    public DbSet<Device> Devices { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<AlertLevel> AlertLevels { get; set; }
    public DbSet<FacilityAction> FacilityActions { get; set; }
    public DbSet<DeviceAction> DeviceActions { get; set; }
    public DbSet<ActionOutput> ActionOutputs { get; set; }
    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<NetworkConfiguration> NetworkConfigurations { get; set; }
    public DbSet<ModbusConfiguration> ModbusConfigurations { get; set; }
    public DbSet<ModbusChannelRegisterMap> ModbusChannelRegisterMaps { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<BoxModule> BoxModules { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer("server=172.20.4.20;database=facilitymodel_dev;" +
                                    "User Id=aelmendorf;Password=Drizzle123!;");
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<ModbusDevice>().HasBaseType<Device>();
        builder.Entity<MonitorBox>().HasBaseType<ModbusDevice>();
        
        builder.Entity<InputChannel>().HasBaseType<Channel>();
        builder.Entity<OutputChannel>().HasBaseType<Channel>();
        builder.Entity<DiscreteInput>().HasBaseType<InputChannel>();
        builder.Entity<AnalogInput>().HasBaseType<InputChannel>();
        builder.Entity<VirtualInput>().HasBaseType<InputChannel>();
        builder.Entity<DiscreteOutput>().HasBaseType<OutputChannel>();

        builder.Entity<DiscreteAlert>().HasBaseType<Alert>();
        builder.Entity<AnalogAlert>().HasBaseType<Alert>();

        builder.Entity<DiscreteLevel>().HasBaseType<AlertLevel>();
        builder.Entity<AnalogLevel>().HasBaseType<AlertLevel>();

        builder.Entity<Device>().HasKey(e => e.Id);
        builder.Entity<Channel>().HasKey(e => e.Id);
        builder.Entity<FacilityAction>().HasKey(e=>e.Id);
        builder.Entity<DeviceAction>().HasKey(e=>e.Id);
        builder.Entity<ActionOutput>().HasKey(e => e.Id);
        builder.Entity<Sensor>().HasKey(e=>e.Id);
        builder.Entity<NetworkConfiguration>().HasKey(e => e.Id);
        builder.Entity<ModbusConfiguration>().HasKey(e=>e.Id);
        builder.Entity<ModbusChannelRegisterMap>().HasKey(e=>e.Id);
        builder.Entity<Alert>().HasKey(e => e.Id);
        builder.Entity<AlertLevel>().HasKey(e=>e.Id);
        builder.Entity<Module>().HasKey(e => e.Id);
        builder.Entity<BoxModule>().HasKey(e => e.Id);

        builder.Entity<Channel>()
            .OwnsOne(e => e.ChannelAddress);

        builder.Entity<Channel>()
            .OwnsOne(e => e.ModbusAddress);

        builder.Entity<Alert>()
            .OwnsOne(e => e.ModbusAddress);
        
        builder.Entity<ModbusDevice>()
            .HasMany(e => e.Channels)
            .WithOne(e => e.ModbusDevice)
            .HasForeignKey(e => e.ModbusDeviceId)
            .IsRequired(true);
        
        builder.Entity<ModbusDevice>()
            .HasOne(e => e.NetworkConfiguration)
            .WithOne(e => e.ModbusDevice)
            .HasForeignKey<NetworkConfiguration>(e => e.ModbusDeviceId);

        builder.Entity<ModbusDevice>()
            .HasOne(e => e.ModbusConfiguration)
            .WithOne(e => e.ModbusDevice)
            .HasForeignKey<ModbusConfiguration>(e => e.ModbusDeviceId);

        builder.Entity<ModbusDevice>()
            .HasOne(e => e.ChannelRegisterMap)
            .WithOne(e => e.ModbusDevice)
            .HasForeignKey<ModbusChannelRegisterMap>(e => e.ModbusDeviceId);

        builder.Entity<ModbusDevice>()
            .HasMany(e => e.DeviceActions)
            .WithOne(e => e.ModbusDevice)
            .HasForeignKey(e => e.ModbusDeviceId);

        builder.Entity<BoxModule>()
            .HasOne(e => e.MonitorBox)
            .WithMany(e => e.BoxModules)
            .HasForeignKey(e => e.MonitorBoxId);

        builder.Entity<BoxModule>()
            .HasOne(e => e.Module)
            .WithMany(e => e.BoxModules)
            .HasForeignKey(e => e.ModuleId);



        /*builder.Entity<MonitorBox>()
            .HasMany(e=>e.Modules)
            .WithMany(e=>e.MoitorBoxes)
            .UsingEntity<MonitorBoxModule>(
                j=>j
                    .HasOne(bm=>bm.Module)
                    .WithMany(m=>m.MonitorBoxModules)
                    .HasForeignKey(e=>e.ModuleId),
                j=>j
                    .HasOne(bm=>bm.ModuleBox)
                    .WithMany(m=>m.MonitorBoxModules)
                    .HasForeignKey(bm=>bm.MonitorBoxId),
                j =>
                {
                    j.HasKey(bm => new { bm.ModuleId, bm.MonitorBoxId });
                });*/
        
        //Channel Configuration
        
        builder.Entity<Channel>()
            .HasOne(e => e.BoxModule)
            .WithMany(e => e.Channels)
            .HasForeignKey(e => e.BoxModuleId)
            .IsRequired(false);
        
        builder.Entity<InputChannel>()
            .HasOne(e => e.Alert)
            .WithOne(e => e.InputChannel)
            .HasForeignKey<Alert>(e => e.InputChannelId);

        builder.Entity<AnalogInput>()
            .HasOne(e => e.Sensor)
            .WithMany(e => e.AnalogInputs)
            .HasForeignKey(e => e.SensorId);

        builder.Entity<DiscreteOutput>()
            .HasMany(e => e.ActionOutputs)
            .WithOne(e => e.DiscreteOutput)
            .HasForeignKey(e => e.DiscreteOutputId)
            .IsRequired(false);
        
        //Alert Configuration
        
        builder.Entity<AnalogAlert>()
            .HasMany(e => e.AlertLevels)
            .WithOne(e => e.AnalogAlert)
            .HasForeignKey(e => e.AnalogAlertId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DiscreteAlert>()
            .HasOne(e => e.AlertLevel)
            .WithOne(e =>e.DiscreteAlert)
            .HasForeignKey<DiscreteLevel>(e =>e.DiscreteAlertId)
            .OnDelete(DeleteBehavior.NoAction);
        
        //Action & AlertLevel Configuration
        builder.Entity<DeviceAction>()
            .HasMany(e => e.AlertLevels)
            .WithOne(e => e.DeviceAction)
            .HasForeignKey(e => e.DeviceActionId);

        builder.Entity<DeviceAction>()
            .HasOne(e => e.FacilityAction)
            .WithMany(e => e.DeviceActions)
            .HasForeignKey(e => e.FacilityActionId);

        builder.Entity<DeviceAction>()
            .HasMany(e => e.ActionOutputs)
            .WithOne(e => e.DeviceAction)
            .HasForeignKey(e => e.DeviceActionId)
            .OnDelete(DeleteBehavior.NoAction);
        
    }
    
}