using System.Net;
using BlazorApp.Models;
using BlazorApp.Utils;

namespace BlazorApp.Services {
	public class UserService {
		private readonly HttpClient _httpClient;
		public UserService(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		public async Task<List<User>?> GetAll(UserFilter? filter = null)
		{
			try
			{
				var queryParams = string.Empty;
				if (filter is not null)
				{
					queryParams += $"?";
					if (filter.CondominiumId.HasValue)
						queryParams += $"filter.CondominiumId={filter.CondominiumId}&";
                
					if (!string.IsNullOrEmpty(filter.Username))
						queryParams += $"filter.Username={filter.Username}&";
                
					if (filter.Role.HasValue)
						queryParams += $"filter.Role={filter.Role}";
				}
				var response = await _httpClient.GetAsync(queryParams);
				if (!response.IsSuccessStatusCode)
					await response.HandleResponseError();

				if (response.StatusCode == HttpStatusCode.NoContent)
					return null;
            
				return await response.Content.ReadFromJsonAsync<List<User>>();
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
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
