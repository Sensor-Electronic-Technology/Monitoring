﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitoringConfig.Infrastructure.Data.Model;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringConfig.Infrastructure.Data.DataAccess {
    public class MonitoringRepository {
        private readonly FacilityContext _context;
        private readonly ILogger<MonitoringRepository> _logger;

        public MonitoringRepository(FacilityContext context, ILogger<MonitoringRepository> logger) {
            this._context = context;
            this._logger = logger;
        }

        //public async Task<T> GetDevice<T>(string deviceId) where T:Device {
        //    return await this._context.Devices.OfType<T>().FirstOrDefaultAsync(e => e.Identifier == deviceId);
        //}

        //public async Task<NetworkConfiguration> GetNetworkConfiguration() {

        //}
    }
}
