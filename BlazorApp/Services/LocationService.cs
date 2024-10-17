using BlazorApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp.Services {
	public class LocationService {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        public LocationService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        private async Task<string> GetAccessToken() {
            if(!_memoryCache.TryGetValue("locationApiAccessToken", out string? accessToken)) {
                _httpClient.DefaultRequestHeaders.Add("api-token", "RgC6p7KVHCZYz3ZgmCOych4F-V8GfZvek4bv46pnqABhUlmbYgtIo7yWAOL0QC-SVJ0");
                _httpClient.DefaultRequestHeaders.Add("user-email", "eduardoellozi2022@gmail.com");
                var response = await _httpClient.GetAsync("getaccesstoken");
                response.EnsureSuccessStatusCode();
                var accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
                accessToken = accessTokenResponse.auth_token;
                _memoryCache.Set("locationApiAccessToken", accessToken, TimeSpan.FromMinutes(720));
            }
            return accessToken!;
        }

        public async Task<List<string>> GetCountries() {
            var accessToken = await GetAccessToken();

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await _httpClient.GetAsync("countries");
            var x = response.StatusCode;
            response.EnsureSuccessStatusCode();
            var countries = await response.Content.ReadFromJsonAsync<List<Country>>();
            var countryNames = countries!.Select(country => country.country_name).ToList();
			return countryNames;
		}
        
        public async Task<List<string>> GetStates(string country) {
            var accessToken = await GetAccessToken();

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await _httpClient.GetAsync($"states/{country}");
            var x = response.StatusCode;
            response.EnsureSuccessStatusCode();
            var states = await response.Content.ReadFromJsonAsync<List<State>>();
            var stateNames = states!.Select(state => state.state_name).ToList();
			return stateNames;
		}

    }

    public class AccessTokenResponse {
        public string auth_token {  get; set; }
    }
}
