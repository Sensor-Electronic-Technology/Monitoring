namespace MonitoringSystem.ConfigApi.Contracts.Requests.Get;

public class GetAnalogAlertRequest {
    public Guid AnalogChannelId { get; set; }
}

public class GetDiscreteAlertRequest {
    public Guid DiscreteChannelId { get; set; }
}

public class GetAnalogLevelsRequest {
    public Guid AnalogAlertId { get; set; }
}

public class GetDiscreteLevelRequest {
    public Guid DiscreteAlertId { get; set; }
}