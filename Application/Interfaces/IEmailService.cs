namespace Application.Interfaces {
    public interface IEmailService {
        public void SendEmail(string userName, string userEmail, string? token = null);
    }
}
