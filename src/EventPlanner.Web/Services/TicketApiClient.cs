using System.Net;
using EventPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Web.Services;

public class TicketApiClient
{
    private readonly HttpClient _http;

    public TicketApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<TicketVm?> GetByIdAsync(int id)
    {
        var resp = await _http.GetAsync($"api/Ticket/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TicketVm>();
    }

    public async Task<List<TicketBookVm?>> GetByEventIdAsync(int eventId)
    {
        var resp = await _http.GetAsync($"api/Ticket/byEvent/{eventId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<TicketBookVm>>();
    }

    public async Task<List<TicketBookVm>> GetAllAsync()
    {
        var resp = await _http.GetAsync($"api/Ticket");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<TicketBookVm>>();
    }

    public async Task<ApiResult> CreateAsync(UpsertTicketVm dto)
    {
        var resp = await _http.PostAsJsonAsync("api/Ticket", dto);

        if (resp.IsSuccessStatusCode)
            return new ApiResult(true, null);

        if (resp.StatusCode == HttpStatusCode.Conflict) return new ApiResult(false, "Ticket already exists.");
        return new ApiResult(false, $"Unexpected error: {resp.StatusCode} ");
    }

    public async Task<ApiResult> UpdateAsync(int id, UpsertTicketVm dto)
    {
        var resp = await _http.PutAsJsonAsync($"api/Ticket/{id}", dto);
        if (resp.IsSuccessStatusCode) return new ApiResult(true, null);
        if (resp.StatusCode == HttpStatusCode.NotFound) return new ApiResult(false, "Ticket not found.");
        if (resp.StatusCode == HttpStatusCode.Conflict) return new ApiResult(false, "Ticket already exists.");

        return new ApiResult(false, $"Unexpected error: {resp.StatusCode} ");
    }

    public async Task DeleteAsync(int id)
    {
        await _http.DeleteAsync($"api/Ticket/{id}");
    }
}