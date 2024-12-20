using System.Net;
using System.Net.Http.Headers;
using BlazorApp.DTOs;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using MudBlazor.Extensions;

namespace BlazorApp.Services;

public class BookingService(HttpClient httpClient, ILocalStorageService localStorage)
{
    public async Task<List<Booking>> GetBookings(BookingFilter? filter = null)
    {
        try
        {
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var queryParams = string.Empty;
            if (filter != null)
            {
                queryParams += "?";
                if (filter.FinalDate.HasValue)
                    queryParams += $"filter.FinalDate={filter.FinalDate.Value:O}&";
            
                if (filter.InitialDate.HasValue)
                    queryParams += $"filter.InitialDate={filter.InitialDate.Value:O}&";
            
                if(filter.Status.HasValue)
                    queryParams += $"filter.Status={filter.Status}&";
            
                if (filter.UserId.HasValue)
                    queryParams += $"filter.UserId={filter.UserId}&";
            
                if (filter.SpaceId.HasValue)
                    queryParams += $"filter.SpaceId={filter.SpaceId}";
            }

            var response = await httpClient.GetAsync(queryParams);
            
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
            if (response.StatusCode == HttpStatusCode.NoContent)
                return [];
            
            var result = await response.Content.ReadFromJsonAsync<List<Booking>>();
            return result ?? [];
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<BookingDetailsDTO?> GetBooking(int bookingId)
    {
        var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync($"{bookingId}");
        if (!response.IsSuccessStatusCode)
        {
            await response.HandleResponseError();
        }        
        
        return await response.Content.ReadFromJsonAsync<BookingDetailsDTO>();
    }

    public async Task Add(Booking booking)
    {
        var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync(string.Empty, booking);

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }

    public async Task ApproveBooking(int bookingId)
    {
        var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PatchAsync($"{bookingId}/accept-booking", null);

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }

    public async Task DeleteBooking(int bookingId, bool isManager)
    {
        var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.DeleteAsync($"{bookingId}?deletedByManager={isManager}");

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }
}