using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Modbus.Device;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Services {
    public class ModbusResult {
        public bool[] DiscreteInputs { get; set; }
        public bool[] Coils { get; set; }
        public ushort[] HoldingRegisters { get; set; }
        public ushort[] InputRegisters { get; set; }
        public bool Success { get; set; }
        public ModbusResult(bool success) {
            this.Success = success;
        }

        public ModbusResult() {
            this.Success = false;
        }
    }
    
    public interface IModbusService {
        Task<ModbusResult> Read(string ip, int port, ModbusConfigDto configurationDto);
        Task WriteMultipleCoils(string ip, int port, int slaveId, int start, bool[] values);
        Task ToggleCoil(string ip, int port, int slaveId, int addr);
        Task<bool> ReadCoil(string ip, int port,int slaveId, int addr);
        Task<bool[]> ReadCoils(string ip, int port, int slaveId, int addr, int length);
        Task WriteCoil(string ip, int port, int slaveId, int addr, bool value);
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

        public async Task<ModbusResult> Read(string ip, int port, ModbusConfigDto configurationDto) {
            try {
                using var client = new TcpClient(ip, port);
                client.ReceiveTimeout = 500;
                var modbus = ModbusIpMaster.CreateIp(client);
                
                ModbusResult result = new ModbusResult();
                if (configurationDto.DiscreteInputs != 0) {
                    result.DiscreteInputs = await modbus.ReadInputsAsync((byte)configurationDto.SlaveAddress, 0, 
                        (ushort)configurationDto.DiscreteInputs);
                    result.Success = true;
                }

                if (configurationDto.HoldingRegisters != 0) {
                    result.HoldingRegisters = await modbus.ReadHoldingRegistersAsync((byte)configurationDto.SlaveAddress, 0, 
                        (ushort)configurationDto.HoldingRegisters);
                    result.Success = true;
                }

                if (configurationDto.InputRegisters != 0) {
                    result.InputRegisters = await modbus.ReadInputRegistersAsync((byte)configurationDto.SlaveAddress, 0, 
                        (ushort)configurationDto.InputRegisters);
                    result.Success = true;
                }

                if (configurationDto.Coils != 0) {
                    result.Coils = await modbus.ReadCoilsAsync((byte)configurationDto.SlaveAddress, 0, 
                        (ushort)configurationDto.Coils);
                    result.Success = true;
                }
                
                client.Close();
                modbus.Dispose();
                return result;
            } catch {
                this.LogError("Exception reading modbus registers in ModbusService.Read");
                return new ModbusResult(false);
            }
        }

        public async Task<bool> ReadCoil(string ip, int port,int slaveId, int addr) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var value = await modbus.ReadCoilsAsync((byte)slaveId, (ushort)addr, 1);
                return value[0];
            } catch {
                this.LogError("Exception reading coil in ModbusService.ReadCoil");
                return false;
            }
        }
        
        public async Task<bool[]> ReadCoils(string ip, int port,int slaveId, int addr,int length) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var value = await modbus.ReadCoilsAsync((byte)slaveId, (ushort)addr, (ushort)length);
                return value;
            } catch {
                this.LogError("Exception reading coil in ModbusService.ReadCoil");
                return  null;
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

        public async Task ToggleCoil(string ip, int port, int slaveId, int addr) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var value = await modbus.ReadCoilsAsync((byte)slaveId, (ushort)addr, 1);
                await modbus.WriteSingleCoilAsync((byte)slaveId, (ushort)addr, !value[0]);
            } catch {
                this.LogError("Exception reading coil in ModbusService.ReadCoil");
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
