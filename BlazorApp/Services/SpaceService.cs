﻿using System.Net.Http.Headers;
using BlazorApp.Models;
using Blazored.LocalStorage;

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
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Photo>($"{id}/photo");
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            var accessToken = await _localStorage.GetItemAsStringAsync("accessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.DeleteAsync($"{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}