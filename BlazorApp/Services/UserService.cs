using System.Net;
using System.Net.Http.Headers;
using BlazorApp.DTOs;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;

namespace BlazorApp.Services {
	public class UserService {
		private readonly HttpClient _httpClient;
		private readonly ILocalStorageService _localStorage;
		public UserService(HttpClient httpClient, ILocalStorageService localStorage) {
			_httpClient = httpClient;
			_localStorage = localStorage;
		}

		public async Task<List<User>?> GetAll(UserFilter? filter = null)
		{
			try
			{
				var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				
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
				var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

		public async Task ChangeRole(ChangeUserRoleDTO changeUserRole)
		{
			var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			var response = await _httpClient.PatchAsJsonAsync($"{changeUserRole.UserId}/change-role", changeUserRole);
			if(!response.IsSuccessStatusCode)
				await response.HandleResponseError();
		}
	}
}
