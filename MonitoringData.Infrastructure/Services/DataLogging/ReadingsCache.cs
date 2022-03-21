using Microsoft.Extensions.Caching.Memory;
using MonitoringData.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class ReadingsCache {
        public MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public Stack<AnalogReading> AnalogReadings { get; private set; }

        public ReadingsCache() {

        }

        public void InsertOne(AnalogReading reading) {
            this._cache.GetOrCreate<AnalogReading>(reading);
            if (this.AnalogReadings.Count >= 10) {
                //this.AnalogReadings.Re
            }
        }

    }
}
