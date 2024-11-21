using Domain.Exceptions;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services {
    public class VerificationTokenService(CondoLifeContext dbContext)
    {
        public VerificationToken CreateVerificationToken(User user) {
            var verificationToken = new VerificationToken {
                Value = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UserId = user.Id,
                User = user
            };

            dbContext.VerificationTokens.Add(verificationToken);
            dbContext.SaveChanges();

            return verificationToken;
        }

        public VerificationToken? GetVerificationTokenWithUser(string token) {
            return dbContext.VerificationTokens.Include(x => x.User).FirstOrDefault(x => x.Value == token);
        }

        public void HandleVerificationEmail(VerificationToken token) {
            if (token.ExpiresAt < DateTime.UtcNow) {
                throw new BadRequestException("O token de verificação é inválido.");
            }
            if (token.User.IsEmailVerified) {
                throw new ConflictException("O email já foi verificado.");
            }

            token.User.IsEmailVerified = true;
        }
        
        public void HandlePasswordChange(VerificationToken token) {
            if (token.ExpiresAt < DateTime.UtcNow) {
                throw new BadRequestException("O token de verificação é inválido.");
            }
            if (token.User.IsChangePasswordConfirmed) {
                throw new ConflictException("O link de mudança de senha já foi acessado.");
            }
            token.User.IsChangePasswordConfirmed = true;
        }
    }
}
