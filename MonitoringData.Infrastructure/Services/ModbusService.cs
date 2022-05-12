using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Modbus.Device;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public class ModbusResult {
        public bool[] DiscreteInputs { get; set; }
        public bool[] Coils { get; set; }
        public ushort[] HoldingRegisters { get; set; }
        public ushort[] InputRegisters { get; set; }
        public bool _success { get; set; }
        public ModbusResult(bool success) {
            this._success = success;
        }

        public ModbusResult() {
            this._success = false;
        }
    }

    public interface IModbusService {
        Task<ModbusResult> Read(string ip, int port, ModbusConfig config);
        Task WriteCoil(string ip, int port, int slaveId, int addr, bool value);
        Task WriteMultipleCoils(string ip, int port, int slaveId, int start, bool[] values);
    }

    public class ModbusService : IModbusService {
        private readonly ILogger<ModbusService> _logger;
        private bool loggerEnabled;
        public ModbusService() {
            this.loggerEnabled = false;
        }

        public ModbusService(ILogger<ModbusService> logger) {
            this._logger = logger;
            this.loggerEnabled = true;
        }

        public async Task<ModbusResult> Read(string ip, int port, ModbusConfig config) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                ModbusResult result = new ModbusResult();

                if (config.DiscreteInputs != 0) {
                    result.DiscreteInputs = await modbus.ReadInputsAsync((byte)config.SlaveAddress, 0, (ushort)config.DiscreteInputs);
                    result._success = true;
                }

                if (config.HoldingRegisters != 0) {
                    result.HoldingRegisters = await modbus.ReadHoldingRegistersAsync((byte)config.SlaveAddress, 0, (ushort)config.HoldingRegisters);
                    result._success = true;
                }

                if (config.InputRegisters != 0) {
                    result.InputRegisters = await modbus.ReadInputRegistersAsync((byte)config.SlaveAddress, 0, (ushort)config.InputRegisters);
                    result._success = true;
                }

                if (config.Coils != 0) {
                    result.Coils = await modbus.ReadCoilsAsync((byte)config.SlaveAddress, 0, (ushort)config.Coils);
                    result._success = true;
                }
                
                client.Close();
                modbus.Dispose();
                return result;
            } catch {
                this.LogError("Exception reading modbus registers in ModbusService.Read");
                return new ModbusResult(false);
            }
        }

        public async Task WriteCoil(string ip, int port, int slaveId, int addr, bool value) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                await modbus.WriteSingleCoilAsync((byte)slaveId, (ushort)addr, value);
            } catch {
                this.LogError("Exception writing to single coil in ModbusService.WriteCoil");
            }
        }

        public async Task WriteMultipleCoils(string ip, int port, int slaveId, int start, bool[] values) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                await modbus.WriteMultipleCoilsAsync((byte)slaveId, (ushort)start, values);
            } catch {
                this.LogError("Exception writing multiple coils in ModbusService.WriteMultipleCoils");
            }
        }

        private void LogError(string msg) {
            if (this.loggerEnabled) {
                this._logger.LogInformation(msg);
            } else {
                Console.WriteLine("ModbusService Error: " + msg);
            }
        }

        private void LogInfo(string msg) {
            if (this.loggerEnabled) {
                this._logger.LogInformation(msg);
            } else {
                Console.WriteLine("ModbusService Info: " + msg);
            }
        }
    }
}
