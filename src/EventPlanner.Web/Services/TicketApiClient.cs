using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class TicketApiClient
{
    private readonly HttpClient _http;
    public TicketApiClient(HttpClient http) => _http = http;
    
    public async Task<TicketVm?> GetByIdAsync(int id) =>
        await _http.GetFromJsonAsync<TicketVm>($"api/Ticket/{id}");
    
    public async Task CreateAsync(UpsertTicketVm dto) =>
        await _http.PostAsJsonAsync("api/Ticket", dto);
    
    public async Task UpdateAsync(int id, UpsertTicketVm dto) =>
    await _http.PutAsJsonAsync($"api/Ticket/{id}", dto);
    
    public async Task DeleteAsync(int id) =>
        await _http.DeleteAsync($"api/Ticket/{id}");
}