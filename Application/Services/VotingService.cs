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
        var query = dbContext.Voting.AsNoTracking().AsQueryable();
        
        query = query.Where(x => x.CondominiumId == filter.CondominiumId);
        
        if (filter.BaseDate.HasValue)
            query = query.Where(x => x.InitialDate >= filter.BaseDate.Value.FormatDateToBrazilianPattern());
        
        query = filter.IsOpened 
            ? query.Where(x => x.FinalDate > DateTime.UtcNow) 
            : query.Where(x => x.FinalDate <= DateTime.UtcNow);
        
        return query.ToList();
    }

    public void CreateVoting(CreateVotingDTO votingDto)
    {
        var voting = new Voting
        {
            Id = votingDto.Id,
            Title = votingDto.Title,
            Description = votingDto.Description,
            InitialDate = votingDto.InitialDate,
            FinalDate = votingDto.FinalDate,
            TotalVotes = votingDto.TotalVotes,
            VotingOptions = votingDto.VotingOptions,
            CondominiumId = votingDto.CondominiumId
        };
        
        votingValidator.ValidateAndThrow(voting);
        dbContext.Voting.Add(voting);
        dbContext.SaveChanges();
    }
}