using System.Net.Http.Headers;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;

namespace BlazorApp.Services;

public class NotificationService(HttpClient httpClient, ILocalStorageService localStorage)
{
    public async Task<List<Notification>?> GetAll(NotificationFilter filter)
    {
        try
        {
            var queryParameters = $"?filter.UserId={filter.UserId}";
            if (filter.IsBookings)
                queryParameters += "&filter.IsBookings=true";
            else if (filter.IsVotings)
                queryParameters += "&filter.IsVotings=true";
            else if (filter.IsPosts)
                queryParameters += "&filter.IsPosts=true";
            else if (filter.IsGeneralAnnouncements)
                queryParameters += "&filter.IsGeneralAnnouncements=true";
            else if (filter.IsFinancial)
                queryParameters += "&filter.IsFinancial=true";
            
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

    public async Task MarkNotificationsAsReaded(int? userId, int firstOpenNotificationId)
    {
        var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PatchAsync($"{userId}/mark-as-readed?firstOpenNotificationId={firstOpenNotificationId}", null);

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }
}