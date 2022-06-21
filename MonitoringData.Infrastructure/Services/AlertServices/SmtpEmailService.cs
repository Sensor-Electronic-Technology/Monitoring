using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;

namespace MonitoringData.Infrastructure.Services.AlertServices;

public class SmtpEmailService:IEmailService {
    //private SmtpClient? _emailClient;
    private readonly ILogger<SmtpEmailService> _logger;
    private IMongoCollection<EmailRecipient> _recipientCollection;
    private MailboxAddress _from;
    private readonly MonitorEmailSettings _settings;

    private IEnumerable<MailboxAddress> _recipients;
    
    public SmtpEmailService(IOptions<MonitorEmailSettings> options,ILogger<SmtpEmailService> logger) {
        this._settings = options.Value;
        this._logger = logger;
        var client = new MongoClient(this._settings.ConnectionString);
        var database = client.GetDatabase(this._settings.DatabaseName);
        this._recipientCollection = database.GetCollection<EmailRecipient>(this._settings.EmailRecipientCollection);
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

    public async Task<bool> Load() {
        var recipients = await this._recipientCollection
            .Find(_ => true)
            .ToListAsync();
        if (recipients != null) {
            this._recipients = recipients.Select(e => new MailboxAddress(e.Username, e.Address));
            return true;
        }
        return false;
    }
}