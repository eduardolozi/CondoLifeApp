using Domain.Enums;

namespace Domain.Models.Filters;

public class NotificationFilter
{
    public int UserId { get; set; }
    public bool IsBookings { get; set; }
    public bool IsVotings { get; set; }
    public bool IsPosts { get; set; }
    public bool IsGeneralAnnouncements { get; set; }
    public bool IsFinancial { get; set; }
}