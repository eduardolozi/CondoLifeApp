using Application.DTOs;
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
        var query = dbContext
            .Voting
            .AsNoTracking()
            .Select(x => new Voting
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                InitialDate = x.InitialDate,
                FinalDate = x.FinalDate,
                CondominiumId = x.CondominiumId,
                TotalVotes = x.VotingOptions.SelectMany(vo => vo.Votes).Count(),
                VotingOptions = x.VotingOptions
            })
            .AsQueryable();
        
        query = query.Where(x => x.CondominiumId == filter.CondominiumId);
        
        if (filter.BaseDate.HasValue)
            query = query.Where(x => x.InitialDate >= filter.BaseDate.Value.FormatDateToBrazilianPattern());
        
        query = filter.IsOpened 
            ? query.Where(x => x.FinalDate > DateTime.UtcNow) 
            : query.Where(x => x.FinalDate <= DateTime.UtcNow);
        
        return query.ToList();
    }

    public Voting? GetVotingById(int votingId)
    {
        return dbContext
            .Voting
            .Include(x => x.VotingOptions)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == votingId);
    }

    public void CreateVoting(Voting voting)
    {
        votingValidator.ValidateAndThrow(voting);
        dbContext.Voting.Add(voting);
        dbContext.SaveChanges();
    }

    public void ConfirmVote(Vote vote)
    {
        dbContext.Vote.Add(vote);
        dbContext.SaveChanges();
    }
}