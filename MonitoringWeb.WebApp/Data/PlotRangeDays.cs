namespace MonitoringWeb.WebApp.Data; 

public enum PlotRangeDays {
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

public enum PlotRangeMonths {
    _3M=3,
    _6M=6,
    _9M=9,
    _12M=12,
    _15M=15,
    _18M=18,
    _21M=21,
    _24M=24
}

public enum PlotRangeWeeks {
    
}


public enum GroupPlotBy {
    Months,
    Weeks,
    Days
}


public static class EnumExtension {
    public static IEnumerable<T> ToEnumerableOf<T>(this Enum theEnum)
    {
        return Enum.GetValues(theEnum.GetType()).Cast<T>().ToList();
    }
}

