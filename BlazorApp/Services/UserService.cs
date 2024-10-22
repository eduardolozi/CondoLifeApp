using BlazorApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp.Services {
	public class UserService {
		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _memoryCache;
		public UserService(HttpClient httpClient, IMemoryCache memoryCache) {
			_httpClient = httpClient;
			_memoryCache = memoryCache;
		}

		public async Task CreateUser(User user) {
			try {
				user.Id = 0;
				var response = await _httpClient.PostAsJsonAsync("", user);
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}
	}
}
