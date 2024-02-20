using MassTransit.Futures.Contracts;
using MongoDB.Bson;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringData.Infrastructure.Data;
public enum AlertAction {
    Clear,
    ChangeState,
    Start,
    Resend,
    Nothing
}
public class AlertRecord {
    public ObjectId AlertId { get; set; }
    public ObjectId ChannelId { get; set; }
    public bool Display { get; set; }
    public string DisplayName { get; set; }
    public ActionType CurrentState { get; set; }
    public AlertAction AlertAction { get; set; }
    public AlertItemType ItemType { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastAlert { get; set; }
    public bool Bypassed { get; set; }
    public bool Latched { get; set; }
    public bool AlertLatched { get; set; }
    public DateTime TimeLatched { get; set; }
    public DateTime TimeAlertLatched { get; set; }
    public DateTime TimeBypassed { get; set; }
    public int BypassResetTime { get; set; }
    public float ChannelReading { get; set; }

    public AlertRecord(MonitorAlert alert,ActionType reading) {
        this.CurrentState = reading;
        this.AlertId = alert._id;
        this.ChannelId = alert.ChannelId;
        this.DisplayName = alert.DisplayName;
        this.CurrentState = reading;
        this.Enabled = alert.Enabled;
        this.AlertAction = AlertAction.Nothing;
        this.ItemType = alert.AlertItemType;
        this.ChannelReading = 0.00f;
        this.Latched = false;
        this.Display = alert.Display;
        this.AlertLatched = false;
    }

    public AlertRecord(MonitorAlert alert, float reading, ActionType state) {
        this.CurrentState = state;
        this.AlertId = alert._id;
        this.ChannelId = alert.ChannelId;
        this.DisplayName = alert.DisplayName;
        this.Enabled = alert.Enabled;
        this.AlertAction = AlertAction.Nothing;
        this.ItemType = alert.AlertItemType;
        this.ChannelReading = reading;
        this.Latched = false;
        this.Display = alert.Display;
        this.AlertLatched = false;
    }

    public AlertRecord() {
        this.ChannelReading = 0.00f;
        this.AlertId=ObjectId.Empty;
        this.ChannelId = ObjectId.Empty;
        this.DisplayName = "Not Set";
        this.CurrentState = ActionType.Okay;
        this.Enabled = false;
        this.AlertAction = AlertAction.Nothing;
        this.ItemType = AlertItemType.Discrete;
        this.Latched = false;
        this.Display = false;
        this.AlertLatched = false;
    }

    public AlertRecord Clone() {
        AlertRecord other = (AlertRecord)this.MemberwiseClone();
        return other;
    }
}
