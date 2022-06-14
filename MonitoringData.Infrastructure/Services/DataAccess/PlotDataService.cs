using System.Text;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services.DataAccess {
    public class AnalogReadingDto {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
    }

    public class PlotDataService
    {
        private IMongoCollection<AnalogReadings> _analogReadings;
        private IMongoCollection<AnalogChannel> _analogItems;

        public async Task<IEnumerable<AnalogReadingDto>> GetData(string deviceData,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceData);
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogChannel>("analog_items");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            var analogItems = await this._analogItems.Find(e => e.display && e.identifier.Contains("H2 PPM"))
                .ToListAsync();
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()){
                var batch = cursor.Current;
                foreach (var readings in batch){
                    foreach (var aItem in analogItems) {
                        var reading =readings.readings.FirstOrDefault(e=>e.itemid==aItem._id);
                        if (reading != null) {
                            var aReading=new AnalogReadingDto(){Name=aItem.identifier,
                                TimeStamp = readings.timestamp.ToLocalTime(),
                                Value=reading.value
                            }; 
                            analogReadings.Add(aReading);
                        }
                    }
                }
            }
            return analogReadings;
        }
        public async Task<IEnumerable<AnalogReadingDto>> GetData(List<string> deviceData,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            foreach (var data in deviceData){
                var readings=await this.GetData(data, start, stop);
                analogReadings.AddRange(readings);
            }
            return analogReadings;
        }
    }
}
