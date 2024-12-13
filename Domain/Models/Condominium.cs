using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Condominium {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Address Address { get; set; } = null!;
        [JsonIgnore] public List<User> Users { get; set; } = [];
        [JsonIgnore] public List<Voting> Votings { get; set; } = [];
    }
}
