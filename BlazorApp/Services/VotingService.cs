using System.Net;
using System.Net.Http.Headers;
using BlazorApp.Models;
using BlazorApp.Utils;
using Blazored.LocalStorage;

namespace BlazorApp.Services;

public class VotingService(HttpClient httpClient, ILocalStorageService localStorage)
{
    public async Task<List<Voting>> GetAllVotings(VotingFilter filter)
    {
        try
        {
            var queryParameters = $"?filter.CondominiumId={filter.CondominiumId}";
            if (filter.BaseDate.HasValue) {
                queryParameters += $"&filter.BaseDate={filter.BaseDate}";
            }
            if (filter.UserId.HasValue) {
                queryParameters += $"&filter.UserId={filter.UserId}";
            }
            queryParameters += $"&filter.IsOpened={filter.IsOpened}";
            
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(queryParameters);
            
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
            if (response.StatusCode is HttpStatusCode.NoContent)
                return [];
            
            var result = await response.Content.ReadFromJsonAsync<List<Voting>>();

            return result!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<Voting> GetVotingById(int votingId, int userId)
    {
        try
        {
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync($"{votingId}?userId={userId}");
            
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
            if (response.StatusCode is HttpStatusCode.NoContent)
                throw new ApplicationException("Você já participou desta votação.");
            
            return await response.Content.ReadFromJsonAsync<Voting>()
                ?? throw new ApplicationException("Esta votação não foi encontrada");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
    
    public async Task<Voting> GetVotingDetails(int id)
    {
        try
        {
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync($"{id}/details");
            
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
            if (response.StatusCode is HttpStatusCode.NoContent)
                throw new ApplicationException("Esta votação não foi encontrada");
            
            return await response.Content.ReadFromJsonAsync<Voting>()
                ?? throw new ApplicationException("Esta votação não foi encontrada");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task CreateVoting(Voting voting)
    {
        try
        {
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync(string.Empty, voting);
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task ConfirmVote(Vote vote)
    {
        try
        {
            var accessToken = await localStorage.GetItemAsStringAsync("accessToken");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync("confirm-vote", vote);
            if (!response.IsSuccessStatusCode)
                await response.HandleResponseError();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}