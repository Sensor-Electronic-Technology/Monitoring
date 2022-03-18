using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IEmailService {
        Task SendMessageAsync(string msg);
        void SendMessage(string msg);
    }
    public class EmailService : IEmailService {
        private ExchangeService _exchange;

        public EmailService() {
            this._exchange = new ExchangeService(ExchangeVersion.Exchange2016);
            WebCredentials credentials = new WebCredentials("facilityalerts", "Facility!1sskv", "sskep.com");
            this._exchange.Credentials = credentials;
            this._exchange.Url = new Uri(@"https://email.seoulsemicon.com/EWS/Exchange.asmx");
        }

        public void SendMessage(string msg) {
            EmailMessage message = new EmailMessage(this._exchange);
            var recp = new List<string>() {
                "aelmendorf@s-et.com",
                "rakesh@s-et.com",
                "bmurdaugh@s-et.com",
                "achapman@s-et.com"
            };
            message.ToRecipients.AddRange(recp);
            MessageBody body = new MessageBody();
            body.BodyType = BodyType.HTML;
            body.Text = "Monitoring V2 Test";
            message.Body = body;
            message.Send();
        }

        public async Task SendMessageAsync(string msg) {
            EmailMessage message = new EmailMessage(this._exchange);
            var recp = new List<string>() {
                "aelmendorf@s-et.com",
                "rakesh@s-et.com",
                "bmurdaugh@s-et.com",
                "achapman@s-et.com"
            };
            message.ToRecipients.AddRange(recp);
            MessageBody body = new MessageBody();
            body.BodyType = BodyType.HTML;
            body.Text = msg;
            message.Body = body;
            await message.SendAndSaveCopy();
        }
    }
}
