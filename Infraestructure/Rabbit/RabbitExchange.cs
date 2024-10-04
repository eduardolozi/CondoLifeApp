namespace Infraestructure.Rabbit {
    public class RabbitExchange {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDurable { get; set; }
        public bool IsAutoDelete { get; set; }

    }
}
