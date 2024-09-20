using Application.Interfaces;
using Domain.Models;
using Infraestructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services {
    public class UserService {
        private readonly CondoLifeContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly PasswordHasher<User> _passworHasher;

        public UserService(CondoLifeContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _passworHasher = new PasswordHasher<User>();
        }

        public List<User> GetAll() { 
            return [.. _dbContext.Users];
        }

        public void Insert(User user) {
            user.PasswordHash = _passworHasher.HashPassword(user, user.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var verificationToken = new VerificationToken {
                Value = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UserId = user.Id,
                User = user
            };

            _dbContext.VerificationTokens.Add(verificationToken);
            _dbContext.SaveChanges();

            SendVerificationEmail(user, verificationToken);
        }

        private void SendVerificationEmail(User user, VerificationToken verificationToken) {
            var verificationLink = $"https://localhost:7031/api/User/verify-email?verificationToken={verificationToken.Value}";
            var message = new EmailMessage {
                FromEmail = "condolifemail@gmail.com",
                FromName = "Eduardo Lozano",
                ToEmail = user.Email,
                ToName = user.Name,
                Subject = "Verificação de conta - Condolife",
                Body = $@"<p>Olá {user.Name}, precisamos verificar a sua conta. Para isso, basta apenas clicar no link a seguir: <a href={verificationLink}>Verificar email</a></p>",
            };
            _emailService.SendEmail(message);
        }

        public void VerifyEmail(string verificationToken) {
            var token =  _dbContext
                .VerificationTokens
                .Include(x => x.User)
                .FirstOrDefault(x => x.Value == verificationToken);
            if (token is null || token.ExpiresAt < DateTime.UtcNow ) {
                throw new Exception("O token de verificação é inválido.");
            }
            if(token.User.IsEmailVerified is true) {
                throw new Exception("O email já foi verificado.");
            }

            token.User.IsEmailVerified = true;
        }
    }
}
