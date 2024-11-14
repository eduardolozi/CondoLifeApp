using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[NotMapped]
public class NotificationPayload
{
    public string Header { get; set; }
    public string Body { get; set; }
    public string? Link { get; set; }
}