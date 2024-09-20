using Application.Interfaces;
using Domain.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Application.Services {
    
    internal class MailKitService : IEmailService {
        
        public void SendEmail(EmailMessage messageData) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(messageData.FromName, messageData.FromEmail));
            message.To.Add(new MailboxAddress(messageData.ToName, messageData.ToEmail));
            message.Subject = messageData.Subject;
            message.Body = new TextPart("html") {
                Text = messageData.Body
            };

            using var client = new SmtpClient();
            //env
            client.Connect("smtp.gmail.com", 465, true);

            //env
            client.Authenticate(messageData.FromEmail, "ipxeydqdbydkamjn");

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
