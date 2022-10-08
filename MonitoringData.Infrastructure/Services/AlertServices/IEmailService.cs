using Task = System.Threading.Tasks.Task;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IEmailService {
        Task SendMessageAsync(string subject, IMessageBuilder messageBuilder);
        Task Load();
    }
}
