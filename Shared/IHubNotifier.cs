using Shared.DTOs;

namespace Shared;

public interface IHubNotifier
{
    public Task SendNotificationToAdmin(NotificationPayloadDTO message);
}