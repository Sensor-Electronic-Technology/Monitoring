﻿using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MonitoringData.Infrastructure.Data;

namespace MonitoringData.Infrastructure.Services.AlertServices;

public class SmtpEmailService:IEmailService {
    private readonly ILogger<SmtpEmailService> _logger;
    private MailboxAddress _from;
    private readonly MonitorEmailSettings _settings;
    private readonly DataLogConfigProvider _configProvider;

    private IEnumerable<MailboxAddress> _recipients;
    
    public SmtpEmailService(DataLogConfigProvider configProvider,
        ILogger<SmtpEmailService> logger) {
        this._configProvider = configProvider;
        this._logger = logger;
        this._settings = configProvider.MonitorEmailSettings;
        this._from = new MailboxAddress(this._settings.FromUser,this._settings.FromAddress);
    }
    public async Task SendMessageAsync(string subject, string msg) {
        var client = new SmtpClient();
       try {
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = CertValidationCallback;
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
            await client.DisconnectAsync(true);
        } catch(Exception e) {
           this._logger.LogCritical("Error: Could not connect to smtp host");
           this._logger.LogCritical($"Message: {e.Message}");
        }
    }
    
    public void SendMessage(string subject, string msg) {
        throw new NotImplementedException();
    }
    
    private bool CertValidationCallback (object sender, 
        X509Certificate certificate, 
        X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        // Note: The following code casts to an X509Certificate2 because it's easier to get the
        // values for comparison, but it's possible to get them from an X509Certificate as well.
        if (certificate is X509Certificate2 certificate2) {
            var cn = certificate2.GetNameInfo (X509NameType.SimpleName, false);
            var fingerprint = certificate2.Thumbprint;
            var serial = certificate2.SerialNumber;
            var issuer = certificate2.Issuer;
            Console.WriteLine($"Cert: {cn}");
            Console.WriteLine($"Fingerprint: {fingerprint}");
            Console.WriteLine($"Serial: {serial}");
            Console.WriteLine($"Issuer: {issuer}");
            /*return cn == "Exchange2016" && issuer == "CN=Exchange2016" &&
                   serial == "3D2E6FBDF9CE1FAF46D9CC68B8D58BAB" &&
                   fingerprint == "EC14ED8D2253824E6522D19EC815AD72CC767759";*/
            return true;
        }
        return true;
    }
    
    public Task Load() {
        this._recipients = this._configProvider.EmailRecipients.Select(e => new MailboxAddress(e.Username, e.Address));
        return Task.CompletedTask;
    }
}