namespace Infraestructure.Rabbit {
    public class RabbitQueue {
        public string Name { get; set; }
        public bool IsDurable { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsAutoDelete { get; set; }
    }
}
