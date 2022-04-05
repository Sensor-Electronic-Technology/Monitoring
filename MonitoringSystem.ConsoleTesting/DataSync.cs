using MonitoringConfig.Infrastructure.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.ConsoleTesting {
    public interface IDataSync {

        void CreateDeviceReadingsDB(string device);
        void UpdateAlertDisplayNames(string device);
        void UpdateReadingsDB(string device);
        void VerifyAndSyncDB(string device);

    }

    public class DataSync : IDataSync {

        private readonly FacilityContext _context;
        public void CreateDeviceReadingsDB(string device) {
            throw new NotImplementedException();
        }

        public void UpdateAlertDisplayNames(string device) {
            throw new NotImplementedException();
        }

        public void UpdateReadingsDB(string device) {
            throw new NotImplementedException();
        }

        public void VerifyAndSyncDB(string device) {
            throw new NotImplementedException();
        }
    }
}
