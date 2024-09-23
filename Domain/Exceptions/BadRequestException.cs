namespace Domain.Exceptions {
    public class BadRequestException : Exception {
        public BadRequestException(string message, Exception? ex = null) : base(message, ex) { 
        }
    }
}
