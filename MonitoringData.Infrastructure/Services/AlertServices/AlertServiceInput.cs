using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services.AlertServices {
    public class AlertRecord {
        public int AlertId { get; set; }
        public int ChannelId { get; set; }
        public string DisplayName { get; set; }
        public ActionType CurrentState { get; set; }
        public AlertAction AlertAction { get; set; }
        public AlertItemType ItemType { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastAlert { get; set; }
        public bool Bypassed { get; set; }
        public DateTime TimeBypassed { get; set; }
        public int BypassResetTime { get; set; }
        public float ChannelReading { get; set; }

        public AlertRecord(MonitorAlert alert,ActionType reading) {
            this.CurrentState = reading;
            this.AlertId = alert._id;
            this.ChannelId = alert.channelId;
            this.DisplayName = alert.displayName;
            this.CurrentState = reading;
            this.Enabled = alert.enabled;
            this.AlertAction = AlertAction.Clear;
            this.ItemType = alert.itemType;
            this.ChannelReading = 0.00f;
        }

        public AlertRecord(MonitorAlert alert, float reading, ActionType state) {
            this.CurrentState = state;
            this.AlertId = alert._id;
            this.ChannelId = alert.channelId;
            this.DisplayName = alert.displayName;
            this.Enabled = alert.enabled;
            this.AlertAction = AlertAction.Clear;
            this.ItemType = alert.itemType;
            this.ChannelReading = reading;
        }

        public AlertRecord() {
            this.ChannelReading = 0.00f;
            this.AlertId = -1;
            this.ChannelId = -1;
            this.DisplayName = "Not Set";
            this.CurrentState = ActionType.Okay;
            this.Enabled = false;
            this.AlertAction = AlertAction.Clear;
            this.ItemType = AlertItemType.Discrete;
        }

        public AlertRecord Clone() {
            AlertRecord other = (AlertRecord)this.MemberwiseClone();
            return other;
        }
    }
}
