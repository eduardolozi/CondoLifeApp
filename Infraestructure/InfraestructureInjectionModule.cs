using Infraestructure.Rabbit;
using Infraestructure.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infraestructure {
    public static class InfraestructureInjectionModule {
        public static void AddInfraServices(this IServiceCollection services) {
            services.AddDbContext<CondoLifeContext>();
            var storeHolder = DocumentStoreHolder.Store;
            services.AddSingleton(storeHolder);
            var rabbitService = new RabbitService();
            services.AddSingleton(services => rabbitService);
            ConfigureRabbit(rabbitService);
        }

        private static void ConfigureRabbit(RabbitService rabbitService) {
            var exchanges = new List<RabbitExchange> {
                new() {
                    Name = RabbitConstants.EMAIL_EXCHANGE,
                    Type = ExchangeType.Direct,
                    IsAutoDelete = false,
                    IsDurable = true,
                },
                new() {
                    Name = RabbitConstants.NOTIFICATION_EXCHANGE,
                    Type = ExchangeType.Direct,
                    IsAutoDelete = false,
                    IsDurable = true,
                }
            };

            var queues = new List<RabbitQueue> {
                new() {
                    Name = RabbitConstants.EMAIL_QUEUE,
                    IsAutoDelete = false,
                    IsDurable = true,
                    IsExclusive = false
                },
                new() {
                    Name = RabbitConstants.NOTIFICATION_QUEUE,
                    IsAutoDelete = false,
                    IsDurable = true,
                    IsExclusive = false
                }
            };

            var binds = new List<QueueBind> {
                new() {
                    ExchangeName = RabbitConstants.EMAIL_EXCHANGE,
                    QueueName = RabbitConstants.EMAIL_QUEUE,
                    RoutingKey = RabbitConstants.EMAIL_ROUTING_KEY
                },
                new() {
                    ExchangeName = RabbitConstants.NOTIFICATION_EXCHANGE,
                    QueueName = RabbitConstants.NOTIFICATION_QUEUE,
                    RoutingKey = RabbitConstants.NOTIFICATION_ROUTING_KEY
                }
            };
            rabbitService.DeclareExchanges(exchanges);
            rabbitService.DeclareQueues(queues);
            rabbitService.BindQueues(binds);
        }
    }
}
