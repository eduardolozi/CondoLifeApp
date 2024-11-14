using System.Text;
using Domain.Enums;
using Domain.Models;
using Infraestructure.Rabbit;
using Newtonsoft.Json;
using Shared;
using Shared.DTOs;

namespace Worker.BackgroundServices;

public class NotificationBackgroundService(RabbitService rabbitService, IServiceProvider serviceProvider) : BackgroundService
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
                        var notificationPayload = new NotificationPayloadDTO
                        {
                            Header = notification.Message.Header,
                            Body = notification.Message.Body,
                            Link = notification.Message.Link
                        };
                        var hubNotifier = serviceProvider.GetRequiredService<IHubNotifier>();
                        await hubNotifier.SendNotificationToAdmin(notificationPayload);
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