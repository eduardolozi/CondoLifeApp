namespace BlazorApp.Models;

public class VotingOption
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int VotingId { get; set; }
    public int TotalVotes { get; set; }
}