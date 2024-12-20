using Application.Helpers;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Application.Services {
    
    internal class MailKitService : IEmailService {
        
        public async Task SendEmail(EmailMessage messageData) {
            try {
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

                await client.SendAsync(message);
                client.Disconnect(true);
            }
            catch (Exception ex) {
                throw new BadRequestException(ex.Message, ex);
            }
        }
        
        public EmailMessage? SetupOneUserEmailMessage(string title,
            string toName,
            string bodyMessage,
            string redirectUrl,
            string toEmail,
            bool notifyEmail)
        {
            if (notifyEmail)
            {
                var placeholders = new Dictionary<string, string>
                {
                    {"Title", title},
                    {"BodyMessage", bodyMessage},
                    {"RedirectUrl", redirectUrl}
                };
                var htmlBody = TemplateHelper.GetTemplateContent(placeholders);
                return new EmailMessage
                {
                    FromEmail = "condolifemail@gmail.com",
                    FromName = "CondoLife",
                    UsersTo = [new EmailUser
                    {
                        Email = toEmail,
                        Name = toName
                    }],
                    Subject = title,
                    Body = htmlBody,
                };
            }
            return null;
        }

        public EmailMessage? SetupManyUsersEmailMessage(string title,
            string bodyMessage,
            string redirectUrl,
            List<(string toName, string toEmail, bool toNotify)> usersTo)
        {
            var placeholders = new Dictionary<string, string>
            {
                {"Title", title},
                {"BodyMessage", bodyMessage},
                {"RedirectUrl", redirectUrl}
            };
            var htmlBody = TemplateHelper.GetTemplateContent(placeholders);
            var emailMessage =  new EmailMessage
            {
                FromEmail = "condolifemail@gmail.com",
                FromName = "CondoLife",
                UsersTo = [],
                Subject = title,
                Body = htmlBody,
            };
            
            for (var i = 0; i < usersTo.Count; i++)
            {
                if (usersTo[i].toNotify)
                {
                    emailMessage.UsersTo.Add(new EmailUser
                    {
                        Email = usersTo[i].toEmail,
                        Name = usersTo[i].toName
                    });
                }
            }

            return emailMessage;
        }
    }
}
