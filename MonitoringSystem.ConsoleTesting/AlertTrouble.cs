using MailKit.Net.Smtp;
using MimeKit;
using MongoDB.Bson;
using MonitoringData.Infrastructure.Data;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
namespace MonitoringSystem.ConsoleTesting;

public class AlertTrouble {
    static async Task Main(string[] args) {
        await TestExternalAlertEmail();
    }
    
    static async Task TestExternalAlertEmail() {
        Thread alertThread=new Thread(new ParameterizedThreadStart(AlertThread));
        Console.WriteLine("Press any key to start the alert thread");
        Console.ReadKey();
        CancellationTokenSource cts=new CancellationTokenSource();
        alertThread.Start(cts.Token);
        Console.WriteLine("Press any key to stop the alert thread");
        Console.ReadKey();
        cts.Cancel();
        alertThread.Join();
        Console.WriteLine("Goodbye!");

    }
    
    static void AlertThread(object? obj){
        if(obj is CancellationToken token){
            List<AlertRecord> activeAlerts = new List<AlertRecord>();
            var h2Alert= new AlertRecord() {
                AlertId = ObjectId.GenerateNewId(),
                Bypassed = false,
                Enabled = true,
                CurrentState = ActionType.Okay,
                DisplayName = "Bulk H2(PSI)",
                ChannelReading = 750,
                Latched = false
            };
            var n2Alert= new AlertRecord() {
                AlertId = ObjectId.GenerateNewId(),
                Bypassed = false,
                Enabled = true,
                CurrentState = ActionType.Okay,
                DisplayName = "Bulk N2(inH20)",
                ChannelReading = 100.0f,
                Latched = false
            };
            bool time = false;
            DateTime start = DateTime.Now;
            while (!token.IsCancellationRequested) {
                if (h2Alert.ChannelReading <= 550) {
                    h2Alert.CurrentState = ActionType.Alarm;
                }else if (h2Alert.ChannelReading <= 750) {
                    h2Alert.CurrentState = ActionType.Warning;
                }else if (h2Alert.ChannelReading <= 1400) {
                    h2Alert.CurrentState = ActionType.SoftWarn;
                }else {
                    h2Alert.CurrentState = ActionType.Okay;
                }
            
                List<AlertRecord> alerts = new List<AlertRecord>() {
                    h2Alert,
                    n2Alert
                };
                activeAlerts=ProcessAlerts(alerts, activeAlerts, DateTime.Now).GetAwaiter().GetResult();
                Thread.Sleep(500);
                DateTime now=DateTime.Now;
                if((now-start).Seconds>=2) {
                    start = now;
                    if (h2Alert.ChannelReading >= 755) {
                        h2Alert.ChannelReading=745;
                    } else {
                        h2Alert.ChannelReading=755;
                    }
                    
                }
                Console.WriteLine($"H2: {h2Alert.ChannelReading} PSI");
            }
        } else {
            Console.WriteLine("Error: Thread did not start.  Object was not a CancellationToken.");
        }
    }

    public static async Task<List<AlertRecord>> ProcessAlerts(List<AlertRecord> alerts,List<AlertRecord> activeAlerts,DateTime now) {
        bool sendEmail = false;
        bool sendExEmail = false;
        foreach (var alert in alerts) {
            if (alert.Enabled) {
                var activeAlert = activeAlerts.FirstOrDefault(e => e.AlertId == alert.AlertId);
                switch (alert.CurrentState) {
                    case ActionType.Okay: {
                            if (activeAlert != null) {
                                if (!activeAlert.Latched) {
                                    activeAlert.Latched = true;
                                    activeAlert.TimeLatched = now;
                                } else {
                                    if (activeAlert.DisplayName == "Bulk H2(PSI)" || activeAlert.DisplayName == "Bulk N2(inH20)") {
                                        if ((now - activeAlert.TimeLatched).TotalMinutes >= 5) {
                                            activeAlerts.Remove(activeAlert);
                                        }
                                    } else {
                                        if ((now - activeAlert.TimeLatched).TotalSeconds >= 60) {
                                            activeAlerts.Remove(activeAlert);
                                        }
                                    }
                                }
                            }//else do nothing, there is no activeAlert
                            break;
                        }
                    case ActionType.Alarm:
                    case ActionType.Warning:
                        case ActionType.SoftWarn: {
                                if (activeAlert != null) {
                                    if (activeAlert.CurrentState != alert.CurrentState) {
                                        if (!activeAlert.AlertLatched) {
                                            activeAlert.AlertLatched = true;
                                            activeAlert.TimeAlertLatched = now;
                                        } else {
                                            if ((now - activeAlert.TimeAlertLatched).TotalSeconds >=2) {
                                                activeAlert.AlertLatched = false;
                                                activeAlert.CurrentState = alert.CurrentState;
                                                activeAlert.ChannelReading = alert.ChannelReading;
                                                activeAlert.AlertAction = alert.AlertAction;
                                                activeAlert.LastAlert = now;
                                                if (activeAlert.DisplayName == "Bulk H2(PSI)" || activeAlert.DisplayName == "Bulk N2(inH20)") {
                                                    sendExEmail = activeAlert.CurrentState<alert.CurrentState;
                                                    sendEmail = activeAlert.CurrentState<alert.CurrentState;
                                                } else {
                                                    sendEmail = true;
                                                }
                                            }
                                        }

                                    } else {
                                        if (activeAlert.Latched) {
                                            activeAlert.Latched = false;
                                            activeAlert.TimeLatched = now;
                                        }
                                        var emailPeriod = 30;
                                        if ((now - activeAlert.LastAlert).TotalMinutes >= emailPeriod) {
                                            activeAlert.LastAlert = now;
                                            activeAlert.ChannelReading = alert.ChannelReading;
                                            sendEmail = true;
                                        } else {
                                            activeAlert.ChannelReading = alert.ChannelReading;
                                        }
                                    }
                                } else {
                                    alert.LastAlert = now;
                                    activeAlerts.Add(alert.Clone());
                                    sendEmail = true;
                                    sendExEmail = true;
                                }
                                break;
                            }
                    case ActionType.Maintenance:
                    case ActionType.Custom:
                    default:
                        //do nothing on maintenance,custom
                        break;
                }
            }
        }
        if (activeAlerts.Any()) {
            if (sendEmail) {
                if (sendExEmail) {
                    var bulkH2 = activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk H2(PSI)");
                    var bulkN2 = activeAlerts.FirstOrDefault(e => e.DisplayName == "Bulk N2(inH20)");
                    await ProcessBulkGasExternalEmail(bulkN2, bulkH2,now);
                    Console.WriteLine("Bulk External Email Sent");
                } else {
                    Console.WriteLine("Internal Email Sent");
                }
            }

        }
        return activeAlerts;
    }

    private static async Task ProcessBulkGasExternalEmail(AlertRecord? bulkN2,AlertRecord? bulkH2,DateTime now) {
            if (bulkN2 != null) {
                if (bulkN2.LastAlert == now) {
                    switch (bulkN2.CurrentState) {
                        case ActionType.Alarm: {
                            /*await this._externalEmailService.SendN2MessageAsync("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "Immediately");*/
                            
                            break;
                        }
                        case ActionType.Warning: {
                            /*await this._externalEmailService.SendN2MessageAsync("Nitrogen EMERGENCY Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "within the next 8 Hrs");*/
                            break;
                        }
                        case ActionType.SoftWarn: {
                            /*await this._externalEmailService.SendN2MessageAsync("Nitrogen Gas Refill Request", 
                                "Nitrogen",
                                bulkN2.ChannelReading.ToString(CultureInfo.InvariantCulture),"inH2O", 
                                "within the next 24 Hrs");*/
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
            if (bulkH2 != null) {
                if (bulkH2.LastAlert == now) {
                    switch (bulkH2.CurrentState) {
                        case ActionType.Alarm: {
                            /*await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "Immediately");*/
                            Console.WriteLine("External H2 ALARM Email Sent");
                            break;
                        }
                        case ActionType.Warning: {
                            /*await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas EMERGENCY Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 24Hrs");*/
                            Console.WriteLine("External H2 WARN Email Sent");
                            break;
                        }
                        case ActionType.SoftWarn: {
                            /*await this._externalEmailService.SendH2MessageAsync("Hydrogen Gas Refill Request", 
                                "Hydrogen",
                                bulkH2.ChannelReading.ToString(CultureInfo.InvariantCulture),"PSI", 
                                "within the next 48 Hrs");*/
                            Console.WriteLine("External H2 SOFTWARN Email Sent");
                            break;
                        }
                        default: break;//do nothing
                    }
                }
            }
    }

    static bool MyServerCertificateValidationCallback (object sender, 
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
        return false;
    }

    static async Task SendExternalEmail(string subject,string gas,string currentValue,string units,string time) {
        SmtpClient client = new SmtpClient();
        client.CheckCertificateRevocation = false;
        client.ServerCertificateValidationCallback = MyServerCertificateValidationCallback;
        await client.ConnectAsync("10.92.3.215",25,false);
        MimeMessage mailMessage = new MimeMessage();
        BodyBuilder bodyBuilder = new BodyBuilder();
        mailMessage.To.Add(new MailboxAddress("Andrew Elmendorf","aelmendorf@s-et.com"));
        mailMessage.To.Add(new MailboxAddress("Norman Culbertson","nculbertson@s-et.com"));
        mailMessage.To.Add(new MailboxAddress("Rakesh Jain","rakesh@s-et.com"));
        mailMessage.From.Add(new MailboxAddress("Monitor Alerts","monitoring@s-et.com"));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("plain") {
                            Text = @$"
        This is an automated message notifying AirGas that Sensor Electronic Technology’s {gas} tanks need a refill {time}

        Current {gas} Value: {currentValue} {units}

        Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

        "
        };
        await client.SendAsync(mailMessage);
        await client.DisconnectAsync(true);
        Console.WriteLine("Check Mail");
    }
}