using Domain.Models;
using Infraestructure;

namespace Application.Services;

public class SpaceService
{
    private readonly CondoLifeContext _dbContext;
    public SpaceService(CondoLifeContext dbContext)
    {
        _dbContext = dbContext; 
    }

    public void Insert(Space space)
    {
        
    }

    public void Update(int id, Space space)
    {
        
    }

    public void Delete(int id)
    {
        
    }

    public Space GetById(int id)
    {
        return null;
    }

    public List<Space> GetAll(SpaceFilter? filter)
    {
        return null;
    }
}