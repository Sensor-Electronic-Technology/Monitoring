using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MonitoringSystem.ConsoleTesting;

public partial class DiscreteJsonChannel
{
    [JsonProperty("Input")]
    public int Input { get; set; }

    [JsonProperty("Address")]
    public Address Address { get; set; }

    [JsonProperty("MRI")]
    public Mra MRI { get; set; }

    [JsonProperty("MRA")]
    public Mra MRA { get; set; }

    [JsonProperty("Connected")]
    public int Connected { get; set; }

    [JsonProperty("Alert")]
    public JsonAlert Alert { get; set; }
}

public partial class Address
{
    [JsonProperty("Channel")]
    public int Channel { get; set; }

    [JsonProperty("Slot")]
    public int Slot { get; set; }
}

public partial class JsonAlert
{
    [JsonProperty("TriggerOn")]
    public int TriggerOn { get; set; }

    [JsonProperty("Action")]
    public int Action { get; set; }

    [JsonProperty("ActionType")]
    public int ActionType { get; set; }

    [JsonProperty("Enabled")]
    public int Enabled { get; set; }
}

public partial class Mra
{
    [JsonProperty("Register")]
    public int Register { get; set; }

    [JsonProperty("Type")]
    public int Type { get; set; }
}

public partial class DiscreteJsonChannel
{
    public static DiscreteJsonChannel FromJson(string json) => JsonConvert.DeserializeObject<DiscreteJsonChannel>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this DiscreteJsonChannel self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Formatting = Formatting.Indented,
        Converters =
        {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
        
    };
}
