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

    public List<Notification>? Get(NotificationFilter? filter = null)
    {
        var query = dbContext
            .Notification
            .Include(x => x.Message)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if(filter.UserId.HasValue) 
                query = query.Where(x => x.UserId == filter.UserId);
            if(filter.NotificationType.HasValue) 
                query = query.Where(x => x.NotificationType == filter.NotificationType);
        }
        
        return query.OrderByDescending(x => x.Id).ToList();
    }

    public Notification? GetById(int id)
    {
        return dbContext
            .Notification
            .Include(x => x.Message)
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public void MarkAsReaded(int userId, int firstOpenNotificationId)
    {
        var notifications = dbContext.Notification
            .Where(x => x.UserId == userId && x.Id >= firstOpenNotificationId && !x.IsRead)
            .ToList();

        if (notifications.Any())
        {
            foreach (var notification in notifications)
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