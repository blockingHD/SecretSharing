using SecretSharing.Secrets.API.Models;
using StackExchange.Redis;

namespace SecretSharing.Secrets.API.Services;

public class SecretService : ISecretService
{
    private readonly IConnectionMultiplexer _cacheConnection;
    private readonly IDatabase _cache;

    public SecretService(IConnectionMultiplexer cacheConnection)
    {
        _cacheConnection = cacheConnection;
        _cache = cacheConnection.GetDatabase();
    }

    public async Task<Secret?> GetSecret(string userId, int secretId)
    {
        var key = $"{userId}:{secretId}";

        var value = await _cache.HashGetAsync(key, ["SenderEmail", "CreatedDate", "SecretValue"]);
        return value.Any(x => x.IsNull) ? null : new Secret(secretId, value[0], DateTime.Parse(value[1]), value[2]);
    }

    public async Task<ICollection<Secret>> GetSecrets(string userId)
    {
        var server = _cacheConnection.GetServer(_cacheConnection.GetServers().First().EndPoint);
        var secrets = new List<Secret>();
        await foreach (var key in server.KeysAsync(pattern: $"{userId}:[^nextId]*"))
        {
            var value = await _cache.HashGetAsync(key, ["SenderEmail", "CreatedDate", "SecretValue"]);
            if (!value.Any(x => x.IsNull))
            {
                secrets.Add(new Secret(int.Parse(key.ToString().Split(':')[1]), value[0], DateTime.Parse(value[1]), null));
            }
        }
        
        return secrets;
    }

    public async Task<int> SetSecret(string userId, Secret secret)
    {
        var keyId = $"{userId}:nextId";
        
        var nextId = await _cache.StringIncrementAsync(keyId);
        var key = $"{userId}:{nextId}";
        await _cache.HashSetAsync(key, secret.ToHashEntryArray());
        
        return (int)nextId;
    }
}