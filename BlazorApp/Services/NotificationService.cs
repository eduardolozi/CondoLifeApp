using System.Net.Http.Headers;
using BlazorApp.Models;
using Blazored.LocalStorage;

namespace BlazorApp.Services;

public class NotificationService(HttpClient httpClient, ILocalStorageService localStorage)
{
    public async Task<List<Notification>?> GetAll(NotificationFilter filter)
    {
        try
        {
            var queryParameters = string.Empty;
            if (filter.UserId != null) {
                queryParameters += $"?filter.UserId={filter.UserId}";
            }
            if (filter.NotificationType != null) {
                queryParameters += $"&filter.NotificationType={filter.NotificationType}";
            }
            
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<List<Notification>>(queryParameters);
            
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}