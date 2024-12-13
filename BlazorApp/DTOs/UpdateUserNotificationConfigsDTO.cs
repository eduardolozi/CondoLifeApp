namespace BlazorApp.DTOs;

public class UpdateUserNotificationConfigsDTO
{
    public int Id { get; set; }
    public int? NotificationLifetime { get; set; }
    public bool NofifyEmail { get; set; }
    public bool NofifyPhone { get; set; }
}