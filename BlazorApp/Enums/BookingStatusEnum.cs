﻿using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BlazorApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatusEnum
{
    [Description("Aguardando pagamento")]
    AwaitingPayment,
    [Description("Aprovação pendente")]
    Pending,
    [Description("Aprovado")]
    Confirmed,
    [Description("Rejeitado")]
    Canceled
}