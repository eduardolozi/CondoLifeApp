using Domain.Enums;

namespace Domain.Models.Filters;

public class PostFilter
{
    public int? CondominiumId { get; set; }
    public int? UserId { get; set; }
    public PostCategoryEnum? PostCategory { get; set; }
}