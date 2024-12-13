using System.IdentityModel.Tokens.Jwt;
using BlazorApp.Enums;
using BlazorApp.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;

namespace BlazorApp.Services {
    public class AuthService {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly UserService _userService;
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, UserService userService) {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _userService = userService;
        }

        public async Task<LoginResponse?> Login(UserLoginRequest loginData) {
            var response = await _httpClient.PostAsJsonAsync("login", loginData);
            response.EnsureSuccessStatusCode();
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResponse;
        }

        public async Task<User?> GetUserByClaims()
        {
            var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
            if(string.IsNullOrEmpty(accessToken)) throw new AuthenticationFailureException("Usuário não autenticado. Por favor faça login para continuar!");

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(accessToken);
            var claims = decodedToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            
            return new User
            {
                Id = claims.TryGetValue("Id", out var id) ? Convert.ToInt32(id) : 0,
                Name = claims.TryGetValue("unique_name", out var name) ? name : string.Empty,
                Email = claims.TryGetValue("email", out var email) ? email : string.Empty,
                Apartment = claims.TryGetValue("Apartment", out var apartment) ? Convert.ToInt32(apartment) : 0,
                Block = claims.GetValueOrDefault("Block"),
                PhotoUrl = claims.TryGetValue("PhotoUrl", out var photoUrl) ? photoUrl : string.Empty,
                Condominium = new Condominium { Name = claims.GetValueOrDefault("CondominiumName") },
                Role = claims.TryGetValue("role", out var role) ? Enum.Parse<UserRoleEnum>(role) : UserRoleEnum.Resident,
                CondominiumId = claims.TryGetValue("CondominiumId", out var condominiumId) ? Convert.ToInt32(condominiumId) : 0,
                NotificationLifetime = claims.TryGetValue("NotificationLifetime", out var notificationLifetime) ? Convert.ToInt32(notificationLifetime) : null,
                NotifyEmail = claims.TryGetValue("NotifyEmail", out var notifyEmail) && Convert.ToBoolean(notifyEmail),
                NotifyPhone = claims.TryGetValue("NotifyPhone", out var notifyPhone) && Convert.ToBoolean(notifyPhone),
            };
        }
    }
}
