using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;
using MonitoringSystem.Shared.SignalR;
using MonitoringWeb.WebApp.Hubs;
namespace MonitoringWeb.WebApp.Services; 


public class AmmoniaHubService:BackgroundService,IAsyncDisposable {
    private readonly ILogger<AmmoniaHubService> _logger;
    private readonly IHubContext<AmmoniaHub, ISendTankWeightsCommand> _hubContext;
    private readonly ManagedDevice _device;
    private HubConnection? _hubConnection;


    public AmmoniaHubService(IHubContext<AmmoniaHub, ISendTankWeightsCommand> hubContext,
        WebsiteConfigurationProvider configurationProvider,
        ILogger<AmmoniaHubService> logger) {
        this._hubContext = hubContext;
        this._logger = logger;
        this._device = configurationProvider.GetDevice("nh3");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await this.HubSetup();
        /*_timer = new Timer(FireSignalRAsync, null, TimeSpan.Zero,
        TimeSpan.FromSeconds(1));
        await Task.CompletedTask;*/
    }

    private async void FireSignalRAsync(object? state) {
        Random rand = new Random();
        List<int> tankWeights = new List<int>();
        for (int i = 0; i < 4; i++) {
            tankWeights.Add(Convert.ToInt32(rand.NextInt64(400, 1000)));
        }
        await this._hubContext.Clients.All.SendTankWeights(tankWeights);
    }
    
    private async Task HubSetup() {
        var hubAddress = this._device.HubAddress;
        if (hubAddress != null) {
            this._hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new TimeSpan[] {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(9),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(120),
                    TimeSpan.FromSeconds(240)
                })
                .WithUrl(hubAddress)
                .Build();
            this._hubConnection.On<MonitorData>("ShowCurrent", this.OnShowCurrent);
            this._hubConnection.HandshakeTimeout = new TimeSpan(0, 0, 3);
            this._hubConnection.ServerTimeout = new TimeSpan(0, 0, 3);
            try {
                await this._hubConnection.StartAsync();
                this._logger.LogInformation(hubAddress + " Connection");
            } catch {
                this._logger.LogError(hubAddress+" hub connection failed");
            }
        }
    }

    async Task OnShowCurrent(MonitorData data) {
        List<int> tankWeights = new List<int>();

        for (int i = 0; i < 4; i++) {
            var tankWeight=data.analogData.FirstOrDefault(e => e.Item == "Tank"+(i+1)+" Weight");
            if (tankWeight != null) {
                string textValue = tankWeight.Value;
                textValue = textValue.Replace(",", string.Empty);
                try {
                    var weight = Convert.ToInt32(textValue);
                    tankWeights.Add(weight);
                } catch {
                    tankWeights.Add(0);
                    this._logger.LogInformation("Error: Exception converting tank weight to int");
                }
            } else {
                tankWeights.Add(0);
            }
        }
        await this._hubContext.Clients.All.SendTankWeights(tankWeights);
    }

    public async ValueTask DisposeAsync() {
        if (this._hubConnection != null) {
            await this._hubConnection.DisposeAsync();
        }
    }
}