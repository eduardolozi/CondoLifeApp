using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorApp.Models;
using Blazored.LocalStorage;

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
            var accessToken = await _localStorage.GetItemAsStringAsync("authToken");
            if (accessToken is null) return null;
            
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
                Condominium = new Condominium { Name = claims.GetValueOrDefault("CondominiumName") }
            };
        }
    }
}
