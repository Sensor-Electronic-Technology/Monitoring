using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Modbus.Device;
using MonitoringSystem.Shared.Data;
namespace MonitoringSystem.Shared.Services;

public class AmmoniaController {
    private readonly ILogger<AmmoniaController> _logger;
    private bool _loggingEnabled;

    public AmmoniaController(ILogger<AmmoniaController> logger) {
        this._logger = logger;
        this._loggingEnabled = true;
    }

    public AmmoniaController() {
        this._loggingEnabled = false;
    }
    
    public async Task SetCalibrationMode(string ip,bool mode) {
        try {
            using var client = new TcpClient(ip, 502);
            var modbus = ModbusIpMaster.CreateIp(client);
            await modbus.WriteSingleCoilAsync((byte)1, (ushort)1, mode);
        } catch {
            this.LogError("Exception writing to single coil in ModbusService.WriteCoil");
        }
    }

    public async Task<AmmoniaCalibrationData?> GetTankCalibration(string ip,int tank) {
        using var client = new TcpClient(ip, 502);
        client.ReceiveTimeout = 500;
        var modbus = ModbusIpMaster.CreateIp(client);
        var registers=await modbus.ReadHoldingRegistersAsync((byte)1,0,(ushort)70);
        AmmoniaCalibrationData calData = new AmmoniaCalibrationData();
        calData.Scale = tank;
        switch (tank) {
            case 1: {
                calData.CurrentWeight=BitConverter.ToInt32(BitConverter.GetBytes(registers[1])
                        .Concat(BitConverter.GetBytes(registers[0])).ToArray(), 0);
                calData.Tare = registers[56];
                var rawData = new ArraySegment<ushort>(registers, 8, 12).ToArray();
                return await Convert(rawData,calData);
                break;
            }
            case 2: {
                calData.CurrentWeight=BitConverter.ToInt32(BitConverter.GetBytes(registers[3])
                    .Concat(BitConverter.GetBytes(registers[2])).ToArray(), 0);
                calData.Tare = registers[57];
                var rawData = new ArraySegment<ushort>(registers, 20, 12).ToArray();
                return await Convert(rawData,calData);
                break;
            }
            case 3: {
                calData.CurrentWeight=BitConverter.ToInt32(BitConverter.GetBytes(registers[5])
                    .Concat(BitConverter.GetBytes(registers[4])).ToArray(), 0);
                calData.Tare = registers[58];
                var rawData = new ArraySegment<ushort>(registers, 32, 12).ToArray();
                return await Convert(rawData,calData);
                break;
            }
            case 4: {
                calData.CurrentWeight=BitConverter.ToInt32(BitConverter.GetBytes(registers[7])
                    .Concat(BitConverter.GetBytes(registers[6])).ToArray(), 0);
                calData.Tare = registers[59];
                var rawData = new ArraySegment<ushort>(registers, 44, 12).ToArray();
                return await Convert(rawData,calData);
                break;
            }
            default:
                return null;
        }
    }

    private Task<AmmoniaCalibrationData?> Convert(ushort[] raw,AmmoniaCalibrationData data) {
        data.ZeroRawValue=BitConverter.ToInt32(BitConverter.GetBytes(raw[1])
            .Concat(BitConverter.GetBytes(raw[0])).ToArray(), 0);
        
        data.NonZeroRawValue=BitConverter.ToInt32(BitConverter.GetBytes(raw[3])
            .Concat(BitConverter.GetBytes(raw[2])).ToArray(), 0);
        
        data.ZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(raw[5])
            .Concat(BitConverter.GetBytes(raw[4])).ToArray(), 0);
        
        data.NonZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(raw[7])
            .Concat(BitConverter.GetBytes(raw[6])).ToArray(), 0);
        
        data.Combined=BitConverter.ToInt32(BitConverter.GetBytes(raw[9])
            .Concat(BitConverter.GetBytes(raw[8])).ToArray(), 0);
        
        data.GasWeight=BitConverter.ToInt32(BitConverter.GetBytes(raw[11])
            .Concat(BitConverter.GetBytes(raw[10])).ToArray(), 0);
        
        return Task.FromResult(data);
    }

    private async Task<int> GetZeroRawValue(string ip, int scale) {
        using var client = new TcpClient(ip, 502);
        client.ReceiveTimeout = 500;
        var modbus = ModbusIpMaster.CreateIp(client);
        await modbus.WriteSingleCoilAsync((byte)1, (ushort)1, true);
        await Task.Delay(250);
        int rawZeroValue = 0;
        switch (scale) {
            case 1: {
                var registers=await modbus.ReadHoldingRegistersAsync((byte)1,0,(ushort)2);
                rawZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(registers[1])
                    .Concat(BitConverter.GetBytes(registers[0])).ToArray(), 0);
                break;
            }
            case 2: {
                var registers=await modbus.ReadHoldingRegistersAsync((byte)1,2,(ushort)2);
                rawZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(registers[1])
                    .Concat(BitConverter.GetBytes(registers[0])).ToArray(), 0);
                break;
            }
            case 3: {
                var registers=await modbus.ReadHoldingRegistersAsync((byte)1,4,(ushort)2);
                rawZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(registers[1])
                    .Concat(BitConverter.GetBytes(registers[0])).ToArray(), 0);
                break;
            }
            case 4: {
                var registers=await modbus.ReadHoldingRegistersAsync((byte)1,6,(ushort)2);
                rawZeroValue=BitConverter.ToInt32(BitConverter.GetBytes(registers[1])
                    .Concat(BitConverter.GetBytes(registers[0])).ToArray(), 0);
                break;
            }
            default: {
                rawZeroValue = 0;
                break;
            }
        }
        await modbus.WriteSingleCoilAsync((byte)1, (ushort)1, true);
        return rawZeroValue;
    }

    public async Task SetCalibration(string ip,AmmoniaCalibrationData calibration) {
        using var client = new TcpClient(ip, 502);
        client.ReceiveTimeout = 500;
        var modbus = ModbusIpMaster.CreateIp(client);
        List<ushort> registersToWrite = new List<ushort>();
        registersToWrite.AddRange(ToUshortArray(calibration.ZeroRawValue));
        registersToWrite.AddRange(ToUshortArray(calibration.NonZeroRawValue));
        registersToWrite.AddRange(ToUshortArray(calibration.ZeroValue));
        registersToWrite.AddRange(ToUshortArray(calibration.NonZeroValue));
        registersToWrite.AddRange(ToUshortArray(calibration.Combined));
        registersToWrite.AddRange(ToUshortArray(calibration.GasWeight));
        registersToWrite.Add((ushort)calibration.Scale);
        await modbus.WriteMultipleRegistersAsync((byte)1, 70, registersToWrite.ToArray());
        await modbus.WriteSingleCoilAsync((byte)1, (ushort)0, true);
    }
    
    private ushort[] ToUshortArray(int value) {
        ushort[] s = new ushort[2];
        byte[] fBytes = BitConverter.GetBytes(value);
        s[0] = BitConverter.ToUInt16(fBytes, 2);
        s[1] = BitConverter.ToUInt16(fBytes, 0);
        return s;
    }

    private void LogError(string message) {
        if (this._loggingEnabled) {
            this._logger.LogError(message);
        } else {
            Console.WriteLine($"Error: {message}");
        }
    }
    
    private void LogInfo(string message) {
        if (this._loggingEnabled) {
            this._logger.LogInformation(message);
        } else {
            Console.WriteLine($"Info: {message}");
        }
    }
}