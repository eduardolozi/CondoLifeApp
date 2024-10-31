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

		public async Task<UserPhoto?> GetUserPhoto(int? id)
		{
			try {
				var photo = await _httpClient.GetFromJsonAsync<UserPhoto>($"{id}/photo");
				return photo;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}
	}
}
