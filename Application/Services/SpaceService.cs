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
        var spaceDb = GetById(id) ?? throw new ResourceNotFoundException("Espaço não encontrado.");
        spaceDb.Name = space.Name;
        spaceDb.Availability = space.Availability;
        _dbContext.SaveChanges();
        
        if (space.Photo.HasValue())
        {
            SavePhoto(space);
        }
    }

    public void Delete(int id)
    {
        var space = GetById(id) ?? throw new ResourceNotFoundException("Espaço não encontrado.");
        if (space.PhotoUrl.HasValue())
        {
            using var session = _ravenStore.OpenSession();
            var photoId = $"space/{space.Id}";
            session.Delete(photoId);
            session.SaveChanges();
        }
        _dbContext.Space.Remove(space);
        _dbContext.SaveChanges();
    }

    public Space? GetById(int id)
    {
        return _dbContext.Space.FirstOrDefault(s => s.Id == id);
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

            if (filter.Availability.HasValue())
            {
                query = query.Where(s => s.Availability == filter.Availability);
            }

            if (filter.Name.HasValue())
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }
        }
        
        return query.ToList();
    }

    public string? GetSpacePhoto(int id)
    {
        var session = _ravenStore.OpenSession();
        var docId = $"space/{id}";
        var spacePhoto = session.Query<Photo>().FirstOrDefault(x => x.Id == docId);
        if(spacePhoto is null) return null;
            
        byte[] bytes;
        List<byte> totalStream = new();
        var buffer = new byte[128];
        int read;

        using var photoAttachment = session.Advanced.Attachments.Get(docId, spacePhoto.FileName);
        while ((read = photoAttachment.Stream.Read(buffer, 0, buffer.Length)) > 0) {
            totalStream.AddRange(buffer.Take(read));
        }

        return Convert.ToBase64String(totalStream.ToArray());
    }
}