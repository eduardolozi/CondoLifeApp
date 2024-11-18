using System.Security.Claims;
using BlazorApp.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public interface INotificationHub
{
    public Task SendNotificationToAdmin(NotificationPayload message);
}

[Authorize]
public class NotificationHub : Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var roleClaim = Context.User!.Claims.ToList().First(x => x.Type == ClaimTypes.Role);
        if (roleClaim.Value == nameof(UserRoleEnum.Manager))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, nameof(UserRoleEnum.Manager));
            var group = Groups;
        }
        await base.OnConnectedAsync();
    }
}