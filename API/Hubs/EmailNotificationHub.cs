using Microsoft.AspNetCore.SignalR;

namespace API.Hubs {
    public class EmailNotificationHub : Hub {
        public async Task NotifyPasswordReset(string userId) {
            await Clients.User(userId).SendAsync("ReceiveNotification", "Sua senha foi alterada com sucesso.");
        }

        public async Task NotifyRegistrationVerification(string userId) {
            await Clients.User(userId).SendAsync("ReceiveNotification", "Seu cadastro foi verificado.");
        }
    }
}
