using Microsoft.Exchange.WebServices.Data;
using MonitoringSystem.Shared.Data;
using Task = System.Threading.Tasks.Task;

namespace MonitoringData.Infrastructure.Services.AlertServices;

public class ExchangeEmailService {
    private ExchangeService _exchange;
    private readonly List<string> _toAddresses;
    private readonly List<string> _ccAddresses;
    private readonly DataLogConfigProvider _configProvider;

    public ExchangeEmailService(DataLogConfigProvider configProvider) {
        this._exchange = new ExchangeService(ExchangeVersion.Exchange2016);
        this._configProvider = configProvider;
        this._ccAddresses = this._configProvider.BulkEmailSettings.CcAddresses;
        this._toAddresses = this._configProvider.BulkEmailSettings.ToAddresses;
        WebCredentials credentials = new WebCredentials("facilityalerts", "Facility!1sskv", "sskep.com");
        this._exchange.Credentials = credentials;
        this._exchange.Url = new Uri(@"https://email.seoulsemicon.com/EWS/Exchange.asmx");
    }
    
    public async Task SendMessageAsync(string subject,string gas,string currentValue,string units,string time) {
        EmailMessage message = new EmailMessage(this._exchange);
        
        /*var recp = new List<string>() {
            "ronnie.huffstetler@airgas.com",
            "ANW.Planning.Group@airgas.com"
        };*/
        
        message.From = new EmailAddress("SETi Monitor Alerts", "setimonitoralerts@s-et.com");
        message.ToRecipients.AddRange(this._toAddresses);
        message.CcRecipients.AddRange(this._ccAddresses);
        /*message.CcRecipients.Add("nculbertson@s-et.com");
        message.CcRecipients.Add("aelmendorf@s-et.com");*/
        message.Subject = subject;
        MessageBody body = new MessageBody();
        body.BodyType = BodyType.Text;
        body.Text =@$"
This is an automated message notifying AirGas that Sensor Electronic Technology’s {gas} tanks need a refill {time}

Current {gas} Value: {currentValue} {units}

Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

";
        message.Body = body;
        await message.SendAndSaveCopy();
    }
    
    public async Task SendN2MessageAsync(string subject,string gas,string currentValue,string units,string time) {
        EmailMessage message = new EmailMessage(this._exchange);
        
        var recp = new List<string>() {
            "Ask.messer@messer-us.com",
            "adam.lane@messer-us.com"
        };
        
        message.From = new EmailAddress("SETi Monitor Alerts", "setimonitoralerts@s-et.com");
        //message.ToRecipients.AddRange(this._toAddresses);
        message.ToRecipients.AddRange(recp);
        message.CcRecipients.AddRange(this._ccAddresses);
        /*message.CcRecipients.Add("nculbertson@s-et.com");
        message.CcRecipients.Add("aelmendorf@s-et.com");*/
        message.Subject = subject;
        MessageBody body = new MessageBody();
        body.BodyType = BodyType.Text;
        body.Text =@$"
This is an automated message notifying Messer that Sensor Electronic Technology’s {gas} tanks need a refill {time}

Current {gas} Value: {currentValue} {units}

Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

";
        message.Body = body;
        await message.SendAndSaveCopy();
    }
    
    public async Task SendTestMessageAsync() {
        EmailMessage message = new EmailMessage(this._exchange);
        var recp = new List<string>() {
            "Ask.messer@messer-us.com",
            "adam.lane@messer-us.com"
        };
        message.From = new EmailAddress("SETi Monitor Alerts", "setimonitoralerts@s-et.com");
        message.ToRecipients.AddRange(recp);
        message.CcRecipients.AddRange(this._ccAddresses);
        message.Subject = "SETi Gas Notification Test";
        MessageBody body = new MessageBody();
        body.BodyType = BodyType.Text;
        body.Text =@$"
This is a test email from Sensor Electronic Technology.  We will be using this email address to send automated
gas refill notifications 

Please send the delivery schedule to Norman Culbertson at nculbertson@s-et.com

";
        message.Body = body;
        await message.SendAndSaveCopy();
    }
}