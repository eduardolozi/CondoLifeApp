using BlazorApp.Enums;

namespace BlazorApp.Models;

public class BookingFilter
{
    public BookingStatusEnum? Status { get; set; }
    public int? SpaceId { get; set; }
    public DateTime? InitialDate { get; set; }
    public DateTime? FinalDate { get; set; }
    public int? UserId { get; set; }
}