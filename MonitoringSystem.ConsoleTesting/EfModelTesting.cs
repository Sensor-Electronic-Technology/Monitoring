using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Data.Model;



namespace MonitoringSystem.ConsoleTesting; 

public class EfModelTesting {
    static async Task Main(string[] args) {
        using var context = new MonitorContext();
        var devices=context.Devices.OfType<MonitorBox>()
            .Include(e => e.Channels)
            .Include(e => e.ModbusConfiguration)
            .Include(e => e.NetworkConfiguration)
            .Include(e => e.ChannelRegisterMap).AsEnumerable();
    }
}