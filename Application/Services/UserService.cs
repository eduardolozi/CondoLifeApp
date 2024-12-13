using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Utils;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents;
using Photo = Domain.Models.Photo;
using User = Domain.Models.User;
using UserFilter = Domain.Models.Filters.UserFilter;

namespace Application.Services {
    public class UserService(
        CondoLifeContext dbContext,
        IEmailService emailService,
        VerificationTokenService verificationTokenService,
        RabbitService rabbitService,
        IDocumentStore ravenStore)
    {
        private readonly PasswordHasher<User> _passworHasher = new();

        public List<User> GetAll(UserFilter? filter = null) { 
            var query = dbContext.Users.AsNoTracking().AsQueryable();
            
            if (filter is not null)
            {
                if (filter.CondominiumId.HasValue)
                    query = query.Where(x => x.CondominiumId == filter.CondominiumId);
                
                if (filter.Username.HasValue())
                    query = query.Where(x => EF.Functions.Like(x.Name, $"%{filter.Username}%"));
                
                if (filter.Role.HasValue)
                    query = query.Where(x => (int)x.Role == (int)filter.Role);

                if (filter.OnlyEmailVerified is true)
                    query = query.Where(x => x.IsEmailVerified == true);
            }
            
            return query.ToList();
        }

        public User? GetById(int id) {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == id);
            if(user == null) return null;

            return user;
        }

        public string? GetUserPhoto(int id, string? fileName) {
            var session = ravenStore.OpenSession();
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

            using var session = ravenStore.OpenSession();
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
            dbContext.SaveChanges();
        }

        public void Insert(User user) {
            user.PasswordHash = _passworHasher.HashPassword(user, user.Password);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            if (user.Photo.HasValue()) {
                SavePhoto(user);
            }

            var verificationToken = verificationTokenService.CreateVerificationToken(user);
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

            rabbitService.Send(message, RabbitConstants.EMAIL_EXCHANGE, RabbitConstants.EMAIL_ROUTING_KEY);
        }

        public int VerifyEmail(string verificationToken) {
            var token = verificationTokenService.GetVerificationTokenWithUser(verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            verificationTokenService.HandleVerificationEmail(token);
            dbContext.SaveChanges();
            return token.User.Id;
        }

        public void Update(int id, User user) {
            var userDb = dbContext.Users.FirstOrDefault(x => x.Id == id)
                ?? throw new ResourceNotFoundException("Usuario não encontrado.");

            userDb.Name = user.Name;
            userDb.Email = user.Email;
            userDb.Photo = user.Photo;
            userDb.Apartment = user.Apartment;
            userDb.Block = user.Block; 
            dbContext.SaveChanges();
            
            if (userDb.Photo.HasValue()) {
                SavePhoto(user);
            }
        }

        public async Task SendRecoveryPasswordEmail(ChangePasswordDTO changePassword) {
            var user = dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == changePassword.Email!.ToLower())
                ?? throw new ResourceNotFoundException("Email inválido");

            var verificationToken = verificationTokenService.CreateVerificationToken(user);
            var verificationLink = $"https://localhost:7031/api/User/confirm-password-change?verificationToken={verificationToken.Value}";

            var message = new EmailMessage {
                FromEmail = "condolifemail@gmail.com",
                FromName = "CondoLife",
                ToEmail = user.Email,
                ToName = user.Name,
                Subject = "Mudança de senha - Condolife",
                Body = $"<p>Olá {user.Name}.\nRecebemos uma notificação de mudança de senha da sua conta.\nConfirme se é você clicando no link a seguir: <a href={verificationLink}>Confirmar mudança de senha</a></p>"
            };
            await emailService.SendEmail(message);
        }

        public int ConfirmPasswordChange(string verificationToken) {
            var token = verificationTokenService.GetVerificationTokenWithUser(verificationToken)
                ?? throw new ResourceNotFoundException("Token não encontrado.");

            verificationTokenService.HandlePasswordChange(token);
            dbContext.SaveChanges();
            return token.User.Id;
        }

        public void ChangePassword(ChangePasswordDTO changePassword) {
            var user = dbContext.Users.FirstOrDefault(x => x.Email == changePassword.Email)
                ?? throw new ResourceNotFoundException("Email inválido.");

            if (user.IsChangePasswordConfirmed) {
                user.Password = changePassword.NewPassword!;
                user.PasswordHash = _passworHasher.HashPassword(user, user.Password);
                user.IsChangePasswordConfirmed = false;
                dbContext.SaveChanges();
                return;
            }

            throw new BadRequestException("Ainda não foi processada a verificação de mudança de senha no email.");
        }

        public void Delete(int id) {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == id)
                ?? throw new ResourceNotFoundException("Usuário não encontrado");
            dbContext.Remove(user);
            dbContext.SaveChanges();
        }
        
        public void DeleteAll() {
            var users = GetAll();
            foreach (var user in users)
            {
                dbContext.Remove(user);
            }
            dbContext.SaveChanges();
        }

        public void ChangeUserRole(ChangeUserRoleDTO changeUserRole)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == changeUserRole.UserId)
                ?? throw new ResourceNotFoundException("Usuário não encontrado.");
            user.Role = changeUserRole.Role;
            dbContext.SaveChanges();
        }
    }
}
