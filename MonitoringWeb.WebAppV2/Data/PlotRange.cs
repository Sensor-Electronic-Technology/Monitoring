namespace MonitoringWeb.WebAppV2.Data; 

public enum PlotRange {
    _1D=1,
    _2D=2,
    _3D=3,
    _5D=5,
    _1W=7,
    _2W=14,
    _1M=30,
    _2M=60,
    _6M=180,
    _1Y=365
}

public static class EnumExtension {
    public static IEnumerable<T> ToEnumerableOf<T>(this Enum theEnum)
    {
        return Enum.GetValues(theEnum.GetType()).Cast<T>().ToList();
    }
}

