using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public interface IMessageBuilder {
        void StartMessage(string device);
        void AppendAlert(string channelName, string state, string value);
        void AppendStatus(string channelName,string state ,string value);
        void AppendChanged(string channelName,string state, string value);
        string FinishMessage();
    }

    public class MessageBuilder : IMessageBuilder {
        private StringBuilder _bodyBuilder,_alertBuilder, _statusBuilder,_changeStatusBuilder;
        private bool displayChanged = false;
        public void StartMessage(string device) {
            this._alertBuilder = new StringBuilder();
            this._statusBuilder = new StringBuilder();
            this._changeStatusBuilder = new StringBuilder();
            this._bodyBuilder = new StringBuilder();
            this.displayChanged = false;

            this._bodyBuilder.AppendLine("<head>");
            this._bodyBuilder.AppendLine("<style>");
            this._bodyBuilder.AppendLine("table, th, td {");
            this._bodyBuilder.AppendLine("border: 1px solid black;");
            this._bodyBuilder.AppendLine("border-collapse: collapse;");
            this._bodyBuilder.AppendLine("}");

            this._bodyBuilder.AppendLine("th, td {");
            this._bodyBuilder.AppendLine("padding: 15px;");
            this._bodyBuilder.AppendLine("text-align:center;");
            this._bodyBuilder.AppendLine("}");

            this._bodyBuilder.AppendLine("table#t01 tr:nth-child(even) {");
            this._bodyBuilder.AppendLine("background-color: #eee;");
            this._bodyBuilder.AppendLine("}");

            this._bodyBuilder.AppendLine("table#t01 tr:nth-child(odd) {");
            this._bodyBuilder.AppendLine("background-color: #B4AFAF;");
            this._bodyBuilder.AppendLine("}");

            this._bodyBuilder.AppendLine("table#t01 th{");
            this._bodyBuilder.AppendLine("background-color: #A01C00;");
            this._bodyBuilder.AppendLine("color:white;");
            this._bodyBuilder.AppendLine("}");

            this._bodyBuilder.AppendLine("</style>");
            this._bodyBuilder.AppendLine("</head>");
            this._bodyBuilder.AppendLine("<body>");

            this._alertBuilder.AppendLine($"<h2>{device} Alerts</h2>");
            this._alertBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._alertBuilder.AppendLine("<tr>");
            this._alertBuilder.AppendLine("<th>Item</th>");
            this._alertBuilder.AppendLine("<th>State</th>");
            this._alertBuilder.AppendLine("<th>Value</th>");
            this._alertBuilder.AppendLine("</tr>");

            this._changeStatusBuilder.AppendLine("<h2>----Alerts Changed----</h2>");
            this._changeStatusBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._changeStatusBuilder.AppendLine("<tr>");
            this._changeStatusBuilder.AppendLine("<th>Item</th>");
            this._changeStatusBuilder.AppendLine("<th>State</th>");
            this._changeStatusBuilder.AppendLine("<th>Value</th>");
            this._changeStatusBuilder.AppendLine("</tr>");

            this._statusBuilder.AppendLine();
            this._statusBuilder.AppendLine($"<h2>----{device} Status----</h2>");
            this._statusBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._statusBuilder.AppendLine("<tr>");
            this._statusBuilder.AppendLine("<th>Item</th>");
            this._statusBuilder.AppendLine("<th>State</th>");
            this._statusBuilder.AppendLine("<th>Value</th>");
            this._statusBuilder.AppendLine("</tr>");
        }

        public string FinishMessage() {
            this._alertBuilder.AppendLine("</table>");
            this._alertBuilder.AppendLine("<h2>----End Alerts----</h2>");
            this._statusBuilder.AppendLine("</table>");
            this._statusBuilder.AppendLine("<h2>----End Status----</h2>");
            this._changeStatusBuilder.AppendLine("</table>");
            this._changeStatusBuilder.AppendLine("<h2>----End Changed Alerts----</h2>");

            if (this.displayChanged) {
                this._bodyBuilder.Append(this._changeStatusBuilder.ToString());
            }
            this._bodyBuilder.Append(this._alertBuilder.ToString());
            this._bodyBuilder.Append(this._statusBuilder.ToString());
            this._bodyBuilder.AppendLine("</body>");
            //this._alertBuilder.Append(this._statusBuilder.ToString());

            return this._bodyBuilder.ToString();
        }

        public void AppendAlert(string channelName, string state, string value) {
            this._alertBuilder.AppendLine("<tr>");
            this._alertBuilder.AppendFormat("<td>{0}</td>", channelName).AppendLine();
            this._alertBuilder.AppendFormat("<td>{0}</td>", state).AppendLine();
            this._alertBuilder.AppendFormat("<td>{0}</td>", value).AppendLine();
            this._alertBuilder.AppendLine("</tr>");
        }

        public void AppendStatus(string channelName,string state, string value) {
            this._statusBuilder.AppendLine("<tr>");
            this._statusBuilder.AppendFormat("<td>{0}</td>", channelName).AppendLine();
            this._statusBuilder.AppendFormat("<td>{0}</td>", state).AppendLine();
            this._statusBuilder.AppendFormat("<td>{0}</td>", value).AppendLine();
            this._statusBuilder.AppendLine("</tr>");
        }

        public void AppendChanged(string channelName,string state,string value) {
            this.displayChanged = false;
            this._changeStatusBuilder.AppendLine("<tr>");
            this._changeStatusBuilder.AppendFormat("<td>{0}</td>", channelName).AppendLine();
            this._changeStatusBuilder.AppendFormat("<td>{0}</td>", state).AppendLine();
            this._changeStatusBuilder.AppendFormat("<td>{0}</td>", value).AppendLine();
            this._changeStatusBuilder.AppendLine("</tr>");

        }
    }
}
