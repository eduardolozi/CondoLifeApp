using Domain.Enums;
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
        var query = dbContext.Booking.AsNoTracking().Include(x => x.User).AsQueryable();
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
            Status = x.Status,
            InitialDate = x.InitialDate,
            FinalDate = x.FinalDate,
            UserId = x.UserId,
            Username = x.User.Name,
            Description = x.Description
        }).OrderBy(x => x.InitialDate).ToList();
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

        var user = dbContext.Users.AsNoTracking().First(x => x.Id == booking.UserId);
        var block = user.Block.HasValue() ? $"-{user.Block}" : string.Empty;
        var userApartment = $"{user.Apartment}{block}";
        var notification = new Notification
        {
            UserToken = token,
            NotificationType = NotificationTypeEnum.BookingCreated,
            Message = new NotificationPayload
            {
                Header = "Uma nova reserva foi solicitada!",
                Body = $"{user.Name} (apto. {userApartment}) solicitou uma reserva."
            }
        };
        rabbitService.Send(notification, RabbitConstants.NOTIFICATION_EXCHANGE, RabbitConstants.NOTIFICATION_ROUTING_KEY);
    }
}