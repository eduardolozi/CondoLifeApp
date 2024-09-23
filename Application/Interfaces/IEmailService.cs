using Domain.Models;

namespace Application.Interfaces {
    public interface IEmailService {
        public Task SendEmail(EmailMessage messageData);
    }
}
