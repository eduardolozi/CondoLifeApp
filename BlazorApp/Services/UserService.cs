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

		public async Task<UserPhoto?> GetPhoto(string? param)
		{
			try {
				var photo = await _httpClient.GetFromJsonAsync<UserPhoto>(param);
				return photo;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}

		public async Task Update(User user)
		{
			try
			{
				var response = await _httpClient.PatchAsJsonAsync($"{user.Id}", user);
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}
	}
}
