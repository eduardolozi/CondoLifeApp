using BlazorApp.Models;

namespace BlazorApp.Services {
	public class UserService {
		private readonly HttpClient _httpClient;
		public UserService(HttpClient httpClient) {
			_httpClient = httpClient;
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
