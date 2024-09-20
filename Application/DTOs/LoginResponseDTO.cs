using Domain.Models;

namespace Application.DTOs {
    public class LoginResponseDTO {
        public required string AccessToken { get; set; }
        public required RefreshToken RefreshToken { get; set; }
    }
}
