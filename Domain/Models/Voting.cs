using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Voting {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        [NotMapped] public int TotalVotes { get; set; }
        public int CondominiumId { get; set; }
        public List<VotingOption> VotingOptions { get; set; } = [];
    }
}
