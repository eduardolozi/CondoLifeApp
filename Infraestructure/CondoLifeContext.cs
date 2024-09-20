using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure {
    public class CondoLifeContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //env
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=CondoLife;User Id=sa;Password=numsey#2021;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True; Encrypt=false");
        }
    }
}
