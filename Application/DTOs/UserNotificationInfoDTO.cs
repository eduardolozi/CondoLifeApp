namespace Application.DTOs;

public class UserNotificationInfoDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool NotifyEmail { get; set; }
}