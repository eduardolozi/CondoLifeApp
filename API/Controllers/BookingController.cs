using System.IdentityModel.Tokens.Jwt;
using Application.DTOs;
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
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var booking = bookingService.GetById(id);
        return Ok(booking);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Add([FromBody] Booking booking)
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        bookingService.Create(booking, token);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        bookingService.Delete(id);
        return Ok();
    }
    
    [Authorize]
    [HttpPatch("cancel/{id}")]
    public IActionResult Cancel([FromRoute] int id, [FromBody] CancellationBookingDTO cancellationBooking)
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        
        bookingService.Cancel(id, token, cancellationBooking);
        return Ok();
    }
    
    [Authorize(Policy = "ManagerOrSubmanager")]
    [HttpPatch("{id}/accept-booking")]
    public IActionResult ApproveBooking([FromRoute] int id)
    {
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        bookingService.ApproveBooking(id, token);
        return Ok();
    }
}