using System.Text;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.WebApp.Data {

    public class AnalogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AnalogReadingDto {
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }
    }
    public class FacilityDataService
    {
        private IMongoCollection<AnalogReadings> _analogReadings;
        private IMongoCollection<AnalogChannel> _analogItems;
        public FacilityDataService() {
            var client=new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi2_data");
            this._analogReadings = database.GetCollection<AnalogReadings>("analog_readings");
            this._analogItems = database.GetCollection<AnalogChannel>("analog_items");
        }

        public async Task<IDictionary<string,IEnumerable<AnalogReadingDto>>> GetData(DateTime start, DateTime stop)
        {
            var analogItems = (await (await this._analogItems.FindAsync(e => e.display)).ToListAsync());
            var analogDtos = analogItems.Select(e=>new AnalogItemDto(){Id=e._id,Name=e.identifier}).ToList();
            using var cursor = await this._analogReadings.FindAsync(e => e.timestamp >= start && e.timestamp <= stop);
            while (await cursor.MoveNextAsync()){
                var batch = cursor.Current;
                List<AnalogReadingDto> analogReadings=new List<AnalogReadingDto>();
                
                foreach (var readings in batch)
                {
                    
                    foreach (var reading in readings.readings){
                        
                    }
                }
            }
        }

    }
}
