using Domain.Models;
using Domain.Models.Filters;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class NotificationService(CondoLifeContext dbContext)
{
    public void Insert(Notification notification)
    {
        dbContext.Notification.Add(notification);
        dbContext.NotificationPayload.Add(notification.Message);
        dbContext.SaveChanges();
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
}