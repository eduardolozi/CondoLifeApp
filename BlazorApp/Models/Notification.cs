using BlazorApp.Enums;

namespace BlazorApp.Models;

public class Notification
{
    public int Id { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
    public NotificationPayload Message { get; set; }
        
    public int UserId { get; set; }
    public int? BookingId { get; set; }
}