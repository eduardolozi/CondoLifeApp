using Domain.Enums;
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
        
        [JsonIgnore]
        public bool IsEmailVerified { get; set; } = false;
        
        [JsonIgnore]
        public bool IsChangePasswordConfirmed { get; set; } = false;

        public UserRoleEnum Role { get; set; }

        public required string Apartment { get; set; }
        
        public string? Block {  get; set; }
        
        public string? Photo { get; set; }
        
        public List<Condominium> Condominium { get; set; } = [];
    }
}
