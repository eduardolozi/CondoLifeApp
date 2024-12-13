namespace BlazorApp.Models;

public class VotingFilter
{
    public int CondominiumId { get; set; }
    public DateTime? BaseDate { get; set; }
    public bool IsOpened { get; set; }
    public int? UserId { get; set; }
}