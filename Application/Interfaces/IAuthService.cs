using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces {
    public interface IAuthService {
        public LoginResponseDTO? Login(UserLoginDTO userLogin);
        public string CreateAccessToken(User user);
        public RefreshToken CreateRefreshToken(int userId);
        public LoginResponseDTO RefreshAccessToken(RefreshRequestDTO refreshRequest);
    }
}
