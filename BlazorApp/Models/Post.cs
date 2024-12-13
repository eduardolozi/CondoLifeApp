using BlazorApp.Enums;

namespace BlazorApp.Models;

public class Post
{
    public int Id { get; set; }
        
    public string Description { get; set; }
        
    public DateTime CreatedAt { get; set; } 
        
    public PostCategoryEnum Category { get; set; }
        
    public int UserId { get; set; }
        
    public User User { get; set; }

    public List<Photo> Photos { get; set; } = [];
    
    public List<PostMedias> Medias { get; set; } = [];
}