using System.Xml.Linq;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Infraestructure;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents;

namespace Application.Services;

public class PostService(CondoLifeContext dbContext, IDocumentStore ravenStore)
{
    public List<Post> GetAll(PostFilter? filter = null)
    {
        var query = dbContext.Post.AsNoTracking().Include(x => x.Medias).AsQueryable();
        if (filter != null)
        {
            if(filter.CondominiumId.HasValue)
                query = query.Where(p => p.Id == filter.CondominiumId);

            if (filter.UserId.HasValue)
                query = query.Where(p => p.Id == filter.UserId);
            
            if(filter.PostCategory.HasValue)
                query = query.Where(x => x.Category == filter.PostCategory);
        }
        return query.ToList();
    }
    
    public void Create(Post post)
    {
        dbContext.Post.Add(post);
        dbContext.SaveChanges();

        if (post.Photos.HasValue())
        {
            if (post.Photos.Count > 0)
            {
                SaveMedias(post);
            }
        }
    }

    private void SaveMedias(Post post)
    {
        List<Photo> photos = [];
        using var session = ravenStore.OpenSession();
        foreach (var photo in post.Photos)
        {
            var fileName = photo.FileName.Replace(' ', '-').Replace('.', '-');
            var photoBytes = Convert.FromBase64String(photo.ContentBase64!);
            
            var photoObj = new Photo($"user/{post.Id}", fileName, photo.ContentType!);
            session.Store(photoObj);
            session.SaveChanges();
            
            session.Advanced.Attachments.Store(photoObj.Id, photoObj.FileName, new MemoryStream(photoBytes), photoObj.ContentType);
            
            post.Medias.Add(new PostMedias
            {
                Url = $"https://localhost:7031/api/Post/{post.Id}/photo?fileName={fileName}"
            });
        }
        session.SaveChanges();
        dbContext.SaveChanges();
    }
}