using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using FluentValidation;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class VotingService(CondoLifeContext dbContext,
    AbstractValidator<Voting> votingValidator,
    UserService userService,
    RabbitService rabbitService,
    NotificationService notificationService,
    IEmailService emailService)
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
            query = query.Where(x => x.InitialDate >= filter.BaseDate.Value);
        
        if (filter.UserId.HasValue)
            query = query.Where(v => !v.VotingOptions.Any(vo => vo.Votes.Any(vote => vote.UserId == filter.UserId)));
        
        query = filter.IsOpened 
            ? query.Where(x => x.FinalDate > DateTime.UtcNow) 
            : query.Where(x => x.FinalDate <= DateTime.UtcNow);

        
        return query.ToList();
    }

    public Voting? GetVotingById(int votingId, int userId)
    {
        return dbContext
            .Voting
            .Include(x => x.VotingOptions)
            .Where(x => !x.VotingOptions.Any(vo => vo.Votes.Any(vote => vote.UserId == userId)))
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == votingId);
    }

    public Voting? GetVotingDetails(int votingId)
    {
        return dbContext
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
                VotingOptions = x.VotingOptions.Select(vo => new VotingOption
                {
                    Id = vo.Id,
                    Name = vo.Name,
                    VotingId = vo.VotingId,
                    TotalVotes = vo.Votes.Count()
                }).ToList()
            })
            .FirstOrDefault(x => x.Id == votingId);
    }
    
    public void CreateVoting(string userToken, Voting voting)
    {
        votingValidator.ValidateAndThrow(voting);
        dbContext.Voting.Add(voting);
        dbContext.SaveChanges();

        var condominiumName = dbContext.Condominium.First(x => x.Id == voting.CondominiumId).Name;

        var usersToNotify = userService.GetAllUsersInfoNotificationExceptManager(voting.CondominiumId);
        var usersIds = usersToNotify.Select(x => x.Id).ToList();
        var usersToEmail = usersToNotify.Select(x => (x.Name, x.Email, x.NotifyEmail)).ToList();
        
        var notification = notificationService.SetupManyUsersNotification
        (
            condominiumName,
            userToken,
            NotificationTypeEnum.VotingCreated,
            "Temos uma nova votação!",
            NotificationResultEnum.Info,
            $"Veja agora: {voting.Title}",
            DateTime.UtcNow,
            usersIds
        );

        var emailMessage = emailService.SetupManyUsersEmailMessage
        (
            "Temos uma nova votação!",
            $"Foi criada a votação: {voting.Title}.",
            "https://localhost:7136/votacoes",
            usersToEmail
        );
        
        notificationService.PublishNotification(notification, emailMessage);
    }

    public void ConfirmVote(Vote vote)
    {
        dbContext.Vote.Add(vote);
        dbContext.SaveChanges();
    }
}