using Microsoft.Extensions.DependencyInjection;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Services;

namespace EventPlanner.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBookingService, BookingService>();
        return services;
    }
    
}