// src/EventPlanner.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs

using EventPlanner.Application.Abstractions.Cache;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Infrastructure.Persistence.CacheServices;
using EventPlanner.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EventPlanner.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Репозитории
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Redis
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(redisConnection));

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}