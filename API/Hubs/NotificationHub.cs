using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public interface INotificationHub
{
    public Task AdminReceiveNotification(NotificationPayload message);
    public Task UserReceiveNotification(NotificationPayload message);
}

[Authorize]
public class NotificationHub : Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var roleClaim = Context.User!.Claims.ToList().First(x => x.Type == ClaimTypes.Role);
        var condominiumClaim = Context.User!.Claims.ToList().First(x => x.Type == "CondominiumName");
        if (roleClaim.Value == nameof(UserRoleEnum.Manager))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{nameof(UserRoleEnum.Manager)}-{condominiumClaim.Value}");
            var group = Groups;
        }
        await base.OnConnectedAsync();
    }
}