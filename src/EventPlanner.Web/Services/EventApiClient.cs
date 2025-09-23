using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class EventApiClient
{
    private readonly HttpClient _http;

    public EventApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EventVm>> GetAllAsync(int page, int size)
    {
        return await _http.GetFromJsonAsync<List<EventVm>>($"api/event?page={page}&size={size}") ?? new List<EventVm>();
    }

    public async Task<EventVm?> GetAsync(int id)
    {
        return await _http.GetFromJsonAsync<EventVm>($"api/event/{id}");
    }

    public async Task CreateAsync(UpsertEventVm dto)
    {
        await _http.PostAsJsonAsync("api/event", dto);
    }

    public async Task UpdateAsync(int id, UpsertEventVm dto)
    {
        await _http.PutAsJsonAsync($"api/event/{id}", dto);
    }

    public async Task DeleteAsync(int id)
    {
        await _http.DeleteAsync($"api/event/{id}");
    }
}