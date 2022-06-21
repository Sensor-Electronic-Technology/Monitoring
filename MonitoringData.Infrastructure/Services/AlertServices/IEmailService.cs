using Task = System.Threading.Tasks.Task;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IEmailService {
        Task SendMessageAsync(string subject,string msg);
        void SendMessage(string subject,string msg);
        Task<bool> Load();
    }
}
