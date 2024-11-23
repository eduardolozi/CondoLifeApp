using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure {
    public class CondoLifeContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Condominium> Condominium { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Space> Space { get; set; } 
        public DbSet<Booking> Booking { get; set; } 
        public DbSet<Notification> Notification { get; set; } 
        public DbSet<NotificationPayload> NotificationPayload { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Comment> Comment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //env
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=CondoLife;User Id=sa;Password=numsey#2021;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True; Encrypt=false");
        }
    }
}
