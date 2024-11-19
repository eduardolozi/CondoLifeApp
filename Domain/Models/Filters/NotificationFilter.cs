using Domain.Enums;

namespace Application.Services;

public class NotificationFilter
{
    public int? UserId { get; set; }
    public NotificationTypeEnum? NotificationType { get; set; }
}