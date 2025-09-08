using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class EventsApiClient
{
    private readonly HttpClient _http;
    public EventsApiClient(HttpClient http) => _http = http;

    public async Task<List<EventVm>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<EventVm>>("api/event") ?? new ();
    
    public async Task<EventVm?> GetAsync(int id) =>
        await _http.GetFromJsonAsync<EventVm>($"api/event/{id}");
    
    public async Task CreateAsync(CreateEventVm dto) =>
        await _http.PostAsJsonAsync("api/event", dto);
    
    public async Task UpdateAsync(int id, UpdateEventVm dto) =>
    await _http.PutAsJsonAsync("api/event", dto);
    
    public async Task DeleteAsync(int id) =>
        await _http.DeleteAsync($"api/event/{id}");
    
    

}