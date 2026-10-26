using System.Linq.Expressions;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services;

public static class Extensions {
    public static IEnumerable<AnalogReadingDto> Filter(this List<AnalogReadingDto> list,Expression<Func<AnalogReadingDto,double>> selector, int windowSize = 5,
        double percentThreshold = 0.9) {
        
        for (int i = 0; i < list.Count; i++) {
            var end = Math.Min(list.Count-1, i + windowSize);
            var start=Math.Max(0,i-windowSize);
            int count = end - start + 1;
            
            var winAvg = list.Skip(start).Take(count).Average(selector.Compile());
            
            var threshold = winAvg * percentThreshold;
            if (list[i].Value>=threshold) {
                yield return list[i];
            }/* else {
                yield return new AnalogReadingDto() {
                    Value = list[i].Value + (threshold-list[i].Value),
                    Name = list[i].Name,
                    Time = list[i].Time,
                    TimeStamp = list[i].TimeStamp,
                };
            }*/
        }
    }
}