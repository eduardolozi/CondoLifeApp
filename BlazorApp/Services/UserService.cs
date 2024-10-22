using BlazorApp.Models;
using BlazorApp.Utils;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;

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
