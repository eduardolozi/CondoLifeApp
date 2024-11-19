using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models {
    [NotMapped]
    public class Notification {
        public string UserToken { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public NotificationPayload Message { get; set; }
    }
}
