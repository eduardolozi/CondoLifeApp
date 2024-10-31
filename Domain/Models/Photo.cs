using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Domain.Models {
    [NotMapped]
    public class Photo {
        public Photo(string id, string fileName, string contentType)
        {
            Id = id;
            FileName = fileName;
            ContentType = contentType;
        }

        public Photo()
        {
            
        }

        public string? Id { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
        public string? ContentBase64 { get; set; }
    }
}
