using Application.Validators;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using FluentValidation;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents;

namespace Application.Services;

public class SpaceService(
    CondoLifeContext dbContext,
    IDocumentStore ravenStore,
    NotificationService notificationService,
    AbstractValidator<Space> spaceValidator)
{
    private void SavePhoto(Space space)
    {
        var fileName = space.Photo!.FileName.Replace(' ', '-').Replace('.', '-');
        var photoBytes = Convert.FromBase64String(space.Photo.ContentBase64!);

        using var session = ravenStore.OpenSession();
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
        dbContext.SaveChanges();
    }

    public void Insert(Space space)
    {
        spaceValidator.ValidateAndThrow(space);
        dbContext.Space.Add(space);
        dbContext.SaveChanges();

        if (space.Photo.HasValue())
        {
            SavePhoto(space);
        }
    }

    public void Update(int id, Space space)
    {
        spaceValidator.ValidateAndThrow(space);
        var spaceDb = GetById(id) ?? throw new ResourceNotFoundException("Espaço não encontrado.");
        spaceDb.Name = space.Name;
        spaceDb.Availability = space.Availability;
        spaceDb.BookingPrice = space.BookingPrice;
        dbContext.SaveChanges();
        
        if (space.Photo.HasValue())
        {
            SavePhoto(space);
        }
    }

    public void Delete(int id)
    {
        var space = dbContext
                        .Space
                        .Include(x => x.Bookings)
                        .FirstOrDefault(s => s.Id == id)
                    ?? throw new ResourceNotFoundException("Espaço não encontrado.");
        
        if (space.PhotoUrl.HasValue())
        {
            using var session = ravenStore.OpenSession();
            var photoId = $"space/{space.Id}";
            session.Delete(photoId);
            session.SaveChanges();
        }

        if (space.Bookings != null)
            notificationService.DeleteBookingsNotifications(space.Bookings);
        dbContext.Space.Remove(space);
        dbContext.SaveChanges();
        
    }

    public Space? GetById(int id)
    {
        return dbContext.Space.FirstOrDefault(s => s.Id == id);
    }

    public List<Space> GetAll(SpaceFilter? filter)
    {
        var query = dbContext.Space.AsNoTracking().AsQueryable();
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

    public Photo? GetSpacePhoto(int id)
    {
        var session = ravenStore.OpenSession();
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

        return new Photo
        {
            FileName = spacePhoto.FileName,
            ContentBase64 = Convert.ToBase64String(totalStream.ToArray()),
            Id = spacePhoto.Id,
            ContentType = spacePhoto.ContentType
        };
    }
}