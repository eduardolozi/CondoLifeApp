using Application.Interfaces;
using Infraestructure.Rabbit;
using System.Text.Json;
using System.Text;
using Domain.Models;

namespace Worker.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private IEmailService _emailService;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitService _rabbitService;
        public EmailBackgroundService(IServiceProvider serviceProvider, RabbitService rabbitService)
        {
            _serviceProvider = serviceProvider;
            _rabbitService = rabbitService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope()) {
                    _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                }
                
                var consumer = _rabbitService.GetBasicConsumer(RabbitConstants.EMAIL_QUEUE, false);
                
                consumer.Received += async (sender, e) => {
                    try {
                        var body = Encoding.UTF8.GetString(e.Body.ToArray());
                        var emailMessage = JsonSerializer.Deserialize<EmailMessage>(body) 
                                           ?? throw new ArgumentNullException();
                        
                        foreach (var user in emailMessage!.UsersTo)
                        {
                            emailMessage.ToEmail = user.Email;
                            emailMessage.ToName = user.Name;
                            await _emailService.SendEmail(emailMessage);
                        }
                        
                        _rabbitService.Ack(e.DeliveryTag);
                    }
                    catch (Exception) {
                        _rabbitService.Nack(e.DeliveryTag, false);
                    }
                };
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
