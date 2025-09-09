using EventPlanner.Web.Models;
namespace EventPlanner.Web.Services;

public class UserApiClient
{
    private readonly HttpClient _http;
    public UserApiClient(HttpClient http) => _http = http;
    
    public async Task<UserVm?>  GetByIdAsync(int id) =>
    await _http.GetFromJsonAsync<UserVm>($"api/user/{id}");
    
    public async Task<UserVm?>  GetByEmailAsync(string email) =>
        await _http.GetFromJsonAsync<UserVm>($"api/user/by-email/{email}");

    public async Task CreateAsync(UpsertUserVm dto) =>
        await _http.PostAsJsonAsync("api/user", dto);
    
    public async Task UpdateAsync(int id, UpsertUserVm dto) =>
    await _http.PutAsJsonAsync($"api/user/{id}", dto);
    
    public async Task DeleteAsync(int id) =>
    await _http.DeleteAsync($"api/user/{id}");

}