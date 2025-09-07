using System.Net.Http.Json;
using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class LocationsApiClient
{
    private readonly HttpClient _http;
    public LocationsApiClient(HttpClient http) => _http = http;

    public async Task<List<LocationVm>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<LocationVm>>("api/location") ?? new();

    public async Task<LocationVm?> GetAsync(int id) =>
        await _http.GetFromJsonAsync<LocationVm>($"api/Location/{id}");

    public async Task CreateAsync(CreateLocationVm dto) =>
        await _http.PostAsJsonAsync("api/Location", dto);

    public async Task UpdateAsync(int id, UpdateLocationVm dto) =>
        await _http.PutAsJsonAsync($"api/Location/put/{id}", dto);

    public async Task DeleteAsync(int id) =>
        await _http.DeleteAsync($"api/Location/delete/{id}");
}