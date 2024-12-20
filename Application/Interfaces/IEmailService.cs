using Domain.Models;

namespace Application.Interfaces {
    public interface IEmailService {
        public Task SendEmail(EmailMessage messageData);
        public EmailMessage? SetupOneUserEmailMessage(string title,
            string toName,
            string bodyMessage,
            string redirectUrl,
            string toEmail,
            bool notifyEmail);

        public EmailMessage? SetupManyUsersEmailMessage(string title,
            string bodyMessage,
            string redirectUrl,
            List<(string toName, string toEmail, bool toNotify)> usersTo);
    }
}
