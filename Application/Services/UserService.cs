using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Infraestructure;
using Microsoft.AspNet.Identity;
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

        public User GetById(int id) {
            return _dbContext.Users.FirstOrDefault(x => x.Id == id)
                ?? throw new ResourceNotFoundException("Usuário não encontrado");
        }  

        public async Task Insert(User user) {
            user.PasswordHash = _passworHasher.HashPassword(user, user.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var verificationToken = CreateVerificationToken(user);

            await SendVerificationEmail(user, verificationToken);
        }

        private async Task SendVerificationEmail(User user, VerificationToken verificationToken) {
            var verificationLink = $"https://localhost:7031/api/User/verify-email?verificationToken={verificationToken.Value}";
            var message = new EmailMessage {
                FromEmail = "condolifemail@gmail.com",
                FromName = "CondoLife",
                ToEmail = user.Email,
                ToName = user.Name,
                Subject = "Verificação de conta - Condolife",
                Body = $@"<p>Olá {user.Name}, precisamos verificar a sua conta. Para isso, basta apenas clicar no link a seguir: <a href={verificationLink}>Verificar email</a></p>",
            };
            await _emailService.SendEmail(message);
        }

        private VerificationToken CreateVerificationToken(User user) {
            var verificationToken = new VerificationToken {
                Value = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UserId = user.Id,
                User = user
            };

            _dbContext.VerificationTokens.Add(verificationToken);
            _dbContext.SaveChanges();

            return verificationToken;
        }

        public void VerifyEmail(string verificationToken) {
            var token = _dbContext.VerificationTokens.Include(x => x.User).FirstOrDefault(x => x.Value == verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            if (token.ExpiresAt < DateTime.UtcNow) {
                throw new BadRequestException("O token de verificação é inválido.");
            }
            if (token.User.IsEmailVerified) {
                throw new ConflictException("O email já foi verificado.");
            }
            token.User.IsEmailVerified = true;
            _dbContext.SaveChanges();
        }

        public async Task SendRecoveryPasswordEmail(ChangePasswordDTO changePassword) {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == changePassword.Email!.ToLower())
                ?? throw new ResourceNotFoundException("Email inválido");

            var verificationToken = CreateVerificationToken(user);
            var verificationLink = $"https://localhost:7031/api/User/confirm-password-change?verificationToken={verificationToken.Value}";

            var message = new EmailMessage {
                FromEmail = "condolifemail@gmail.com",
                FromName = "CondoLife",
                ToEmail = user.Email,
                ToName = user.Name,
                Subject = "Mudança de senha - Condolife",
                Body = $"<p>Olá {user.Name}.\nRecebemos uma notificação de mudança de senha da sua conta.\nConfirme se é você clicando no link a seguir: <a href={verificationLink}>Confirmar mudança de senha</a></p>"
            };
            await _emailService.SendEmail(message);
        }

        public void ConfirmPasswordChange(string verificationToken) {
            var token = _dbContext.VerificationTokens.Include(x => x.User).FirstOrDefault(x => x.Value == verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            if (token.ExpiresAt < DateTime.UtcNow) {
                throw new BadRequestException("O token de verificação é inválido.");
            }
            if(token.User.IsChangePasswordConfirmed) {
                throw new ConflictException("O link de mudança de senha já foi acessado.");
            }
            token.User.IsChangePasswordConfirmed = true;
            _dbContext.SaveChanges();
        }

        public void ChangePassword(ChangePasswordDTO changePassword) {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == changePassword.Email)
                ?? throw new ResourceNotFoundException("Email inválido.");

            if (user.IsChangePasswordConfirmed) {
                user.Password = changePassword.NewPassword!;
                user.PasswordHash = _passworHasher.HashPassword(user, user.Password);
                user.IsChangePasswordConfirmed = false;
                _dbContext.SaveChanges();
                return;
            }

            throw new BadRequestException("Ainda não foi processada a verificação de mudança de senha no email.");
        }
    }
}
