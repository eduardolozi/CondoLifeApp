using System.Security.Claims;
using BlazorApp.Enums;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public interface INotificationHub
{
    public Task ReceiveCreatedBookingNotification(string message);
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

    public async Task SendCreatedBookingNotification(string message)
    {
        await Clients.Group(nameof(UserRoleEnum.Manager)).ReceiveCreatedBookingNotification(message);
    }
}