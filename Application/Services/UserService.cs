using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Utils;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;

namespace Application.Services {
    public class UserService {
        private readonly CondoLifeContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly VerificationTokenService _verificationTokenService;
        private readonly PasswordHasher<User> _passworHasher;
        private readonly RabbitService _rabbitService;
        private readonly IDocumentStore _ravenStore;

        public UserService(CondoLifeContext dbContext,
            IEmailService emailService,
            VerificationTokenService verificationTokenService,
            RabbitService rabbitService,
            IDocumentStore ravenStore)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _verificationTokenService = verificationTokenService;
            _passworHasher = new PasswordHasher<User>();
            _rabbitService = rabbitService;
            _ravenStore = ravenStore;
        }

        public List<User> GetAll() { 
            return [.. _dbContext.Users];
        }

        public User? GetById(int id) {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if(user == null) return null;

            user.PhotoUrl = $"https://localhost:7031/api/user/{id}/photo";
            
            return user;
        }

        public UserPhotoDTO GetUserPhoto(int id) {
            var session = _ravenStore.OpenSession();
            var docId = $"user/{id}";
            var userPhoto = session.Query<UserPhoto>().FirstOrDefault(x => x.Id == docId);

            byte[] bytes;
            List<byte> totalStream = new();
            var buffer = new byte[128];
            int read;

            using var photoAttachment = session.Advanced.Attachments.Get(docId, userPhoto.FileName);
            while ((read = photoAttachment.Stream.Read(buffer, 0, buffer.Length)) > 0) {
                totalStream.AddRange(buffer.Take(read));
            }

            return new UserPhotoDTO {
                PhotoBytes = totalStream.ToArray(),
                ContentType = userPhoto.ContentType
            };
        }

        public void Insert(User user) {
            user.PasswordHash = _passworHasher.HashPassword(user, user.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            if (user.Photo.HasValue()) {
                var docId = $"user/{user.Id}";
                var fileName = $"profile-photo-user-{user.Id}.{user.Photo.ContentType.Split('/')[1]}";

                using var session = _ravenStore.OpenSession();
                var userPhoto = new UserPhoto(docId, fileName, user.Photo!.ContentType);
                session.Store(userPhoto, userPhoto.Id);
                session.Advanced.Attachments.Store(userPhoto.Id, userPhoto.FileName, user.Photo!.OpenReadStream(), userPhoto.ContentType);
                session.SaveChanges();
            }

            var verificationToken = _verificationTokenService.CreateVerificationToken(user);

            SendVerificationEmail(user, verificationToken);
        }

        private void SendVerificationEmail(User user, VerificationToken verificationToken) {
            var verificationLink = $"https://localhost:7031/api/User/verify-email?verificationToken={verificationToken.Value}";
            var message = new EmailMessage {
                FromEmail = "condolifemail@gmail.com",
                FromName = "CondoLife",
                ToEmail = user.Email,
                ToName = user.Name,
                Subject = "Verificação de conta - Condolife",
                Body = $@"<p>Olá {user.Name}, precisamos verificar a sua conta. Para isso, basta apenas clicar no link a seguir: <a href={verificationLink}>Verificar email</a></p>",
            };

            _rabbitService.Send(message, RabbitConstants.EMAIL_EXCHANGE, RabbitConstants.EMAIL_ROUTING_KEY);
            //await _emailService.SendEmail(message);
        }

        public int VerifyEmail(string verificationToken) {
            var token = _verificationTokenService.GetVerificationTokenWithUser(verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            _verificationTokenService.HandleVerificationEmail(token);
            _dbContext.SaveChanges();
            return token.User.Id;
        }

        public void Update(int id, User user) {
            var userDB = _dbContext.Users.FirstOrDefault(x => x.Id == id)
                ?? throw new ResourceNotFoundException("Usuario não encontrado.");

            userDB = user;
            _dbContext.SaveChanges();
        }

        public async Task SendRecoveryPasswordEmail(ChangePasswordDTO changePassword) {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == changePassword.Email!.ToLower())
                ?? throw new ResourceNotFoundException("Email inválido");

            var verificationToken = _verificationTokenService.CreateVerificationToken(user);
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

        public int ConfirmPasswordChange(string verificationToken) {
            var token = _verificationTokenService.GetVerificationTokenWithUser(verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            _verificationTokenService.HandlePasswordChange(token);
            _dbContext.SaveChanges();
            return token.User.Id;
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

        public void Delete(int id) {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id)
                ?? throw new ResourceNotFoundException("Usuário não encontrado");
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
