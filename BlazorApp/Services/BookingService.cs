using System.Net;
using BlazorApp.Models;
using BlazorApp.Utils;

namespace BlazorApp.Services;

public class BookingService(HttpClient httpClient)
{
    public async Task<List<Booking>?> GetBookings(BookingFilter? filter = null)
    {
        try
        {
            var queryParams = string.Empty;
            if (filter != null)
            {
                queryParams += "?";
                if (filter.FinalDate.HasValue)
                    queryParams += $"filter.FinalDate={filter.FinalDate}&";
            
                if (filter.InitialDate.HasValue)
                    queryParams += $"filter.InitialDate={filter.InitialDate}&";
            
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
                return null;
            
            return await response.Content.ReadFromJsonAsync<List<Booking>>();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task Add(Booking booking)
    {
        var response = await httpClient.PostAsJsonAsync(string.Empty, booking);

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }
}