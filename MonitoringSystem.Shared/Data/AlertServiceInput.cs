namespace MonitoringSystem.Shared.Data {
    public enum AlertAction {
        Clear,
        ChangeState,
        Start,
        Resend,
        Nothing
    }
    public class AlertRecord {
        public string AlertId { get; set; }
        public string ChannelId { get; set; }
        public string DisplayName { get; set; }
        public ActionType CurrentState { get; set; }
        public AlertAction AlertAction { get; set; }
        public AlertItemType ItemType { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastAlert { get; set; }
        public bool Bypassed { get; set; }
        public bool Latched { get; set; }
        public DateTime TimeLatched { get; set; }
        public DateTime TimeBypassed { get; set; }
        public int BypassResetTime { get; set; }
        public float ChannelReading { get; set; }

        public AlertRecord(MonitorAlert alert,ActionType reading) {
            this.CurrentState = reading;
            this.AlertId = alert._id.ToString();
            this.ChannelId = alert.channelId.ToString();
            this.DisplayName = alert.displayName;
            this.CurrentState = reading;
            this.Enabled = alert.enabled;
            this.AlertAction = AlertAction.Nothing;
            this.ItemType = alert.itemType;
            this.ChannelReading = 0.00f;
            this.Latched = false;
        }

        public AlertRecord(MonitorAlert alert, float reading, ActionType state) {
            this.CurrentState = state;
            this.AlertId = alert._id.ToString();
            this.ChannelId = alert.channelId.ToString();
            this.DisplayName = alert.displayName;
            this.Enabled = alert.enabled;
            this.AlertAction = AlertAction.Nothing;
            this.ItemType = alert.itemType;
            this.ChannelReading = reading;
            this.Latched = false;
        }

        public AlertRecord() {
            this.ChannelReading = 0.00f;
            this.AlertId = "-1";
            this.ChannelId = "-1";
            this.DisplayName = "Not Set";
            this.CurrentState = ActionType.Okay;
            this.Enabled = false;
            this.AlertAction = AlertAction.Nothing;
            this.ItemType = AlertItemType.Discrete;
            this.Latched = false;
        }

        public AlertRecord Clone() {
            AlertRecord other = (AlertRecord)this.MemberwiseClone();
            return other;
        }
    }
}
