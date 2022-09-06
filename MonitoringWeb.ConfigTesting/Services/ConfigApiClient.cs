using MonitoringSystem.ConfigApi.Contracts.Requests.Get;
using MonitoringSystem.ConfigApi.Contracts.Responses.Get;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringWeb.ConfigTesting.Services; 

public class ConfigApiClient {
    private readonly HttpClient _client;

    public ConfigApiClient(HttpClient client) {
        this._client = client;
    }

    public async Task<IEnumerable<ModbusDeviceDto>?> GetModbusDevice() {
        return await this._client.GetFromJsonAsync<IEnumerable<ModbusDeviceDto>>("devices");
    }

    public async Task<IEnumerable<DeviceActionDto>> GetDeviceAction(string id) {
        var response = await this._client.GetFromJsonAsync<GetDeviceActionsResponse>($"actions/deviceactions/{id}");
        if (response is not null) {
            return response.DeviceActions;
        } else {
            return Enumerable.Empty<DeviceActionDto>();   
        }
    }

    public async Task<IEnumerable<AnalogInputDto>> GetAnalogChannels(string deviceId) {
        var response = await this._client.GetFromJsonAsync<GetAnalogChannelsResponse>($"channels/analog/{deviceId}");
        return response.AnalogInputs;
    }
    
    public async Task<IEnumerable<DiscreteInputDto>> GetDiscreteChannels(string deviceId) {
        var response = await this._client.GetFromJsonAsync<GetDiscreteChannelsResponse>($"channels/discrete/{deviceId}");
        return response.DiscreteInputs;
    }
    
    public async Task<IEnumerable<VirtualInputDto>> GetVirtualChannels(string deviceId) {
        var response = await this._client.GetFromJsonAsync<GetVirtualChannelsResponse>($"channels/virtual/{deviceId}");
        return response.VirtualInputs;
    }
    
    public async Task<IEnumerable<DiscreteOutputDto>> GetOutputChannels(string deviceId) {
        var response = await this._client.GetFromJsonAsync<GetOutputChannelsResponse>($"channels/output/{deviceId}");
        return response.OutputChannels;
    }

    public async Task<AnalogAlertDto> GetAnalogAlert(string channelId) {
        var response = await this._client.GetFromJsonAsync<GetAnalogAlertResponse>($"alerts/analog/{channelId}");
        return response.AnalogAlert;
    }
    
    public async Task<DiscreteAlertDto> GetDiscreteAlert(string channelId) {
        var response = await this._client.GetFromJsonAsync<GetDiscreteAlertResponse>($"alerts/discrete/{channelId}");
        return response.DiscreteAlert;
    }

    public async Task<IEnumerable<AnalogLevelDto>> GetAnalogAlertLevels(string alertId) {
        var response = await this._client.GetFromJsonAsync<GetAnalogLevelsResponse>($"alerts/analog/levels/{alertId}");
        return response.AnalogLevels;
    }
    
    public async Task<DiscreteLevelDto> GetDiscreteAlertLevel(string alertId) {
        var response = await this._client.GetFromJsonAsync<GetDiscreteLevelResponse>($"alerts/discrete/levels/{alertId}");
        return response.DiscreteLevel;
    }
}