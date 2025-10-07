namespace EventPlanner.Web.Infrastructure;

public class BearerFromCookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _http;

    public BearerFromCookieHandler(IHttpContextAccessor http)
    {
        _http = http;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = _http.HttpContext?.Request.Cookies["Auth"];
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, ct);
    }
}