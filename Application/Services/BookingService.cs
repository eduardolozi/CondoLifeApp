using System.Globalization;
using Application.DTOs;
using Application.Helpers;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using FluentValidation;
using FluentValidation.Results;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class BookingService(CondoLifeContext dbContext, IEmailService emailService, AbstractValidator<Booking> bookingValidator, RabbitService rabbitService, NotificationService notificationService)
{
    public List<Booking> GetAll(BookingFilter? filter)
    {
        var query = dbContext.Booking.AsNoTracking().Include(x => x.User).Include(x => x.Space).AsQueryable();
        if (filter != null)
        {
            if (filter.SpaceId.HasValue)
                query = query.Where(x => x.SpaceId == filter.SpaceId);

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            if (filter.InitialDate.HasValue)
                query = query.Where(x => x.InitialDate >= filter.InitialDate.Value);

            if (filter.FinalDate.HasValue)
                query = query.Where(x => x.FinalDate <= filter.FinalDate.Value);

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId);
        }
        
        return query.Select(x => new Booking
        {
            Id = x.Id,
            SpaceId = x.SpaceId,
            SpaceName = x.Space.Name,
            Status = x.Status,
            InitialDate = x.InitialDate,
            FinalDate = x.FinalDate,
            UserId = x.UserId,
            Username = x.User.Name,
            Description = x.Description,
            
        }).OrderBy(x => x.InitialDate).ToList();
    }

    public BookingDetailsDTO GetById(int id)
    {
        var booking = dbContext
            .Booking
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Space)
            .FirstOrDefault(x => x.Id == id)
            ?? throw new ResourceNotFoundException("Reserva não encontrada.");

        return new BookingDetailsDTO
        {
            Id = booking.Id,
            Description = booking.Description,
            Username = booking.User!.Name,
            UserId = booking.UserId,
            UserPhotoUrl = booking.User.PhotoUrl,
            Apartment = $"{booking.User.Apartment} {(booking.User.Block.HasValue() ? booking.User.Block : string.Empty)}",
            Status = booking.Status,
            Date = $"{booking.InitialDate.ToShortDateString()} - {booking.InitialDate.Hour}:00 às {booking.FinalDate.Hour}:00",
            SpaceName = booking.Space!.Name
        };
    }

    public void Create(Booking booking, string token)
    {
        bookingValidator.ValidateAndThrow(booking);
        if (booking.Status != BookingStatusEnum.Pending && booking.Status != BookingStatusEnum.AwaitingPayment)
        {
            var statusError = new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "O status de uma reserva criada deve ser pendente ou aguardando pagamento."
                }
            };
            
            throw new ValidationException(statusError);
        }
        
        dbContext.Booking.Add(booking);
        dbContext.SaveChanges();

        var user = dbContext
            .Users
            .AsNoTracking()
            .Include(x => x.Condominium)
            .First(x => x.Id == booking.UserId);
        
        var block = user.Block.HasValue() ? $"-{user.Block}" : string.Empty;
        var userApartment = $"{user.Apartment}{block}";
        
        var space = dbContext
                        .Space
                        .AsNoTracking()
                        .FirstOrDefault(x => x.Id == booking.SpaceId)
            ?? throw new ResourceNotFoundException("Não foi encontrado esse espaço no condomínio.");
        
        var condoManager = dbContext
                               .Users
                               .AsNoTracking()
                               .FirstOrDefault(x => x.Role == UserRoleEnum.Manager && x.CondominiumId == space.CondominiumId)
            ?? throw new ResourceNotFoundException("Não foi encontrado síndico no condomínio.");
        
        var notification = notificationService.SetupOneUserNotification
        (
            user.Condominium!.Name,
            token,
            NotificationTypeEnum.BookingCreated,
            "Uma nova reserva foi solicitada!",
            NotificationResultEnum.Info,
            $"{user.Name} (apto. {userApartment}) solicitou uma reserva no espaço: {space.Name}.",
            DateTime.UtcNow,
            condoManager.Id,
            booking.Id
        );
        
        var emailMessage = emailService.SetupOneUserEmailMessage
        (
            "Uma nova reserva foi solicitada!",
            condoManager.Name,
            $"{user.Name} (apto. {userApartment}) solicitou uma reserva no espaço: {space.Name}.",
            $"https://localhost:7136/reserva/{booking.Id}",
            condoManager.Email,
            condoManager.NotifyEmail
        );

        notificationService.PublishNotification(notification, emailMessage);
    }

    public void Delete(int id, string userToken, bool deletedByManager)
    {
        var booking = dbContext
            .Booking
            .Include(x => x.Space)
            .Include(x => x.User)
            .ThenInclude(y => y!.Condominium)
            .FirstOrDefault(x => x.Id == id) 
                    ?? throw new ResourceNotFoundException("O agendamento não foi encontrado");
        
        dbContext.Booking.Where(x => x.Id == id).ExecuteDelete();
        notificationService.DeleteBookingNotifications(id);
        dbContext.SaveChanges();

        if (deletedByManager)
        {
            var notification = notificationService.SetupOneUserNotification
            (
                booking.User!.Condominium!.Name,
                userToken,
                NotificationTypeEnum.BookingRejected,
                "A sua reserva foi cancelada.",
                NotificationResultEnum.Cancelled,
                $"O síndico cancelou a sua reserva no espaço: {booking.Space!.Name} para o dia {booking.InitialDate.ToShortDateString()}.",
                DateTime.UtcNow, 
                booking.UserId,
                booking.Id
            );

            var emailMessage = emailService.SetupOneUserEmailMessage
            (
                "A sua reserva foi cancelada.",
                booking.User!.Name,
                $"O síndico cancelou a sua reserva no espaço: {booking.Space.Name} para o dia {booking.InitialDate.ToShortDateString()}.",
                "https://localhost:7136/minhas-reservas",
                booking.User.Email,
                booking.User.NotifyEmail
            );

            notificationService.PublishNotification(notification, emailMessage);
        }
    }

    public void ApproveBooking(int id, string token)
    {
        var booking = dbContext
                          .Booking
                          .Include(x => x.User)
                          .FirstOrDefault(x => x.Id == id)
                        ?? throw new ResourceNotFoundException("A reserva não foi encontrada.");

        if (booking.Status == BookingStatusEnum.Pending)
        {
            booking.Status = BookingStatusEnum.Confirmed;
            dbContext.SaveChanges();
            
            var space = dbContext
                .Space
                .AsNoTracking()
                .Include(x => x.Condominium)
                .FirstOrDefault(x => x.Id == booking.SpaceId)
                ?? throw new ResourceNotFoundException("Não foi encontrado esse espaço no condomínio.");
            
            var notification = notificationService.SetupOneUserNotification
            (
                space.Condominium!.Name,
                token,
                NotificationTypeEnum.BookingApproved,
                "A sua reserva foi aprovada!",
                NotificationResultEnum.Approved,
                $"O síndico aprovou a sua reserva no espaço: {space.Name} para o dia {booking.InitialDate.ToShortDateString()}.",
                DateTime.UtcNow, 
                booking.UserId,
                booking.Id
            );

            var emailMessage = emailService.SetupOneUserEmailMessage
            (
                "A sua reserva foi aprovada!",
                booking.User!.Name,
                $"O síndico aprovou a sua reserva no espaço: {space.Name} para o dia {booking.InitialDate.ToShortDateString()}.",
                "https://localhost:7136/minhas-reservas",
                booking.User.Email,
                booking.User.NotifyEmail
            );

            notificationService.PublishNotification(notification, emailMessage);
        }
    }
}