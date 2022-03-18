using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IMessageBuilder {
        void StartMessage();
        void AppendAlert(string channelName, string alertType, string value);
        void AppendStatus(string channelName, string value);
        string FinishMessage();
    }

    public class MessageBuilder : IMessageBuilder {
        private StringBuilder _alertBuilder, _statusBuilder;
        public void StartMessage() {
            this._alertBuilder = new StringBuilder();
            this._statusBuilder = new StringBuilder();
            this._alertBuilder.AppendLine("<head>");
            this._alertBuilder.AppendLine("<style>");
            this._alertBuilder.AppendLine("table, th, td {");
            this._alertBuilder.AppendLine("border: 1px solid black;");
            this._alertBuilder.AppendLine("border-collapse: collapse;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("th, td {");
            this._alertBuilder.AppendLine("padding: 15px;");
            this._alertBuilder.AppendLine("text-align:center;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 tr:nth-child(even) {");
            this._alertBuilder.AppendLine("background-color: #eee;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 tr:nth-child(odd) {");
            this._alertBuilder.AppendLine("background-color: #B4AFAF;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 th{");
            this._alertBuilder.AppendLine("background-color: #A01C00;");
            this._alertBuilder.AppendLine("color:white;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("</style>");
            this._alertBuilder.AppendLine("</head>");

            this._alertBuilder.AppendLine("<body>");
            this._alertBuilder.AppendLine("<h2>Facility Alerts</h2>");

            this._alertBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._alertBuilder.AppendLine("<tr>");
            this._alertBuilder.AppendLine("<th>Item</th>");
            this._alertBuilder.AppendLine("<th>Alert Type</th>");
            this._alertBuilder.AppendLine("<th>Value</th>");
            this._alertBuilder.AppendLine("</tr>");

            this._statusBuilder.AppendLine();
            this._statusBuilder.AppendLine();
            this._statusBuilder.AppendLine("<h2>----Facility Status----</h2>");
            this._statusBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._statusBuilder.AppendLine("<tr>");
            this._statusBuilder.AppendLine("<th>Item</th>");
            this._statusBuilder.AppendLine("<th>Value</th>");
            this._statusBuilder.AppendLine("</tr>");
        }

        public string FinishMessage() {
            this._alertBuilder.AppendLine("</table>");
            this._alertBuilder.AppendLine("<h2>----End Alerts----</h2>");
            this._statusBuilder.AppendLine("</table>");
            this._statusBuilder.AppendLine("<h2>----End Status----</h2>");

            this._alertBuilder.Append(this._statusBuilder.ToString());
            this._alertBuilder.AppendLine("</body>");
            return this._alertBuilder.ToString();
        }

        public void AppendAlert(string channelName, string alertType, string value) {
            this._alertBuilder.AppendLine("<tr>");
            this._alertBuilder.AppendFormat("<td>{0}</td>", channelName).AppendLine();
            this._alertBuilder.AppendFormat("<td>{0}</td>", alertType).AppendLine();
            this._alertBuilder.AppendFormat("<td>{0}</td>", value).AppendLine();
            this._alertBuilder.AppendLine("</tr>");
        }

        public void AppendStatus(string channelName, string value) {
            this._statusBuilder.AppendLine("<tr>");
            this._statusBuilder.AppendFormat("<td>{0}</td>", channelName).AppendLine();
            this._statusBuilder.AppendFormat("<td>{0}</td>", value).AppendLine();
            this._statusBuilder.AppendLine("</tr>");
        }
    }
}
