using System.Text.Json.Serialization;

namespace Domain.Models;

public class UserNotification
{
    public int Id { get; set; }
    public bool IsRead { get; set; }
    
    public int UserId { get; set; }
    [JsonIgnore] public User? User { get; set; }
    
    public int NotificationId { get; set; }
    [JsonIgnore] public Notification? Notification { get; set; }
}