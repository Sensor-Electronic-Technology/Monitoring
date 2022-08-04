using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services {
    public interface IDataLogger {
        Task Read();
        Task Load();
        Task Reload();
    }
}
