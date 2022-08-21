using MongoDB.Bson;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringData.DataApi.Repositories; 

public interface IAnalogItemRepository {
    Task<AnalogItem> GetAnalogItem(ObjectId id);
    Task<IEnumerable<AnalogItem>> GetAnalogItems();
}