using Microsoft.Extensions.Caching.Memory;
using MonitoringData.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services.DataLogging {
    public class DataFixedQueue<T> : Queue<T> {
        private readonly int maxQueueSize;
        private readonly object syncRoot = new object();

        public DataFixedQueue(int maxSize) {
            this.maxQueueSize = maxSize;
        }

        public new void Enqueue(T item) {
            lock (syncRoot) {
                base.Enqueue(item);
                if (base.Count >= 10)
                    base.Dequeue();
            }
        }

    }

    public class DataCache<T> {
        private IMemoryCache _cache;
        DataCache() {

        }
    }
}
