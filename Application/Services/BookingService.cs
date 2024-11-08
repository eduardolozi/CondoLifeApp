﻿using Domain.Enums;
using Domain.Models;
using Domain.Models.Filters;
using FluentValidation;
using FluentValidation.Results;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class BookingService(CondoLifeContext dbContext, AbstractValidator<Booking> bookingValidator)
{
    public List<Booking> GetAll(BookingFilter? filter)
    {
        var query = dbContext.Booking.AsNoTracking().AsQueryable();
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
        
        return query.ToList();
    }

    public void Create(Booking booking)
    {
        bookingValidator.ValidateAndThrow(booking);
        if (booking.Status != BookingStatusEnum.Pending)
        {
            var statusError = new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "O status de uma reserva criada deve ser pendente."
                }
            };
            
            throw new ValidationException(statusError);
        }
        
        dbContext.Booking.Add(booking);
        dbContext.SaveChanges();
    }
}