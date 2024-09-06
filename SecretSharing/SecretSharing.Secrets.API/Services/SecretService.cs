using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace SecretSharing.Secrets.API.Services;

public class SecretService
{
    private readonly IDistributedCache _cache;

    public SecretService(IConnectionMultiplexer cache)
    {
        _cache = cache;
    }
    
    public async Task<string?> GetSecret(string userId, int secretId)
    {
        var key = $"{userId}:{secretId}";
        return await _cache.GetStringAsync(key);
    }
    
    public async Task SetSecret(string userId, int secretId, string secret)
    {
        var key = $"{userId}:{secretId}";
        await _cache.SetStringAsync(key, secret);
    }
}