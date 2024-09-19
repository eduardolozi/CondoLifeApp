using Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace Application.Services {
    internal class MailKitService : IEmailService {
        
        public void SendEmail(string userName, string userEmail, string? token = null) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Eduardo Lozano", "eduardoellozi2022@noreply.com"));
            message.To.Add(new MailboxAddress(userName, userEmail));
            message.Subject = "Condolife account verification";
            var verificationLink = $"https://localhost:7031/api/User/verify-email?verificationToken={token}";
            message.Body = new TextPart("plain") {
                Text = $@"Olá {userName}, precisamos verificar a sua conta. Para isso, basta apenas clicar no link a seguir: {verificationLink}"
            };

            using (var client = new SmtpClient()) {
                client.Connect("smtp.gmail.com", 465, true);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("condolifemail@gmail.com", "ipxeydqdbydkamjn");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
