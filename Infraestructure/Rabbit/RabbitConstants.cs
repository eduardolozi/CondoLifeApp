namespace Infraestructure.Rabbit {
    public static class RabbitConstants
    {
        public const string URL_CONN = "amqp://admin:Admin123!@localhost:5672";
        
        public const string EMAIL_QUEUE = "email_queue";
        public const string EMAIL_EXCHANGE = "email_exchange";
        public const string EMAIL_ROUTING_KEY = "email";
        
        public const string NOTIFICATION_QUEUE = "notification_queue";
        public const string NOTIFICATION_EXCHANGE = "notification_exchange";
        public const string NOTIFICATION_ROUTING_KEY = "notification";
    }
}
