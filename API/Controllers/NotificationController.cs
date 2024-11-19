using API.Hubs;
using BlazorApp.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(IHubContext<NotificationHub, INotificationHub> hubContext) : ControllerBase
{
    [Authorize]
    [HttpPost("notify-admin")]
    public async Task<IActionResult> NotifyAdmin([FromBody] NotificationPayload payload)
    {
        await hubContext.Clients.Group(nameof(UserRoleEnum.Manager)).AdminReceiveNotification(payload);
        
        return Ok();
    }
}