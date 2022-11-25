using MonitoringSystem.Shared.Contracts.Requests.Insert;
using MonitoringSystem.Shared.Contracts.Requests.Update;
using MonitoringSystem.Shared.Contracts.Responses.Get;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringWeb.WebApp.Services; 

public class ConfigApiClient {
    private readonly HttpClient _client;

    public ConfigApiClient(HttpClient client) {
        this._client = client;
    }

    public async Task<IEnumerable<ModbusDeviceDto>?> GetModbusDevices() {
        return await this._client.GetFromJsonAsync<IEnumerable<ModbusDeviceDto>>("devices");
    }

    public async Task UpdateDevice(ModbusDeviceDto device) {
        var response = await this._client.PutAsJsonAsync($"devices/ModbusDevice",
            new UpdateDeviceRequest(){ModbusDevice = device});
    }

    public async Task<IEnumerable<SensorDto>?> GetSensors() {
        var response= await this._client.GetFromJsonAsync<GetAllSensorsResponse>("sensors");
        return response?.Sensors;
    }

    public async Task UpdateSensor(SensorDto sensor) {
        var response = await this._client.PutAsJsonAsync("sensors/Sensor", 
                new UpdateSensorRequest() { Sensor = sensor });
    }
    
    public async Task InsertSensor(SensorDto sensor) {
        var response = await this._client.PostAsJsonAsync("sensors/Sensor",
            new InsertSensorRequest() { Sensor = sensor });
    } 

    public async Task<IEnumerable<FacilityActionDto>?> GetFacilityActions() {
        var response = await this._client.GetFromJsonAsync<GetFacilityActionsResponse>("actions/facility");
        return response?.FacilityActions;
    }

    public async Task UpdateFacilityAction(FacilityActionDto action) {
        var response = await this._client.PutAsJsonAsync("actions/facility/FacilityAction",
            new UpdateFacilityActionRequest() { FacilityAction = action });
    }

    public async Task<IEnumerable<DeviceActionDto>> GetDeviceActions(string id) {
        var response = await this._client.GetFromJsonAsync<GetDeviceActionsResponse>($"actions/device/{id}");
        return response.DeviceActions;
    }

    public async Task UpdateDeviceAction(DeviceActionDto action) {
        var response = await this._client.PutAsJsonAsync("actions/device/DeviceAction",
            new UpdateDeviceActionRequest() { DeviceAction = action });
    }

    public async Task UpdateChannel(ChannelDto channelDto) {
        switch (channelDto) {
            case AnalogInputDto analogInput: {
                var request = new UpdateAnalogChannelRequest() { AnalogChannel = analogInput };
                var response=await this._client.PutAsJsonAsync<UpdateAnalogChannelRequest>($"channels/analog/AnalogChannel",request);
                break;
            }
            case DiscreteInputDto discreteInput: {
                var request = new UpdateDiscreteChannelRequest() { DiscreteChannel = discreteInput };
                await this._client.PutAsJsonAsync<UpdateDiscreteChannelRequest>($"channels/discrete/DiscreteChannel",request);
                break;
            }
            case VirtualInputDto virtualInput: {
                var request = new UpdateVirtualChannelRequest() { VirtualChannel = virtualInput };
                await this._client.PutAsJsonAsync<UpdateVirtualChannelRequest>($"channels/virtual/VirtualChannel",request);
                break;
            }
            case DiscreteOutputDto discreteOutput: {
                var request = new UpdateOutputChannelRequest() { OutputChannel = discreteOutput };
                await this._client.PutAsJsonAsync<UpdateOutputChannelRequest>($"channels/output/OutputChannel",request);
                break;
            }
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

    public async Task<AlertDto> GetAlert(string channelId) {
        var response = await this._client.GetFromJsonAsync<GetAlertResponse>($"alerts/{channelId}");
        return response.Alert;
    }
    
    public async Task<IEnumerable<AnalogLevelDto>> GetAnalogAlertLevels(string alertId) {
        var response = await this._client.GetFromJsonAsync<GetAnalogLevelsResponse>($"alerts/levels/analog/{alertId}");
        return response.AnalogLevels;
    }
    
    public async Task<DiscreteLevelDto> GetDiscreteAlertLevel(string alertId) {
        var response = await this._client.GetFromJsonAsync<GetDiscreteLevelResponse>($"alerts/levels/discrete/{alertId}");
        return response.DiscreteLevel;
    }

    public async Task UpdateAlert(AlertDto alert) {
        var alertRequest = new UpdateAlertRequest() { Alert = alert };
        var alertResponse = await this._client.PutAsJsonAsync("alerts/Alert", alertRequest);

    }
    
    public async Task UpdateDiscreteLevel(DiscreteLevelDto level) {
        var levelRequest = new UpdateDiscreteLevelRequest() { DiscreteLevel = level };
        var levelResponse= await this._client.PutAsJsonAsync("alerts/levels/discrete/DiscreteLevel",levelRequest);
    }

    public async Task UpdateAnalogLevel(AnalogLevelDto level) {
        var request = new UpdateAnalogLevelRequest() { AnalogLevel = level };

        var response=await this._client.PutAsJsonAsync<UpdateAnalogLevelRequest>($"alerts/levels/analog/AnalogLevel",request);
        var content = await response.Content.ReadAsStringAsync();
        var requestContent=await response.RequestMessage.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        Console.WriteLine(requestContent);
    }
}