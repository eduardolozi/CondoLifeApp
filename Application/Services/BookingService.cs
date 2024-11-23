using Application.DTOs;
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

public class BookingService(CondoLifeContext dbContext, AbstractValidator<Booking> bookingValidator, RabbitService rabbitService)
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
                query = query.Where(x => x.InitialDate >= filter.InitialDate);

            if (filter.FinalDate.HasValue)
                query = query.Where(x => x.FinalDate <= filter.FinalDate);

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

        var user = dbContext.Users.AsNoTracking().Include(x => x.Condominium).First(x => x.Id == booking.UserId);
        var block = user.Block.HasValue() ? $"-{user.Block}" : string.Empty;
        var userApartment = $"{user.Apartment}{block}";
        
        var condoManager = dbContext.Users.AsNoTracking().FirstOrDefault(x => x.Role == UserRoleEnum.Manager)
            ?? throw new ResourceNotFoundException("Não foi encontrado síndico no condomínio.");
        
        var space = dbContext.Space.AsNoTracking().FirstOrDefault(x => x.Id == booking.SpaceId)
            ?? throw new ResourceNotFoundException("Não foi encontrado esse espaço no condomínio.");
        
        var notification = new Notification
        {
            CondominiumName = user.Condominium!.Name,
            UserId = condoManager.Id,
            UserToken = token,
            NotificationType = NotificationTypeEnum.BookingCreated,
            Message = new NotificationPayload
            {
                Header = "Uma nova reserva foi solicitada!",
                Body = $"{user.Name} (apto. {userApartment}) solicitou uma reserva no espaço: {space.Name}."
            },
            BookingId = booking.Id,
            CreatedAt = DateTime.Now
        };
        rabbitService.Send(notification, RabbitConstants.NOTIFICATION_EXCHANGE, RabbitConstants.NOTIFICATION_ROUTING_KEY);
    }

    public void ApproveBooking(int id, string token)
    {
        var booking = dbContext.Booking.Find(id)
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
            
            var notification = new Notification
            {
                CondominiumName = space.Condominium!.Name,
                UserId = booking.UserId,
                UserToken = token,
                NotificationType = NotificationTypeEnum.BookingApproved,
                Message = new NotificationPayload
                {
                    Header = "A sua reserva foi aprovada!",
                    Body = $"O síndico aprovou a sua reserva no espaço: {space.Name} para o dia {booking.InitialDate.ToShortDateString()}."
                },
                BookingId = booking.Id,
                CreatedAt = DateTime.Now
            };
            rabbitService.Send(notification, RabbitConstants.NOTIFICATION_EXCHANGE, RabbitConstants.NOTIFICATION_ROUTING_KEY);
        }
    }
}