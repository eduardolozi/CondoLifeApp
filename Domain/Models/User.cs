using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class User {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public required string Email { get; set; }
        
        [NotMapped]
        public required string Password { get; set; }

        [JsonIgnore]
        public string? PasswordHash { get; set; } = null!;
        
        public bool IsEmailVerified { get; set; } = false;
        
        public bool IsChangePasswordConfirmed { get; set; } = false;

        public UserRoleEnum Role { get; set; }

        public required int Apartment { get; set; }
        
        public string? Block {  get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }

        public string? PhotoUrl { get; set; }

        [JsonIgnore]
        public List<Condominium> Condominium { get; set; } = [];
    }
}
