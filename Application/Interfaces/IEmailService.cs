using Domain.Models;

namespace Application.Interfaces {
    public interface IEmailService {
        public void SendEmail(EmailMessage messageData);
    }
}
