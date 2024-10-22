using BlazorApp.Models;

namespace BlazorApp.Services {
    public class AuthService {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse?> Login(UserLoginRequest loginData) {
            var response = await _httpClient.PostAsJsonAsync("login", loginData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }
}
