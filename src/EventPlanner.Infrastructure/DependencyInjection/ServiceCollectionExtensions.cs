// src/EventPlanner.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventPlanner.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует инфраструктурные сервисы (репозитории и т.п.).
    /// Вызывается из Composition Root (обычно из Web API: builder.Services.AddInfrastructure()).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Репозитории
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}