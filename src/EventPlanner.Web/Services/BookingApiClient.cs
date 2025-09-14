using System.Net;
using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class BookingApiClient
{
    private readonly HttpClient _http;
    public BookingApiClient(HttpClient http) => _http = http;

    public async Task<BookingVm?> GetByIdAsync(int id)
    {
        var resp = await _http.GetAsync($"api/Booking/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<BookingVm>();
    }

    public async Task<List<BookingVm>> GetAllActiveBookingsAsync() =>
        await _http.GetFromJsonAsync<List<BookingVm>>($"api/Booking/allActiveBookings") ?? new();
    
    public async Task<List<MyBookingVm>> GetByUserIdAsync(int userId)
    {
        var resp = await _http.GetAsync($"api/Booking/byUser/{userId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return new List<MyBookingVm>();
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<MyBookingVm>>();
    }
    public async Task<List<BookingVm>> GetByEventIdAsync(int eventId)
    {
        var resp = await _http.GetAsync($"api/Booking/byEventId/{eventId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<BookingVm>>();
    }

    public async Task<BookingVm?> GetByUserAndTicketAsync(int UserId, int TicketId)
    {
        var resp = await _http.GetAsync($"api/Booking/ByUserAndTickets/{UserId}/{TicketId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<BookingVm>();
    }
       
    
    public async Task<BookingVm?> CreateAsync(UpsertBookingVm booking)
    {
        var response = await _http.PostAsJsonAsync("api/Booking", booking);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<BookingVm>();
        return null;
    }
    
    public async Task CancelAsync(int id) =>
    await _http.DeleteAsync($"api/Booking/cancel/{id}");
    
    public async Task DeleteAsync(int id) =>
        await _http.DeleteAsync($"api/Booking/deleteBooking/{id}");
}