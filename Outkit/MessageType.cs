using System.Text.Json.Serialization;

namespace Outkit
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageType
    {
        // ReSharper disable once InconsistentNaming
        sms,
        // ReSharper disable once InconsistentNaming
        email
    }

}
