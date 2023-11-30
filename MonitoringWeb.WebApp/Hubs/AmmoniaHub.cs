using Microsoft.AspNetCore.SignalR;
namespace MonitoringWeb.WebApp.Hubs;

public interface ISendTankWeightsCommand {
    Task SendTankWeights(List<int> tankWeights);
}

public class AmmoniaHub:Hub<ISendTankWeightsCommand> {
    public async Task SendTankWeights(List<int> tankWeights) {
        await this.Clients.All.SendTankWeights(tankWeights);
    }
}