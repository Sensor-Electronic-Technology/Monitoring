using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringWeb.WebAppV2.Data;

namespace MonitoringWeb.WebAppV2.Services;
public class PlotDataService {
        private IMongoCollection<AnalogReadings> _analogReadings;
        private IMongoCollection<AnalogItem> _analogItems;

        public async Task<IEnumerable<AnalogReadingDto>> GetData(string deviceData,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceData);
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogItem>("analog_items");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            var analogItems = await this._analogItems.Find(e => e.Display && e.Identifier.Contains("H2 PPM"))
                .ToListAsync();
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()){
                var batch = cursor.Current;
                foreach (var readings in batch){
                    foreach (var aItem in analogItems) {
                        var reading =readings.readings.FirstOrDefault(e=>e.MonitorItemId==aItem._id);
                        
                        if (reading != null) {
                            
                            var aReading=new AnalogReadingDto(){
                                Name=aItem.Identifier,
                                TimeStamp = readings.timestamp.ToLocalTime(),
                                Value=reading.Value
                            }; 
                            analogReadings.Add(aReading);
                        }
                    }
                }
            }
            return analogReadings;
        }
        
        public async Task<IEnumerable<AnalogReadingDto>> GetChannelData(string deviceData,string chName,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceData);
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogItem>("analog_items");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            var h2psi = await this._analogItems.Find(e => e.Identifier == chName).FirstOrDefaultAsync();
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()){
                var batch = cursor.Current;
                foreach (var readings in batch){

                    var reading =readings.readings.FirstOrDefault(e=>e.MonitorItemId==h2psi._id);
                    if (reading != null) {
                        var aReading=new AnalogReadingDto(){
                            Name=h2psi.Identifier,
                            TimeStamp = readings.timestamp.ToLocalTime(),
                            Value=reading.Value
                        }; 
                        analogReadings.Add(aReading);
                    }
                }
            }
            return analogReadings;
        }
        
        public async Task<IEnumerable<AnalogReadingDto>> GetDataBySensor(string deviceData,DateTime start, DateTime stop,ObjectId sensorId) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceData);
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogItem>("analog_items");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            var analogItems = await this._analogItems.Find(e => e.Display && e.SensorId==sensorId)
                .ToListAsync();
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()){
                var batch = cursor.Current;
                foreach (var readings in batch){
                    foreach (var aItem in analogItems) {
                        var reading =readings.readings.FirstOrDefault(e=>e.MonitorItemId==aItem._id);
                        if (reading != null) {
                            var aReading=new AnalogReadingDto(){
                                Name=aItem.Identifier,
                                TimeStamp = readings.timestamp.ToLocalTime(),
                                Value=reading.Value
                            }; 
                            analogReadings.Add(aReading);
                        }
                    }
                }
            }
            return analogReadings;
        }
        
        public async Task<IEnumerable<AnalogReadingDto>> GetDataNew(string deviceData,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase(deviceData);
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogItem>("analog_items");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            var analogItems = await this._analogItems.Find(e => e.Display && e.Identifier.Contains("H2 PPM"))
                .ToListAsync();
            var readings = await this._analogReadings.Find(e => e.timestamp >= start && e.timestamp <= stop)
                .ToListAsync();
            DateTime min = readings.Min(e => e.timestamp);
            foreach (var reading in readings) {
                var delta = (reading.timestamp - min).TotalHours;
                foreach (var item in analogItems) {
                    var aReading =reading.readings.FirstOrDefault(e=>e.MonitorItemId==item._id);
                    if (aReading != null) {
                        analogReadings.Add(new AnalogReadingDto() {
                            Time = delta,
                            TimeStamp = reading.timestamp,
                            Name=item.Identifier,
                            Value = aReading.Value
                        });
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
        
        public async Task<IEnumerable<AnalogReadingDto>> GetDataBySensor(List<string> deviceData,DateTime start, DateTime stop) {
            var client = new MongoClient("mongodb://172.20.3.41");
            List<AnalogReadingDto> analogReadings = new List<AnalogReadingDto>();
            foreach (var data in deviceData){
                var readings=await this.GetData(data, start, stop);
                analogReadings.AddRange(readings);
            }
            return analogReadings;
        }
    }