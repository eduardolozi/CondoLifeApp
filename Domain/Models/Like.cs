namespace Domain.Models {
    public class Like {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}
