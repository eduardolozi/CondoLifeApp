using Domain.Enums;

namespace Domain.Models {
    public class Post {
        public int Id { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<string>? Media {  get; set; }
        public PostCategoryEnum? Category { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Like> Likes { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
    }
}
