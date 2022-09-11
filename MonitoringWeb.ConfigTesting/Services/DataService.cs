using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Services;

namespace MonitoringWeb.ConfigTesting.Services; 

public class DataService {
    //private readonly HttpClient _client;
    private readonly ConfigApiClient _client;

    public DataService(ConfigApiClient client) {
        //this._client = client;
        this._client = client;
    }

    public async Task<IEnumerable<ModbusDeviceDto>> GetAllDevices() {
        return await this._client.GetModbusDevices();
        //return await this._client.GetFromJsonAsync<IEnumerable<ModbusDeviceDto>>("https://localhost:7133/devices");
    }
    
    

    public async Task UpdateModbusDevice(ModbusDeviceDto device,ModbusDeviceDto updated) {
        device.Database = updated.Database;
        device.Name = updated.Name;
        device.HubAddress = updated.HubAddress;
        device.HubName = updated.HubName;
        device.ReadInterval = updated.ReadInterval;
        device.SaveInterval = updated.SaveInterval;
        /*UpdateDeviceRequest request = new UpdateDeviceRequest() { ModbusDevice = device };*/
        //var responseMessage = await this._client.PutAsJsonAsync("https://localhost:7133/devices/ModbusDevice", request);
        
        /*if (responseMessage.IsSuccessStatusCode) {
            Console.WriteLine("Success");
        }*/
    }
}