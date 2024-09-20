using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs {
    [NotMapped]
    public class UserLoginDTO {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
