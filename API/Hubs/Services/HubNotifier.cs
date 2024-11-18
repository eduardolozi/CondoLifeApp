using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs.Services;

public class HubNotifier(IHubContext<NotificationHub, INotificationHub> hubContext)
{
    public async Task SendNotificationToAdmin(NotificationPayload message)
    {
        var group = hubContext.Clients.Group(nameof(UserRoleEnum.Manager));
        await hubContext.Clients.Group(nameof(UserRoleEnum.Manager)).SendNotificationToAdmin(message);
    }
}