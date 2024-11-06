using BlazorApp.Models;

namespace BlazorApp.Services {
	public class UserService {
		private readonly HttpClient _httpClient;
		public UserService(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		public async Task Create(User user) {
			try {
				user.Id = 0;
				var response = await _httpClient.PostAsJsonAsync("", user);
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}

		public async Task<Photo?> GetPhoto(string? param)
		{
			try {
				var photo = await _httpClient.GetFromJsonAsync<Photo>(param);
				return photo;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}

		public async Task<LoginResponse?> Update(User user)
		{
			try
			{
				var response = await _httpClient.PatchAsJsonAsync($"{user.Id}", user);
				response.EnsureSuccessStatusCode();
				var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
				return loginResponse;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}
	}
}
