using System.Net;
using System.Net.Http.Headers;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;
using Microsoft.Extensions.Primitives;
using Extensions = BlazorApp.Utils.Extensions;

namespace BlazorApp.Services;

public class SpaceService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    public SpaceService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<List<Space>?> GetCondominiumSpaces(SpaceFilter filter)
    {
        try
        {
            var queryParameters = string.Empty;
            if (filter.CondominiumId != null) {
                queryParameters += $"?filter.CondominiumId={filter.CondominiumId}";
            }
            if (filter.Availability != null) {
                queryParameters += $"&filter.Availability={filter.Availability}";
            }
            if (filter.Name != null) {
                queryParameters += $"&filter.Name={filter.Name}";
            }

            var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetFromJsonAsync<List<Space>>(queryParameters);
            
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<Photo?> GetSpacePhoto(int id)
    {
        var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.GetAsync($"{id}/photo");
        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();

        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;
        
        return await response.Content.ReadFromJsonAsync<Photo>();
    }

    public async Task Delete(int id)
    {
        var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await _httpClient.DeleteAsync($"{id}");
        
        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }

    public async Task Add(Space space)
    {
        var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.PostAsJsonAsync("", space);
        
        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }
    
    public async Task Update(Space space)
    {
        var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.PatchAsJsonAsync($"{space.Id}", space);

        if (!response.IsSuccessStatusCode)
            await response.HandleResponseError();
    }
}