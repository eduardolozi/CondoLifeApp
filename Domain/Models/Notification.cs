﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Models {
    public class Notification {
        public int Id { get; set; }
        [NotMapped] public string UserToken { get; set; }
        public string? CondominiumName { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public NotificationPayload Message { get; set; }
        public int UserId { get; set; }
        public int? BookingId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
