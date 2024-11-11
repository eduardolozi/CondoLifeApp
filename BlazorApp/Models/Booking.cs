﻿namespace BlazorApp.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SpaceId { get; set; }
    public DateTime InititalDate { get; set; }
    public DateTime FinalDate { get; set; }
    public string? Description { get; set; }
    public BookingStatusEnum Status { get; set; }
}