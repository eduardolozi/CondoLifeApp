using System.Text;
using Domain.Enums;
using Domain.Models;
using Infraestructure.Rabbit;
using Newtonsoft.Json;

namespace Worker.BackgroundServices;

public class NotificationBackgroundService(RabbitService rabbitService) : BackgroundService
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
                        
                    }
                    
                    rabbitService.Ack(e.DeliveryTag);
                }
                catch (Exception) {
                    rabbitService.Nack(e.DeliveryTag, false);
                }
            };
            await Task.Delay(1000, stoppingToken);
        }
    }
}