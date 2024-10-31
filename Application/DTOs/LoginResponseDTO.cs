using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs {
    [NotMapped]
    public class LoginResponseDTO {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
