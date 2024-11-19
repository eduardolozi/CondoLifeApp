using API.Hubs;
using Application.Services;
using BlazorApp.Enums;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(IHubContext<NotificationHub, INotificationHub> hubContext, NotificationService notificationService) : ControllerBase
{
    [Authorize]
    [HttpPost("notify-admin")]
    public async Task<IActionResult> NotifyAdmin([FromBody] NotificationPayload payload)
    {
        await hubContext.Clients.Group(nameof(UserRoleEnum.Manager)).AdminReceiveNotification(payload);
        
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetAll([FromQuery] NotificationFilter? filter = null)
    {
        var notifications = notificationService.Get(filter);
        return notifications.HasValue() ? Ok(notifications) : NotFound();
    }
}