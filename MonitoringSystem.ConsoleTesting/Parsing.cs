using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Infrastructure.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ConsoleTesting {
    public class Parsing {
        static async Task Main(string[] args) {
            //ParseConfiguation();
            //await FixDiscreteNames("Epi1");
            //await FixAnalogNames("epi1");
            //await FixOutputNames("Epi1");
            await SetAlertNames("epi2");
        }
        static async Task SetAlertNames(string deviceName) {
            using var context = new FacilityContext();
            var monitoring = await context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefaultAsync(e => e.Identifier == deviceName);
            if (monitoring != null) {
                var channels = await context.Channels.OfType<InputChannel>().Include(e => e.Alert).ToListAsync();
                foreach (var ain in channels) {
                    ain.Alert.DisplayName = ain.DisplayName;
                }
                var alerts = channels.Select(e => e.Alert).ToList();
                context.UpdateRange(alerts);
                var ret = await context.SaveChangesAsync();
                if (ret > 0) {
                    Console.WriteLine("Alert names updated");
                } else {
                    Console.WriteLine("Save failed..");
                }
            } else {
                Console.WriteLine("Error: Could not find monitoring box");
            }
            Console.ReadKey();
        }

        static void ParseConfiguation() {
            Console.WriteLine("Creating ModbusDevice,Please wait..");
            //FacilityParser.CreateModbusDevices();
            //FacilityParser.CreateOutputs();
            //FacilityParser.CreateFacilityActions();
            //FacilityParser.CreateDiscreteInputs();
            //FacilityParser.CreateAnalogInputs();
            //FacilityParser.CreateVirtualInputs();
            Console.WriteLine("Press any key..");
            Console.ReadKey();
        }
        static async Task FixDiscreteNames(string box) {
            Console.WriteLine("Fixing Discrete Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<DiscreteInput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Discrete " + dIn.SystemChannel;
                if (string.IsNullOrEmpty(dIn.DisplayName)) {
                    dIn.DisplayName = dIn.Identifier;
                } else if (dIn.DisplayName == "Not Set") {
                    dIn.DisplayName = dIn.Identifier;
                }
            }
            context.UpdateRange(dInputs);
            var ret = await context.SaveChangesAsync();
            if (ret > 0) {
                Console.WriteLine("Changes saved");
            } else {
                Console.WriteLine("Save Failed");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
        static async Task FixAnalogNames(string box) {
            Console.WriteLine("Fixing Analog Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<AnalogInput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Analog " + dIn.SystemChannel;
                if (string.IsNullOrEmpty(dIn.DisplayName)) {
                    dIn.DisplayName = dIn.Identifier;
                } else if (dIn.DisplayName == "Not Set") {
                    dIn.DisplayName = dIn.Identifier;
                }
            }
            context.UpdateRange(dInputs);
            var ret = await context.SaveChangesAsync();
            if (ret > 0) {
                Console.WriteLine("Changes saved");
            } else {
                Console.WriteLine("Save Failed");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
        static async Task FixOutputNames(string box) {
            Console.WriteLine("Fixing Output Names, Please Wait....");
            using var context = new FacilityContext();
            var monitoring = context.Devices.OfType<MonitoringBox>()
                .Include(e => e.Channels)
                .FirstOrDefault(e => e.Identifier == box);
            var dInputs = monitoring.Channels.OfType<DiscreteOutput>().OrderBy(e => e.SystemChannel).ToList();
            foreach (var dIn in dInputs) {
                dIn.Identifier = "Analog " + dIn.SystemChannel;
                if (string.IsNullOrEmpty(dIn.DisplayName)) {
                    dIn.DisplayName = dIn.Identifier;
                } else if (dIn.DisplayName == "Not Set") {
                    dIn.DisplayName = dIn.Identifier;
                }
            }
            context.UpdateRange(dInputs);
            var ret = await context.SaveChangesAsync();
            if (ret > 0) {
                Console.WriteLine("Changes saved");
            } else {
                Console.WriteLine("Save Failed");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
