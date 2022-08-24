using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Requests.Update;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringWeb.ConfigTesting.Services; 

public class DataService {
    private readonly HttpClient _client;

    public DataService(HttpClient client) {
        this._client = client;
    }

    public async Task<IEnumerable<ModbusDeviceDto>> GetAllDevices() {
        return await this._client.GetFromJsonAsync<IEnumerable<ModbusDeviceDto>>("https://localhost:7133/devices");
    }

    public async Task<GetDeviceChannelsResponse> GetDeviceChannels(Guid deviceId) {
        GetDeviceChannelsRequest request = new GetDeviceChannelsRequest() { Id = deviceId };
        var response=await this._client.GetFromJsonAsync<GetDeviceChannelsResponse>($"https://localhost:7133/channels/{request.Id}");
        return response;
    }

    public async Task UpdateModbusDevice(ModbusDeviceDto device,ModbusDeviceDto updated) {
        device.Database = updated.Database;
        device.Name = updated.Name;
        device.HubAddress = updated.HubAddress;
        device.HubName = updated.HubName;
        device.ReadInterval = updated.ReadInterval;
        device.SaveInterval = updated.SaveInterval;
        UpdateDeviceRequest request = new UpdateDeviceRequest() { ModbusDevice = device };
        var responseMessage = await this._client.PutAsJsonAsync("https://localhost:7133/devices/ModbusDevice", request);
        if (responseMessage.IsSuccessStatusCode) {
            Console.WriteLine("Success");
        }
    }
}