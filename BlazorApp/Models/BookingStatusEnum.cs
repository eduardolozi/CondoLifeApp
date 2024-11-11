using System.Text.Json.Serialization;

namespace BlazorApp.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatusEnum
{
    AwaitingPayment,
    Pending,
    Confirmed,
    Canceled
}