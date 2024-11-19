using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Domain.Enums;
using Domain.Models;
using Infraestructure.Rabbit;
using Newtonsoft.Json;

namespace Worker.BackgroundServices;

public class NotificationBackgroundService(RabbitService rabbitService, IHttpClientFactory httpClientFactory) : BackgroundService
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

                    if (notification.NotificationType is NotificationTypeEnum.BookingCreated)
                    {
                        var httpClient = httpClientFactory.CreateClient();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notification.UserToken);
                        var response = await httpClient.PostAsJsonAsync("https://localhost:7031/api/Notification/notify-admin", notification.Message);
                        response.EnsureSuccessStatusCode();
                    }
                    
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