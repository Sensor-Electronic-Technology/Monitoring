﻿using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit.Utils;
using MonitoringData.Infrastructure.Data;

namespace MonitoringData.Infrastructure.Services.AlertServices;


public class SmtpEmailService:IEmailService {
    private readonly ILogger<SmtpEmailService> _logger;
    private MailboxAddress _from;
    private MailboxAddress _fromExternal;
    private readonly MonitorEmailSettings _settings;
    private readonly DataLogConfigProvider _configProvider;
    private IEnumerable<MailboxAddress> _recipients;
    
    public SmtpEmailService(DataLogConfigProvider configProvider,
        ILogger<SmtpEmailService> logger) {
        this._configProvider = configProvider;
        this._logger = logger;
        this._settings = configProvider.MonitorEmailSettings;
        this._from = new MailboxAddress(this._settings.FromUser,this._settings.FromAddress);
        this._fromExternal = new MailboxAddress(this._settings.ExternalFromUser, this._settings.ExternalFromAddress);
    }
    public async Task SendMessageAsync(string subject, IMessageBuilder messageBuilder) {
        var client = new SmtpClient();
        try {
                client.CheckCertificateRevocation = false;
                client.ServerCertificateValidationCallback = CertValidationCallback;
                await client.ConnectAsync(this._settings.SmtpHost, this._settings.SmtpPort,false);
                var message =new MimeMessage();
                message.From.Add(this._from);
                message.To.AddRange(this._recipients);
                BodyBuilder builder = new BodyBuilder();
                var bodyImage = await builder.LinkedResources.AddAsync("GasDetectorMap.png");
                bodyImage.ContentId = MimeUtils.GenerateMessageId();
                builder.HtmlBody=messageBuilder.FinishMessage(bodyImage.ContentId);
                message.Body = builder.ToMessageBody();
                message.Subject = subject;
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            //stream.Close();
        } catch(Exception e) {
            this._logger.LogCritical("Error: Could not connect to smtp host");
            this._logger.LogCritical("Message: {EMessage}", e.Message);
        }
    }

    public async Task SendExternalEmail(string subject,string gas,string currentValue,string units,string time) {
        var client = new SmtpClient();
        try {
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = CertValidationCallback;
            await client.ConnectAsync(this._settings.SmtpHost, this._settings.SmtpPort,false);
            var message =new MimeMessage();
            message.From.Add(new MailboxAddress("Andrew Elmendorf","aelmendorf@s-et.com"));
            //message.To.Add(new MailboxAddress("Ronnie Huffstetler","ronnie.huffstetler@airgas.com"));
            message.To.Add(new MailboxAddress("Andrew Gmail","aelmendorf234@gmail.com"));
            //message.Cc.Add(new MailboxAddress("Andrew Elmendorf", "aelmendorf@s-et.com"));
            message.Cc.Add(new MailboxAddress("Norman Culbertson","nculbertson@s-et.com"));
            message.Subject = subject;
            message.Body = new TextPart("plain") {
                Text = @$"
This is an automated message notifying AirGas that Sensor Electronic Technology’s {gas} tanks need a refill {time}

Current {gas} Value: {currentValue} {units}

Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

"
            };
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            //stream.Close();
        } catch(Exception e) {
            this._logger.LogCritical("Error: Could not connect to smtp host");
            this._logger.LogCritical("Message: {EMessage}", e.Message);
        }
    }
    
    private bool CertValidationCallback (object sender, 
        X509Certificate certificate, 
        X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;
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