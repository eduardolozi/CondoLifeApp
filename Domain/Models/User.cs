using System.Text.Json.Serialization;

namespace Domain.Models {
    public class User {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        [JsonIgnore]
        public bool IsEmailVerified { get; set; } = false;
        public required string Apartment { get; set; }
        public string? Block {  get; set; }
        public string? Photo { get; set; }
        public List<Condominium> Condominium { get; set; } = [];
    }
}
