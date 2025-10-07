namespace EventPlanner.AuthAPI;

public class DomainApiClient
{
    private readonly HttpClient _http;
    public DomainApiClient(HttpClient http) => _http = http;
    public Task<HttpResponseMessage> CreateUserAsync(object payload) =>
        _http.PostAsJsonAsync("api/user", payload);
}