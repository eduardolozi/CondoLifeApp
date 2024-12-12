using Domain.Models;

namespace Application.DTOs;

public class CreateVotingDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public int TotalVotes { get; set; }
    public int CondominiumId { get; set; }
    public List<VotingOption> VotingOptions { get; set; }
}