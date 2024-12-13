namespace Domain.Models {
    public class Vote {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VotingOptionId { get; set; }
    }
}
