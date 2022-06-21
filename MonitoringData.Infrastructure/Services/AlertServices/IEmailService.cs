using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IEmailService {
        Task SendMessageAsync(string subject,string msg);
        void SendMessage(string subject,string msg);
    }
    public class EmailService : IEmailService {
        private ExchangeService _exchange;

        public EmailService() {
            this._exchange = new ExchangeService(ExchangeVersion.Exchange2016);
            //WebCredentials credentials = new WebCredentials("facilityalerts", "Facility!1sskv", "sskep.com");
            WebCredentials credentials = new WebCredentials("600076", "Drizzle3219753!", "sskep.com");
            this._exchange.Credentials = credentials;
            this._exchange.Url = new Uri(@"https://email.seoulsemicon.com/EWS/Exchange.asmx");
        }

        public void SendMessage(string subject,string msg) {
            EmailMessage message = new EmailMessage(this._exchange);
            var recp = new List<string>() {
                "aelmendorf@s-et.com",
                "rakesh@s-et.com",
                "bmurdaugh@s-et.com",
                "achapman@s-et.com"
            };
            /*var recp = new List<string>() {
                "aelmendorf@s-et.com"
            };*/
            message.ToRecipients.AddRange(recp);
            message.Subject = subject;
            MessageBody body = new MessageBody();
            body.BodyType = BodyType.HTML;
            body.Text = msg;
            message.Body = body;
            message.Send();
        }

        public async Task SendMessageAsync(string subject,string msg) {
            EmailMessage message = new EmailMessage(this._exchange);
            var recp = new List<string>() {
                "aelmendorf@s-et.com",
                "rakesh@s-et.com",
                "bmurdaugh@s-et.com",
                "achapman@s-et.com"
            };
            /*var recp = new List<string>() {
                "aelmendorf@s-et.com",
            };*/
            message.ToRecipients.AddRange(recp);
            message.Subject = subject;
            MessageBody body = new MessageBody();
            body.BodyType = BodyType.HTML;
            body.Text = msg;
            
            message.Body = body;
            await message.Send();
        }
    }
}
