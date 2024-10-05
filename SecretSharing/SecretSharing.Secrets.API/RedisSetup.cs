using StackExchange.Redis;

namespace SecretSharing.Secrets.API;

public class RedisSetup
{
    private readonly IConnectionMultiplexer _cacheConnection;

    public RedisSetup(IConnectionMultiplexer cacheConnection)
    {
        _cacheConnection = cacheConnection;
    }

    public async Task SetupReplication()
    {
        var retries = 0;
        var primary = _cacheConnection.GetServers()[0];
        var secondary = _cacheConnection.GetServers()[1];
        while (!primary.IsConnected && !secondary.IsConnected && retries < 10)
        {
            retries++;
            await Task.Delay(5000);
        }
        
        switch (primary.IsConnected)
        {
            case true when secondary is { IsConnected: true, IsReplica: false }:
                await secondary.ReplicaOfAsync(primary.EndPoint);
                break;
            case false when !secondary.IsConnected:
                throw new Exception("Could not connect to primary and secondary Redis servers.");
        }
    }
}