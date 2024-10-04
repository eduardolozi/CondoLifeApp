using Domain.Enums;

namespace Domain.Models {
    public class Notification {
        public string Id { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
