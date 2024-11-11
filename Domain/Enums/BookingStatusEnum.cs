﻿using System.Text.Json.Serialization;

namespace Domain.Enums {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookingStatusEnum {
        AwaitingPayment,
        Pending,
        Confirmed,
        Canceled
    }
}
