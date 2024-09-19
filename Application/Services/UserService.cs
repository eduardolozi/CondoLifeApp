using Application.Interfaces;
using Domain.Models;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services {
    public class UserService {
        private readonly CondoLifeContext _dbContext;
        private readonly IEmailService _emailService;

        public UserService(CondoLifeContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public void Insert(User user) {
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

            _emailService.SendEmail(user.Name, user.Email, verificationToken.Value.ToString());
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
