using EventPlanner.Web.Infrastructure;
using EventPlanner.Web.Services;

namespace EventPlanner.Web;

public static class ApiClientsRegistration
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiSettings:BaseUrl"]!;
        var authUrl = config["ApiSettings:AuthApiUrl"]!;
        services.AddTransient<BearerFromCookieHandler>();
        services.AddHttpClient<LocationApiClient>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();
        services.AddHttpClient<EventApiClient>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();
        services.AddHttpClient<TicketApiClient>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();
        services.AddHttpClient<UserApiClient>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();
        services.AddHttpClient<BookingApiClient>(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();

        services.AddHttpClient<AuthApiClient>(c => c.BaseAddress = new Uri(authUrl))
            .AddHttpMessageHandler<BearerFromCookieHandler>();

        return services;
    }
}