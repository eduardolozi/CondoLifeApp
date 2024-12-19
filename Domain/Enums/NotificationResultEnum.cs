using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationResultEnum
{
    Approved,
    Cancelled,
    Info,
    Warning
}