using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Configuration;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public interface IMonitorDataService {
        Task CreateDatabase(string deviceId);
        Task AddAnalogItems(IList<MonitorItemDTO> items);
        Task AddDiscreteItems(IList<MonitorItemDTO> items);
        Task AddOutputItems(IList<MonitorItemDTO> items);
        Task AddVirtualItems(IList<MonitorItemDTO> items);
        Task AddActionItems(IList<MonitorItemDTO> items);
        Task AddDeviceItem(DeviceDTO device);
        Task AddMonitorAlerts(IList<MonitorAlertDTO> monitorAlerts);
        Task AddMonitorFacilityActions(IList<FacilityActionDTO> facilityActions);
    }

    public class MonitorDataService {
        private MonitorDevice _monitorDevice;
        private readonly IMongoCollection<AnalogChannel> _analogChannels;
        private readonly IMongoCollection<DiscreteChannel> _discreteChannels;
        private readonly IMongoCollection<OutputChannel> _outputChannels;
        private readonly IMongoCollection<VirtualChannel> _virtualChannels;
        private readonly IMongoCollection<MonitorAlert> _monitorAlerts;
        private readonly IMongoCollection<MonitorAction> _monitorActions;

        private readonly IMongoCollection<AnalogReading> _analogReadings;
        private readonly IMongoCollection<DiscreteReading> _discreteReadings;
        private readonly IMongoCollection<OutputReading> _outputReadings;
        private readonly IMongoCollection<VirtualReading> _virtualReadings;
        private readonly IMongoCollection<ActionReading> _actionReadings;
        private readonly IMongoCollection<AlertReading> _alertReadings;

        public MonitorDataService(IOptions<MonitorDatabaseSettings> databaseSettings) {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
        }

        public MonitorDataService() {

        }

    }
}
