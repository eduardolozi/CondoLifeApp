using System.Security.Claims;
using BlazorApp.Enums;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs;

namespace API.Hubs;

public interface INotificationHub
{
    public Task SendNotificationToAdmin(NotificationPayloadDTO message);
}

public class NotificationHub : Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var roleClaim = Context.User!.Claims.ToList().First(x => x.Type == ClaimTypes.Role);
        if (roleClaim.Value == nameof(UserRoleEnum.Manager))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, nameof(UserRoleEnum.Manager));
        }
        await base.OnConnectedAsync();
    }
}