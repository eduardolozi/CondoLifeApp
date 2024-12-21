using BlazorApp.Enums;

namespace BlazorApp.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SpaceId { get; set; }
    public string? Username { get; set; }
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public string? Description { get; set; }
    public BookingStatusEnum Status { get; set; }
    public string? CancellationReason { get; set; }
    public string? SpaceName { get; set; }
    
}