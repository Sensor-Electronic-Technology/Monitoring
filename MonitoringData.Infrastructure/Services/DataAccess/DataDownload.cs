using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services.DataAccess {
    public class DataDownload {
        IMongoCollection<AnalogReadings> analogReadings { get; set; }
        IMongoCollection<AnalogChannel> analogChannels { get; set; }


        public DataDownload() {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            this.analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this.analogChannels = database.GetCollection<AnalogChannel>("analog_items");
        }

        public async Task<byte[]> GetData(DateTime start,DateTime stop) {
            var analogItems = await (await this.analogChannels.FindAsync(_ => true)).ToListAsync();
            //var data = await (await this.analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)).ToListAsync();
            var headers = analogItems.Select(e => e.identifier).ToList();
            StringBuilder hbuilder = new StringBuilder();
            hbuilder.Append("timestamp,");
            headers.ForEach((id) => {
                hbuilder.Append($"{id},");
            });
            using (var cursor=await this.analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop)) {
                while(await cursor.MoveNextAsync()) {
                    var batch = cursor.Current;
                    foreach(var readings in batch) {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(readings.timestamp.ToString() + ",");
                        foreach (var reading in readings.readings) {
                            builder.Append($"{reading.value},");
                        }
                        hbuilder.AppendLine(builder.ToString());
                    }
                }
            }
            return Encoding.ASCII.GetBytes(hbuilder.ToString());
        }
    }
}
