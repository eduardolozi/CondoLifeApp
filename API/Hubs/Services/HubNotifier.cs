using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Shared;
using Shared.DTOs;

namespace API.Hubs.Services;

public class HubNotifier(IHubContext<NotificationHub, INotificationHub> hubContext) : IHubNotifier
{
    public async Task SendNotificationToAdmin(NotificationPayloadDTO message)
    {
        await hubContext.Clients.Group(nameof(UserRoleEnum.Manager)).SendNotificationToAdmin(message);
    }
}