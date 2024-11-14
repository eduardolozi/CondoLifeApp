using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models {
    [NotMapped]
    public class Notification {
        public NotificationTypeEnum NotificationType { get; set; }
        public NotificationPayload Message { get; set; }
    }
}
