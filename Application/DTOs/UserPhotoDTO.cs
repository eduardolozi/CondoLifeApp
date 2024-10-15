using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace Application.DTOs {
    [NotMapped]
    public class UserPhotoDTO {
        public byte[] PhotoBytes { get; set; }
        public string ContentType { get; set; }
    }
}
