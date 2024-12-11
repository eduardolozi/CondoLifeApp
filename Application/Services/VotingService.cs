using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using FluentValidation;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class VotingService(CondoLifeContext dbContext, AbstractValidator<Voting> votingValidator)
{
    public List<Voting> GetAllVotings(VotingFilter filter)
    {
        var query = dbContext.Voting.AsNoTracking().AsQueryable();
        
        query = query.Where(x => x.CondominiumId == filter.CondominiumId);
        
        if (filter.BaseDate.HasValue)
            query = query.Where(x => x.InitialDate >= filter.BaseDate.Value.FormatDateToBrazilianPattern());
        
        query = filter.IsOpened 
            ? query.Where(x => x.FinalDate > DateTime.UtcNow) 
            : query.Where(x => x.FinalDate <= DateTime.UtcNow);
        
        return query.ToList();
    }

    public void CreateVoting(Voting voting)
    {
        votingValidator.ValidateAndThrow(voting);
        voting.TotalVotes = 0;
        dbContext.Voting.Add(voting);
        dbContext.SaveChanges();
    }
}