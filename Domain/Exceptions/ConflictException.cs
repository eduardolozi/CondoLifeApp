namespace Domain.Exceptions {
    public class ConflictException : Exception {
        public ConflictException(string message, Exception? ex = null) : base(message, ex) { }
    }
}
