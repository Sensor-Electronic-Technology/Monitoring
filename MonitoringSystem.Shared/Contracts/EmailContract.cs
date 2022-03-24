using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringSystem.Shared.Contracts {
    public interface EmailContract {
        string Subject { get; }
        string Message { get; }
        
    }
}
