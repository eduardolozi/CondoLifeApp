using Domain.Enums;

namespace Domain.Models.Filters;

public class NotificationFilter
{
    public int? UserId { get; set; }
    public NotificationTypeEnum? NotificationType { get; set; }
}