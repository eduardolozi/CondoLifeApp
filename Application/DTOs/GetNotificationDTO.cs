using Domain.Enums;
using Domain.Models;

namespace Application.DTOs;

public class GetNotificationDTO
{
    public int Id { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
    public NotificationPayload Message { get; set; }
    public int UserId { get; set; }
    public int? BookingId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}