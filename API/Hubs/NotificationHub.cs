using System.Security.Claims;
using Application.Services;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public interface INotificationHub
{
    public Task UserReceiveNotification(NotificationPayload message);
    public Task UserReceiveNotificationsToReadCount(string message);
}

[Authorize]
public class NotificationHub(NotificationService notificationService) : Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var claims = Context.User!.Claims.ToList();
        var role = claims.First(x => x.Type == ClaimTypes.Role);
        var condominium = claims.First(x => x.Type == "CondominiumName");
        var userId = int.Parse(claims.First(x => x.Type == "Id").Value);
        var totalNotifications = notificationService.HasUnreadNotifications(userId);
    
        if(totalNotifications)
            await Clients.Caller.UserReceiveNotificationsToReadCount($"Você possui notificações não lidas");
        
        if (role.Value == nameof(UserRoleEnum.Manager))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{nameof(UserRoleEnum.Manager)}-{condominium.Value}");
        }

        if (role.Value == nameof(UserRoleEnum.Submanager) || role.Value == nameof(UserRoleEnum.Resident))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{nameof(UserRoleEnum.Resident)}-{condominium.Value}");
        }
        await base.OnConnectedAsync();
    }
}