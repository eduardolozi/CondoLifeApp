using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Domain.Models {
    [NotMapped]
    public class UserPhoto {
        public UserPhoto(string id, string fileName, string contentType)
        {
            Id = id;
            FileName = fileName;
            ContentType = contentType;
        }

        public string Id { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
    }
}
