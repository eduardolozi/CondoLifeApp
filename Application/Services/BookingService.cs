using Domain.Models;
using Infraestructure;

namespace Application.Services;

public class BookingService
{
    private readonly CondoLifeContext _dbContext;
    public BookingService(CondoLifeContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Create(Booking booking)
    {
        
        _dbContext.Add(booking);
    }
}