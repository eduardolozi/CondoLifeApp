using API.Hubs;
using Application.Services;
using Domain.Enums;
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
    public async Task<IActionResult> NotifyAdmin([FromBody] Notification notification)
    {
        await hubContext.Clients.Group($"{nameof(UserRoleEnum.Manager)}-{notification.CondominiumName}").UserReceiveNotification(notification.Message);
        
        return Ok();
    }
    
    [Authorize]
    [HttpPost("notify-user")]
    public async Task<IActionResult> NotifyUser([FromBody] Notification notification, [FromQuery] string userId)
    {
        await hubContext.Clients.User(userId).UserReceiveNotification(notification.Message);
        
        return Ok();
    }
    
    [Authorize]
    [HttpPost("notify-all-except-manager")]
    public async Task<IActionResult> NotifyAllExceptManager([FromBody] Notification notification)
    {
        await hubContext.Clients.Group($"{nameof(UserRoleEnum.Resident)}-{notification.CondominiumName}").UserReceiveNotification(notification.Message);
        
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetAll([FromQuery] NotificationFilter filter)
    {
        var notifications = notificationService.Get(filter);
        return notifications.HasValue() ? Ok(notifications) : NotFound();
    }

    [Authorize]
    [HttpPatch("{userId}/mark-as-readed")]
    public IActionResult MarkAsReaded([FromRoute] int userId, [FromQuery] int firstOpenNotificationId)
    {
        notificationService.MarkAsReaded(userId, firstOpenNotificationId);
        return Ok();
    }
}