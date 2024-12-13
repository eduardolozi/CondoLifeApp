using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models {
    public class Post {
        public int Id { get; set; }
        
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; } 
        
        public PostCategoryEnum Category { get; set; }
        
        public int UserId { get; set; }
        
        public User User { get; set; }
        
        [JsonIgnore]
        public List<Like> Likes { get; set; } = [];
        
        [JsonIgnore]
        public List<Comment> Comments { get; set; } = [];
        
        [NotMapped]
        public List<Photo> Photos { get; set; } = [];
        
        public List<PostMedias> Medias { get; set; } = [];
    }
}
