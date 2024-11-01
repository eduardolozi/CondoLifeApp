using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Utils;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
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

            return user;
        }

        public string? GetUserPhoto(int id, string fileName) {
            var session = _ravenStore.OpenSession();
            var docId = $"user/{id}";
            var userPhoto = session.Query<Photo>().FirstOrDefault(x => x.Id == docId);
            if(userPhoto is null) return null;
            
            byte[] bytes;
            List<byte> totalStream = new();
            var buffer = new byte[128];
            int read;

            using var photoAttachment = session.Advanced.Attachments.Get(docId, userPhoto.FileName);
            while ((read = photoAttachment.Stream.Read(buffer, 0, buffer.Length)) > 0) {
                totalStream.AddRange(buffer.Take(read));
            }

            return Convert.ToBase64String(totalStream.ToArray());
        }

        private void SavePhoto(User user)
        {
            var fileName = user.Photo!.FileName.Replace(' ', '-').Replace('.', '-');
            var photoBytes = Convert.FromBase64String(user.Photo.ContentBase64!);

            using var session = _ravenStore.OpenSession();
            var userPhoto = new Photo($"user/{user.Id}", fileName, user.Photo.ContentType!);
            var document = session.Load<Photo>(userPhoto.Id);
            if (document is not null)
            {
                document.ContentType = userPhoto.ContentType;
                document.FileName = userPhoto.FileName;
            }
            else
            {
                session.Store(userPhoto);
            }
            session.SaveChanges();
            
            session.Advanced.Attachments.Delete(userPhoto.Id, userPhoto.FileName);
            session.SaveChanges();
            
            session.Advanced.Attachments.Store(userPhoto.Id, userPhoto.FileName, new MemoryStream(photoBytes), userPhoto.ContentType);
            session.SaveChanges();

            user.PhotoUrl = $"https://localhost:7031/api/user/{user.Id}/photo?fileName={fileName}";
            _dbContext.SaveChanges();
        }

        public void Insert(User user) {
            user.PasswordHash = _passworHasher.HashPassword(user, user.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            if (user.Photo.HasValue()) {
                SavePhoto(user);
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

            userDB.Name = user.Name;
            userDB.Email = user.Email;
            userDB.Photo = user.Photo;
            userDB.Apartment = user.Apartment;
            userDB.Block = user.Block; 
            _dbContext.SaveChanges();
            
            if (userDB.Photo.HasValue()) {
                SavePhoto(user);
            }
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
        
        public void DeleteAll() {
            var users = GetAll();
            foreach (var user in users)
            {
                _dbContext.Remove(user);
            }
            _dbContext.SaveChanges();
        }
    }
}
