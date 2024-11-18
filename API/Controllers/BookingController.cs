using Application.Services;
using Domain.Models;
using Domain.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(BookingService bookingService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult GetAll([FromQuery] BookingFilter? filter = null)
    {
        var bookings = bookingService.GetAll(filter);
        return bookings.Any() ? Ok(bookings) : NoContent();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Add([FromBody] Booking booking)
    {
        bookingService.Create(booking);
        return Ok();
    }
}