using BlazorApp.Enums;

namespace BlazorApp.DTOs;

public class BookingDetailsDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string? UserPhotoUrl { get; set; }
    public int UserId { get; set; }
    public string Apartment { get; set; }
    public string? Description { get; set; }
    public BookingStatusEnum Status { get; set; }
    public string Date { get; set; }
    public string SpaceName { get; set; }
}