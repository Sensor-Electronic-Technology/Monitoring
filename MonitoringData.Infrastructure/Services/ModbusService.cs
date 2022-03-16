using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
    }

    public interface IModbusService {
        Task<ModbusResult> Read(string ip, int port, ModbusConfig config);
        Task Write(string ip, int port, ModbusConfig config);
    }

    public static class ModbusService {
        public static async Task<ModbusResult> Read(string ip, int port, ModbusConfig config) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var dInputs = await modbus.ReadInputsAsync((byte)config.SlaveAddress, 0, (ushort)config.DiscreteInputs);
                var holding = await modbus.ReadHoldingRegistersAsync((byte)config.SlaveAddress, 0, (ushort)config.HoldingRegisters);
                var inputs = await modbus.ReadInputRegistersAsync((byte)config.SlaveAddress, 0, (ushort)config.InputRegisters);
                var coils = await modbus.ReadCoilsAsync((byte)config.SlaveAddress, 0, (ushort)config.Coils);
                client.Close();
                modbus.Dispose();
                return new ModbusResult(true) { Coils = coils, DiscreteInputs = dInputs, HoldingRegisters = holding, InputRegisters = inputs };
            } catch {
                return new ModbusResult(false);
            }
        }

        public static async Task Write(string ip,int port,ModbusConfig config) {
            return;
        }

        public static async Task<ushort[]> ReadHolding(string ip, int port, int slaveId, int start, int length) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var holding = await modbus.ReadHoldingRegistersAsync((byte)slaveId, (ushort)start, (ushort)length);
                client.Close();
                modbus.Dispose();
                return holding;
            } catch {
                return null;
            }
        }

        public static async Task<bool[]> ReadInputs(string ip, int port, int slaveId, int start, int length) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var inputs = await modbus.ReadInputsAsync((byte)slaveId, (ushort)start, (ushort)length);
                client.Close();
                modbus.Dispose();
                return inputs;
            } catch {
                return null;
            }
        }

        public static async Task<ushort[]> ReadInputRegisters(string ip, int port, int slaveId, int start, int length) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var holding = await modbus.ReadInputRegistersAsync((byte)slaveId, (ushort)start, (ushort)length);
                client.Close();
                modbus.Dispose();
                return holding;
            } catch {
                return null;
            }
        }

        public static async Task<bool[]> ReadCoils(string ip, int port, int slaveId, int start, int length) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                var coils = await modbus.ReadCoilsAsync((byte)slaveId, (ushort)start, (ushort)length);
                client.Close();
                modbus.Dispose();
                return coils;
            } catch {
                return null;
            }
        }

        public static async Task WriteCoil(string ip, int port, int slaveId, int addr, bool value) {
            try {
                using var client = new TcpClient(ip, port);
                var modbus = ModbusIpMaster.CreateIp(client);
                await modbus.WriteSingleCoilAsync((byte)slaveId, (ushort)addr, value);
            } catch { }
        }
    }
}
