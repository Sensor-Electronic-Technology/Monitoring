using Microsoft.Extensions.Logging;
using MonitoringConfig.Infrastructure.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services {

    public interface IDataLogger {
        Task Read();
        Task Load();
    }

    public class ModbusDataLogger : IDataLogger {

        private readonly IMonitorDataService _monitorDataService;
        private readonly ILogger _logger;
        private readonly FacilityContext _context;

        public ModbusDataLogger(IMonitorDataService dataService,ILogger logger,FacilityContext context) {
            this._monitorDataService = dataService;
            this._logger = logger;
            this._context = context;
        }

        public Task Load() {
            throw new NotImplementedException();
        }

        public Task Read() {
            throw new NotImplementedException();
        }
    }

}
