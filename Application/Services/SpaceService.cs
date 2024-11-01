using Application.Validators;
using Domain.Exceptions;
using Domain.Models;
using Domain.Utils;
using FluentValidation;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents;

namespace Application.Services;

public class SpaceService
{
    private readonly CondoLifeContext _dbContext;
    private readonly IDocumentStore _ravenStore;
    private readonly IValidator<Space> _spaceValidator;
    public SpaceService(CondoLifeContext dbContext,
        IDocumentStore ravenStore,
        IValidator<Space> spaceValidator)
    {
        _dbContext = dbContext; 
        _ravenStore = ravenStore;
        _spaceValidator = spaceValidator;
    }

    private void SavePhoto(Space space)
    {
        var fileName = space.Photo!.FileName.Replace(' ', '-').Replace('.', '-');
        var photoBytes = Convert.FromBase64String(space.Photo.ContentBase64!);

        using var session = _ravenStore.OpenSession();
        var spacePhoto = new Photo($"space/{space.Id}", fileName, space.Photo.ContentType!);
        var document = session.Load<Photo>(spacePhoto.Id);
        if (document is not null)
        {
            document.ContentType = spacePhoto.ContentType;
            document.FileName = spacePhoto.FileName;
        }
        else
        {
            session.Store(spacePhoto);
        }
        session.SaveChanges();
            
        session.Advanced.Attachments.Delete(spacePhoto.Id, spacePhoto.FileName);
        session.SaveChanges();
            
        session.Advanced.Attachments.Store(spacePhoto.Id, spacePhoto.FileName, new MemoryStream(photoBytes), spacePhoto.ContentType);
        session.SaveChanges();

        space.PhotoUrl = $"https://localhost:7031/api/space/{space.Id}/photo?fileName={fileName}";
        _dbContext.SaveChanges();
    }

    public void Insert(Space space)
    {
        _spaceValidator.ValidateAndThrow(space);
        _dbContext.Space.Add(space);
        _dbContext.SaveChanges();

        if (space.Photo.HasValue())
        {
            SavePhoto(space);
        }
    }

    public void Update(int id, Space space)
    {
        _spaceValidator.ValidateAndThrow(space);
        var spaceDb = GetById(id);
        spaceDb.Name = space.Name;
        _dbContext.SaveChanges();
        
        if (space.Photo.HasValue())
        {
            SavePhoto(space);
        }
    }

    public void Delete(int id)
    {
        var space = GetById(id);
        if (space.PhotoUrl.HasValue())
        {
            using var session = _ravenStore.OpenSession();
            session.Delete(space);
            session.SaveChanges();
        }
        _dbContext.Space.Remove(space);
        _dbContext.SaveChanges();
    }

    public Space GetById(int id)
    {
        return _dbContext.Space.FirstOrDefault(s => s.Id == id)
            ?? throw new ResourceNotFoundException("Espaço não foi encontrado");
    }

    public List<Space> GetAll(SpaceFilter? filter)
    {
        var query = _dbContext.Space.AsNoTracking().AsQueryable();
        if (filter != null)
        {
            if (filter.CondominiumId.HasValue())
            {
                query = query.Where(s => s.CondominiumId == filter.CondominiumId);
            }

            if (filter.Name.HasValue())
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }
        }
        
        return query.ToList();
    }
}