using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using MonitoringData.Infrastructure.Data;

namespace MonitoringData.Infrastructure.Services.AlertServices;

public class SmtpEmailService:IEmailService {
    private readonly ILogger<SmtpEmailService> _logger;
    private MailboxAddress _from;
    private readonly MonitorEmailSettings _settings;
    private readonly DataLogConfigProvider _configProvider;

    private IEnumerable<MailboxAddress> _recipients;
    
    public SmtpEmailService(DataLogConfigProvider configProvider,
        IOptions<MonitorEmailSettings> settings,ILogger<SmtpEmailService> logger) {
        this._configProvider = configProvider;
        this._logger = logger;
        this._settings = settings.Value;
        this._from = new MailboxAddress(this._settings.FromUser,this._settings.FromAddress);
    }

    public async Task SendMessageAsync(string subject, string msg) {
        var client = new SmtpClient();
        try {
            await client.ConnectAsync(this._settings.SmtpHost, this._settings.SmtpPort,false);
            var message =new MimeMessage();
            message.From.Add(this._from);
            message.To.AddRange(this._recipients);
            BodyBuilder builder = new BodyBuilder() {
                HtmlBody = msg
            };
            message.Body = builder.ToMessageBody();
            message.Subject = subject;
            await client.SendAsync(message);
        } catch {
            this._logger.LogCritical("Error: Could not connect to smtp host");
        }
    }
    
    public void SendMessage(string subject, string msg) {
        throw new NotImplementedException();
    }
    
    public Task Load() {
        this._recipients = this._configProvider.EmailRecipients.Select(e => new MailboxAddress(e.Username, e.Address));
        return Task.CompletedTask;
    }
}