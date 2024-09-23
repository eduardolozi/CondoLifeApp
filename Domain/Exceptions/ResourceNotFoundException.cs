namespace Domain.Exceptions {
    public class ResourceNotFoundException : Exception {
        public ResourceNotFoundException(string message, Exception? ex = null) : base(message, ex) { }
    }
}
