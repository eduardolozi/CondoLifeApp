using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Application.Services;
using Domain.Enums;
using Domain.Models;
using Infraestructure.Rabbit;
using Newtonsoft.Json;

namespace Worker.BackgroundServices;

public class NotificationBackgroundService(RabbitService rabbitService, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumer = rabbitService.GetBasicConsumer(RabbitConstants.NOTIFICATION_QUEUE, false);
            consumer.Received += async (sender, e) => {
                try {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    var notification = JsonConvert.DeserializeObject<Notification>(body)
                        ?? throw new NullReferenceException("Error deserializing notification.");

                    var httpClient = httpClientFactory.CreateClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notification.UserToken);
                    
                    if (notification.NotificationType is NotificationTypeEnum.BookingCreated)
                    {
                        var response = await httpClient.PostAsJsonAsync("https://localhost:7031/api/Notification/notify-admin", notification);
                        response.EnsureSuccessStatusCode();
                    }
                    
                    if (notification.NotificationType is NotificationTypeEnum.BookingApproved)
                    {
                        var response = await httpClient.PostAsJsonAsync("https://localhost:7031/api/Notification/notify-user", notification);
                        response.EnsureSuccessStatusCode();
                    }

                    using var scope = serviceProvider.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                    notificationService.Insert(notification);
                    rabbitService.Ack(e.DeliveryTag);
                }
                catch (Exception ex) {
                    rabbitService.Nack(e.DeliveryTag, false);
                    throw new Exception(ex.Message, ex);
                }
            };
            await Task.Delay(1000, stoppingToken);
        }
    }
}