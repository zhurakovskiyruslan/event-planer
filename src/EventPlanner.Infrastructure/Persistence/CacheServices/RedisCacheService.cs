using EventPlanner.Application.Abstractions.Cache;
using StackExchange.Redis;

namespace EventPlanner.Infrastructure.Persistence.CacheServices;

public class RedisCacheService :  ICacheService
{
    private readonly IDatabase _db;
    public RedisCacheService(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _db.StringSetAsync(key, value, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}