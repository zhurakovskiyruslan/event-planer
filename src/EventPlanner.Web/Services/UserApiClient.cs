using System.Net;
using EventPlanner.Web.Models;
namespace EventPlanner.Web.Services;

public class UserApiClient
{
    private readonly HttpClient _http;
    public UserApiClient(HttpClient http) => _http = http;
    
    public async Task<UserVm?>  GetByIdAsync(int id)
    {
        var resp = await _http.GetAsync($"api/user/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<UserVm>();
    }
    
    public async Task<UserVm?>  GetByEmailAsync(string email)
    {
        var resp = await _http.GetAsync($"api/user/by-email/{email}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<UserVm>();
    }
    public async Task<UserVm?> CreateAsync(UpsertUserVm dto)
    {
        var resp = await _http.PostAsJsonAsync("api/user", dto);
        if(resp.IsSuccessStatusCode)  return await resp.Content.ReadFromJsonAsync<UserVm>();
        return null;
    }

    public async Task<UserVm?> UpdateAsync(int id, UpsertUserVm dto)
    {
        var resp = await _http.PutAsJsonAsync($"api/user/{id}", dto);
        if (resp.IsSuccessStatusCode) return await resp.Content.ReadFromJsonAsync<UserVm>();
        return null;
    }
    
    public async Task DeleteAsync(int id) =>
    await _http.DeleteAsync($"api/user/{id}");

}