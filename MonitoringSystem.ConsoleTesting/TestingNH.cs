using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringConfig.Infrastructure.Data;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringSystem.ConsoleTesting {
    public class TestingNH {
        public async Task Main(string args[]) {
            using var context = new FacilityContext();

            var device = new ModbusDevice();
            device.Identifier = "nh3";
            device.DisplayName = "nh3";
            var netConfig = new NetworkConfiguration();
            netConfig.IPAddress = "";
            netConfig.Port = 502;
            var modbusConfig=new ModbusConfig();
            modbusConfig.SlaveAddress = 0;
            modbusConfig.Coils = 10;
            modbusConfig.HoldingRegisters = 70;
            modbusConfig.InputRegisters = 0;
            modbusConfig.DiscreteInputs = 0;
            //modbusConfig.ChannelMapping

            AnalogInput input = new AnalogInput();
            ModbusAddress address = new ModbusAddress();
            address.RegisterType = ModbusRegister.Holding;


        }

    }
}
