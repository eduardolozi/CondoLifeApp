using Application.DTOs;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Filters;
using Infraestructure;
using Infraestructure.Rabbit;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class NotificationService(CondoLifeContext dbContext, RabbitService rabbitService)
{
    public void Insert(Notification notification)
    {
        dbContext.Notification.Add(notification);
        dbContext.NotificationPayload.Add(notification.Message);
        dbContext.SaveChanges();
    }

    public void DeleteBookingNotifications(int bookingId)
    {
        dbContext.Notification.Where(x => x.BookingId == bookingId).ExecuteDelete();
    }

    public void DeleteBookingsNotifications(List<Booking> bookings)
    {
        if (!bookings.Any()) return;
        var bookingIds = bookings.Select(x => x.Id).ToList();
        var notifications = dbContext
            .Notification
            .Where(x => x.BookingId.HasValue && bookingIds.Contains(x.BookingId.Value))
            .ExecuteDelete();
    }

    public List<GetNotificationDTO>? Get(NotificationFilter filter = null)
    {
        var userNotifications = dbContext
            .UserNotification
            .Include(x => x.Notification)
            .ThenInclude(y => y.Message)
            .Where(x => x.UserId == filter.UserId)
            .AsNoTracking()
            .ToList();
        
        if(filter.NotificationType.HasValue) 
            userNotifications = userNotifications.Where(x => x.Notification.NotificationType == filter.NotificationType).ToList();
        
        return userNotifications
            .Select(x => new GetNotificationDTO
            {
                Id = x.Notification.Id,
                NotificationType = x.Notification.NotificationType,
                Message = x.Notification.Message,
                UserId = x.UserId,
                BookingId = x.Notification.BookingId,
                IsRead = x.IsRead,
                CreatedAt = x.Notification.CreatedAt
            })
            .OrderByDescending(x => x.Id)
            .ToList();
    }

    public Notification? GetById(int id)
    {
        return dbContext
            .Notification
            .Include(x => x.Message)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public bool HasUnreadNotifications(int userId)
    {
        return dbContext
            .UserNotification
            .Any(x => x.UserId == userId && x.IsRead == false);
    }

    public void MarkAsReaded(int userId, int firstOpenNotificationId)
    {
        var userNotifications = dbContext.UserNotification
            .Where(x => x.UserId == userId && x.NotificationId >= firstOpenNotificationId && !x.IsRead)
            .ToList();

        if (userNotifications.Any())
        {
            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
            }
            dbContext.SaveChanges();
        }
    }

    public void PublishNotification(Notification notification, EmailMessage? emailMessage = null)
    {
        rabbitService.Send(notification, RabbitConstants.NOTIFICATION_EXCHANGE, RabbitConstants.NOTIFICATION_ROUTING_KEY);

        if (emailMessage != null)
            rabbitService.Send(emailMessage, RabbitConstants.EMAIL_EXCHANGE, RabbitConstants.EMAIL_ROUTING_KEY);
    }
}