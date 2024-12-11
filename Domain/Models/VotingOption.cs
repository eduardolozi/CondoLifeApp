using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class VotingOption {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VotingId { get; set; }
        public int TotalVotes { get; set; }
        [JsonIgnore] public List<Vote> Votes { get; set; }
        [Timestamp] [JsonIgnore] byte[] RowVersion { get; set; }
    }
}
