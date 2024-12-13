using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class VotingOption {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VotingId { get; set; }
        [NotMapped] public int TotalVotes { get; set; }
        [JsonIgnore] public List<Vote> Votes { get; set; } = [];
    }
}
