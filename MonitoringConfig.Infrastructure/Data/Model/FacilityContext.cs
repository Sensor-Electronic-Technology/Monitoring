using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Infrastructure.Data.Model {
    public class FacilityContext:DbContext {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<FacilityAction> FacilityActions { get; set; }
        public DbSet<FacilityZone> FacilityZones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=MonitorModelTesting;" +
              "User Id=aelmendorf;Password=Drizzle123!;");
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            //Modbus Device inheritance
            builder.Entity<BnetDevice>().HasBaseType<Device>();
            builder.Entity<ApiDevice>().HasBaseType<Device>();
            builder.Entity<ModbusDevice>().HasBaseType<Device>();
            builder.Entity<MonitoringBox>().HasBaseType<ModbusDevice>();
            //Channel inheritance
            builder.Entity<InputChannel>().HasBaseType<Channel>();
            builder.Entity<OutputChannel>().HasBaseType<Channel>();
            builder.Entity<DiscreteInput>().HasBaseType<InputChannel>();
            builder.Entity<AnalogInput>().HasBaseType<InputChannel>();
            builder.Entity<VirtualInput>().HasBaseType<InputChannel>();
            builder.Entity<DiscreteOutput>().HasBaseType<OutputChannel>();
            //Alert inheritance
            builder.Entity<DiscreteAlert>().HasBaseType<Alert>();
            builder.Entity<AnalogAlert>().HasBaseType<Alert>();
            //AlertLevel Inheritance
            builder.Entity<DiscreteLevel>().HasBaseType<AlertLevel>();
            builder.Entity<AnalogLevel>().HasBaseType<AlertLevel>();

            //builder.Owned<ModbusAddress>();

            builder.Entity<Channel>()
                .OwnsOne(p => p.ChannelAddress);

            builder.Entity<Channel>()
                .OwnsOne(p => p.ModbusAddress);

            builder.Entity<ModbusActionMap>()
                .OwnsOne(e => e.ModbusAddress);

            builder.Entity<FacilityAction>()
                .OwnsMany(e => e.ActionOutputs);

            builder.Entity<Alert>()
                .OwnsOne(e => e.ModbusAddress);

            builder.Entity<ModbusDevice>()
                .OwnsOne(p => p.NetworkConfiguration)
                .OwnsOne(p => p.ModbusConfig, (p) => {
                    p.OwnsOne(e => e.ChannelMapping);
                    p.OwnsOne(e => e.ModbusAddress);
                });

            builder.Entity<FacilityAction>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Entity<FacilityAction>()
                .OwnsMany(p => p.ActionOutputs, a => {
                    a.WithOwner().HasForeignKey("OwnerId");
                    a.Property<int>("Id");
                    a.HasKey("Id");
                });

            //builder.Entity<MonitoringBox>()
            //    .Property(e => e.DataConfigIteration)
            //    .HasDefaultValue(0);

            builder.Entity<FacilityZone>()
                .OwnsOne(p => p.ZoneSize);

            builder.Entity<FacilityZone>()
                .OwnsOne(p => p.Location);

            builder.Entity<ModbusDevice>()
                .HasMany(p => p.Channels)
                .WithOne(p => p.ModbusDevice)
                .HasForeignKey(p => p.ModbusDeviceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MonitoringBox>()
                .HasMany(p => p.Modules)
                .WithMany(p => p.MonitoringBoxes)
                .UsingEntity(j => j.ToTable("BoxModules"));

            builder.Entity<InputChannel>()
                .HasOne(e => e.Alert)
                .WithOne(e => e.InputChannel)
                .HasForeignKey<Alert>(p => p.InputChannelId)
                .IsRequired(false);

            builder.Entity<AnalogInput>()
                .HasOne(p => p.Sensor)
                .WithMany(p => p.AnalogInputs)
                .HasForeignKey(p => p.SensorId)
                .IsRequired(false);

            builder.Entity<Sensor>()
                .HasMany(p => p.AnalogInputs)
                .WithOne(p => p.Sensor)
                .HasForeignKey(p => p.SensorId)
                .IsRequired(false);

            builder.Entity<FacilityAction>()
                .HasMany(p => p.AlertLevels)
                .WithOne(p => p.FacilityAction)
                .HasForeignKey(p => p.FacilityActionId)
                .IsRequired(false);

            builder.Entity<DiscreteAlert>()
                .HasOne(e => e.AlertLevel)
                .WithOne(e => e.DiscreteAlert)
                .HasForeignKey<DiscreteLevel>(p => p.DiscrteAlertId)
                .IsRequired(false);

            builder.Entity<MonitoringBox>()
                .HasMany(e => e.ModbusActionMapping)
                .WithOne(e => e.MonitoringBox)
                .HasForeignKey(e => e.MonitoringBoxId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FacilityAction>()
                .HasMany(e => e.ModbusActionMapping)
                .WithOne(e => e.FacilityAction)
                .HasForeignKey(e => e.FacilityActionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AnalogAlert>()
                .HasMany(e => e.AlertLevels)
                .WithOne(e => e.AnalogAlert)
                .HasForeignKey(e => e.AnalogAlertId)
                .IsRequired(false);

            builder.Entity<Device>()
                .HasMany(p => p.Zones)
                .WithMany(p => p.Devices)
                .UsingEntity(j => j.ToTable("DeviceZones"));

            builder.Entity<Channel>()
                .HasMany(p => p.Zones)
                .WithMany(p => p.Channels)
                .UsingEntity(j => j.ToTable("ChannelZones"));

            builder.Entity<FacilityZone>()
                .HasOne(p => p.ZoneParent)
                .WithMany(p => p.SubZones)
                .HasForeignKey(p => p.ZoneParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            SeedSensors(builder);
            SeedModules(builder);
            SeedFacilityActions(builder);
        }

            private void SeedSensors(ModelBuilder builder) {
            Sensor h2 = new Sensor();
            h2.Id = 1;
            h2.Name = "H2 Detector-PPM";
            h2.Description = "H2 Gas Detector";
            h2.Slope = 62.5;
            h2.Offset = -250;
            h2.Factor = 1;
            h2.Units = "PPM";

            Sensor o2 = new Sensor();
            o2.Id = 2;
            o2.Name = "O2 Detector";
            o2.DisplayName = "O2";
            o2.Description = "O2 Gas Detector";
            o2.Slope = 4.69;
            o2.Offset = -18.75;
            o2.Factor = 1;
            o2.Units = "PPM";

            Sensor nh3 = new Sensor();
            nh3.Id = 3;
            nh3.Name = "NH3 Detector";
            nh3.DisplayName = "NH3";
            nh3.Description = "NH3 Gas Detector";
            nh3.Slope = 1.56;
            nh3.Offset = -6.25;
            nh3.Factor = 1;
            nh3.Units = "PPM";

            Sensor n2 = new Sensor();
            n2.Id = 4;
            n2.Name = "N2 Detector";
            n2.DisplayName = "N2";
            n2.Description = "N2 Gas Detector";
            n2.Slope = 5.00;
            n2.Offset = -140;
            n2.Factor = 1;
            n2.Units = "PPM";

            Sensor h2_lel = new Sensor();
            h2_lel.Id = 5;
            h2_lel.Name = "H2 LEL Detector";
            h2_lel.DisplayName = "H2-LEL";
            h2_lel.Description = "H2 Explosion Gas Detector";
            h2_lel.Slope = 6.25;
            h2_lel.Offset = -25;
            h2_lel.Factor = 1;
            h2_lel.Units = "LEL";

            builder.Entity<Sensor>().HasData(h2, o2, nh3, n2, h2_lel);

        }

        private void SeedModules(ModelBuilder builder) {
            Module dModule1 = new Module();
            dModule1.Id = 1;
            dModule1.Name = "P1-16ND3";
            dModule1.Slot = 1;
            dModule1.ChannelCount = 16;
            dModule1.ModuleChannel = ModuleChannel.DiscreteInput;

            Module dModule2 = new Module();
            dModule2.Id = 2;
            dModule2.Name = "P1-16ND3";
            dModule2.Slot = 2;
            dModule2.ChannelCount = 16;
            dModule2.ModuleChannel = ModuleChannel.DiscreteInput;

            Module aModule1 = new Module();
            aModule1.Id = 3;
            aModule1.Name = "P1-08ADL-1";
            aModule1.Slot = 3;
            aModule1.ChannelCount = 8;
            aModule1.ModuleChannel = ModuleChannel.AnalogInput;

            Module aModule2 = new Module();
            aModule2.Id = 4;
            aModule2.Name = "P1-08ADL-1";
            aModule2.Slot = 4;
            aModule2.ChannelCount = 8;
            aModule2.ModuleChannel = ModuleChannel.AnalogInput;

            Module oModule = new Module();
            oModule.Id = 5;
            oModule.Name = "P1-08TD2";
            oModule.Slot = 5;
            oModule.ChannelCount = 8;
            oModule.ModuleChannel = ModuleChannel.DiscreteOutput;

            builder.Entity<Module>().HasData(dModule1, dModule2, aModule1, aModule2, oModule);
        }

        private void SeedFacilityActions(ModelBuilder builder) {
            FacilityAction action1 = new FacilityAction();
            action1.ActionName = "Okay";
            action1.ActionType = ActionType.Okay;
            action1.Id = 1;

            FacilityAction action2 = new FacilityAction();
            action2.ActionName = "Alarm";
            action2.ActionType = ActionType.Alarm;
            action2.Id = 2;

            FacilityAction action3 = new FacilityAction();
            action3.ActionName = "Warning";
            action3.ActionType = ActionType.Warning;
            action3.Id = 3;

            FacilityAction action4 = new FacilityAction();
            action4.ActionName = "SoftWarn";
            action4.ActionType = ActionType.SoftWarn;
            action4.Id = 4;

            FacilityAction action5 = new FacilityAction();
            action5.ActionName = "Maintenance";
            action5.ActionType = ActionType.Maintenance;
            action5.Id = 5;

            FacilityAction action6 = new FacilityAction();
            action6.ActionName = "ResetWetFloor";
            action6.ActionType = ActionType.Custom;
            action6.Id = 6;

            builder.Entity<FacilityAction>().HasData(action1, action2, action3, action4, action5, action6);
        }
    }

    public class FacilityContextFactory : IDesignTimeDbContextFactory<FacilityContext> {
        public FacilityContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<FacilityContext>();
            return new FacilityContext();
        }
    }
}
