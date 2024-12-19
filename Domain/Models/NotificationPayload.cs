using Domain.Enums;

namespace Domain.Models;

public class NotificationPayload
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public string? Link { get; set; }
    public NotificationResultEnum ResultCategory { get; set; }
    public int NotificationId { get; set; }
}