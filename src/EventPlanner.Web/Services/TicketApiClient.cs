using System.Net;
using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class TicketApiClient
{
    private readonly HttpClient _http;
    public TicketApiClient(HttpClient http) => _http = http;

    public async Task<TicketVm?> GetByIdAsync(int id)
    {
        var resp = await _http.GetAsync($"api/Ticket/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        resp.EnsureSuccessStatusCode();
        return await  resp.Content.ReadFromJsonAsync<TicketVm>();
    }
    
    public async Task<List<TicketBookVm?>> GetByEventIdAsync(int eventId)
    {
        var resp = await _http.GetAsync($"api/Ticket/byEvent/{eventId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<TicketBookVm>>();
    }
    
    public async Task CreateAsync(UpsertTicketVm dto) =>
        await _http.PostAsJsonAsync("api/Ticket", dto);
    
    public async Task UpdateAsync(int id, UpsertTicketVm dto) =>
    await _http.PutAsJsonAsync($"api/Ticket/{id}", dto);
    
    public async Task DeleteAsync(int id) =>
        await _http.DeleteAsync($"api/Ticket/{id}");
}