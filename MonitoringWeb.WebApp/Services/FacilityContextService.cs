using Microsoft.EntityFrameworkCore;
using MonitoringConfig.Infrastructure.Data.Model;

namespace MonitoringWeb.WebApp.Services {
    
    public class FacilityContextService {
        private FacilityContext _context;
        public FacilityContextService(IDbContextFactory<FacilityContext> contextFactory) {
            this._context = contextFactory.CreateDbContext();
        }

        public Task<IEnumerable<ModbusDevice>> GetModbusDevices() {
            return Task.FromResult<IEnumerable<ModbusDevice>>(
                this._context.Devices.OfType<ModbusDevice>().ToList());
        }

    }
}
