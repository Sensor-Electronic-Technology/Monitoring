using System.Linq.Expressions;

namespace MonitoringSystem.Shared.Extensions;

public static class ListExtensions {

}

/*public static Task<TNum[]> Filter<T,TNum>(this IEnumerable<T> list,Expression<Func<T,TNum>> selector,int windowSize=5) where T : class {
    var arr=list.Select(selector.Compile()).ToArray();
    Span<TNum> arr.AsSpan();
    for (int i = 0; i < list.Count; i+=windowSize) {
        var top=i+windowSize;
        var winAvg = list[i..top].Select(selector.Compile()).Average();
    }
}
}*/