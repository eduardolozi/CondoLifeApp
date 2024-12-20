using System.Net;
using System.Net.Http.Headers;
using BlazorApp.DTOs;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;

namespace BlazorApp.Services {
	public class UserService(HttpClient httpClient, ILocalStorageService localStorage)
	{
		public async Task<List<User>?> GetAll(UserFilter? filter = null)
		{
			try
			{
				var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				
				var queryParams = string.Empty;
				if (filter is not null)
				{
					queryParams += $"?";
					if (filter.CondominiumId.HasValue)
						queryParams += $"filter.CondominiumId={filter.CondominiumId}&";
                
					if (!string.IsNullOrEmpty(filter.Username))
						queryParams += $"filter.Username={filter.Username}&";
                
					if (filter.Role.HasValue)
						queryParams += $"filter.Role={filter.Role}&";
					
					if (filter.OnlyEmailVerified.HasValue)
						queryParams += $"filter.OnlyEmailVerified={filter.OnlyEmailVerified}";
				}
				var response = await httpClient.GetAsync(queryParams);
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
				var response = await httpClient.PostAsJsonAsync("", user);
				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}

		public async Task<Photo?> GetPhoto(string? param)
		{
			try {
				var photo = await httpClient.GetFromJsonAsync<Photo>(param);
				return photo;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message, ex);
			}
		}

		public async Task<LoginResponse?> UpdateUserData(UpdateUserDataDTO userData)
		{
			try
			{
				var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				var response = await httpClient.PatchAsJsonAsync($"{userData.Id}/user-data", userData);
				response.EnsureSuccessStatusCode();
				var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
				return loginResponse;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}
		
		public async Task<LoginResponse?> UpdateNotificationConfigs(UpdateUserNotificationConfigsDTO notificationConfigs)
		{
			try
			{
				var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				var response = await httpClient.PatchAsJsonAsync($"{notificationConfigs.Id}/user-notification-configs", notificationConfigs);
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
			var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			var response = await httpClient.PatchAsJsonAsync($"{changeUserRole.UserId}/change-role", changeUserRole);
			if(!response.IsSuccessStatusCode)
				await response.HandleResponseError();
		}
	}
}
