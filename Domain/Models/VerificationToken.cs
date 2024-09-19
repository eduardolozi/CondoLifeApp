namespace Domain.Models {
    public class VerificationToken {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
