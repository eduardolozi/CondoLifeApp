using System.Text.Json.Serialization;

namespace BlazorApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationResultEnum
{
    Approved,
    Cancelled,
    Info,
    Warning
}