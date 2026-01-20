namespace MonitoringSystem.Shared.Extensions;

public static class CollectionExtensions {
    public static IEnumerable<T> GetNth<T>(this List<T> list, int n) {
        n = (n == 0) ? n = 1 : n;
        for (int i=0; i<list.Count; i+=n)
            yield return list[i];
    }
    
    public static T GetPropertyValue<T>(this Object obj, string propertyName,T defaultValue=default(T)) {
        return (T)obj.GetType().GetProperty(propertyName)?.GetValue(obj, null) ?? defaultValue;
    }
}