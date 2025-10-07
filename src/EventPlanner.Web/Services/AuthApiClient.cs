using EventPlanner.Web.Models;

namespace EventPlanner.Web.Services;

public class AuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthRespVm?> LoginAsync(LoginReqVm loginReq)
    {
        return await _http.PostAsJsonAsync("api/user/login", loginReq).Result.Content
            .ReadFromJsonAsync<AuthRespVm>();
    }

    public async Task<AuthRespVm?> RegisterAsync(RegisterReqVm registerReq)
    {
        return await _http.PostAsJsonAsync("api/user/register", registerReq).Result.Content
            .ReadFromJsonAsync<AuthRespVm>();
    }

    public async Task<AuthRespVm?> ChangePasswordAsync(ChangePasswordReqVm changeReq)
    {
        return await _http.PostAsJsonAsync("api/user/changePassword", changeReq).Result.Content
            .ReadFromJsonAsync<AuthRespVm>();
    }
}