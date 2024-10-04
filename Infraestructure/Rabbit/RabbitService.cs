using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infraestructure.Rabbit {
    public class RabbitService {
        private IConnection _connection;
        private ConnectionFactory _factory;
        private IModel _channel;

        public RabbitService() {
            _factory = new ConnectionFactory { Uri = new Uri("amqp://admin:Admin@123#!@5c08cf41cba6:5672/") };
            Connect();
        }

        public void Connect() {
            try {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex) {
                throw new Exception("Erro ao estabelecer conexão com o RabbitMQ", ex);
            }

        }

        public void Disconnect() {
            _channel.Close();
            _connection.Close();
        }

        public void DeclareExchanges(List<RabbitExchange> exchanges) {
            foreach (RabbitExchange exchange in exchanges) {
                _channel.ExchangeDeclare(exchange.Name, exchange.Type, exchange.IsDurable, exchange.IsAutoDelete);
            }
        }

        public void DeclareQueues(List<RabbitQueue> queues) {
            foreach (RabbitQueue queue in queues) {
                _channel.QueueDeclare(queue.Name, queue.IsDurable, queue.IsExclusive, queue.IsAutoDelete, null);
            }
        }

        public void BindQueues(List<QueueBind> queueBinds) {
            foreach (QueueBind bind in queueBinds) {
                _channel.QueueBind(bind.QueueName, bind.ExchangeName, bind.RoutingKey, null);
            }
        }

        public void Send<T>(T messageObject, string exchangeName, string routingKey) {
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageObject));
            _channel.BasicPublish(exchangeName, routingKey, null, messageBody);
        }

        public EventingBasicConsumer GetBasicConsumer(string queueName, bool autoAck) {
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queueName, autoAck, consumer);
            return consumer;
        }

        public void Ack(ulong deliveryTag, bool multiple = false) {
            _channel.BasicAck(deliveryTag, multiple);
        }

        public void Nack(ulong deliveryTag, bool requeue, bool multiple = false) {
            _channel.BasicNack(deliveryTag, multiple, requeue);
        }
    }
}