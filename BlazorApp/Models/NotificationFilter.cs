using BlazorApp.Enums;

namespace BlazorApp.Models;

public class NotificationFilter
{
    public int? UserId { get; set; }
    public NotificationTypeEnum? NotificationType { get; set; }
}