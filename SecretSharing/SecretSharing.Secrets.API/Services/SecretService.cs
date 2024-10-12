using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SecretSharing.Secrets.API.Models;

namespace SecretSharing.Secrets.API.Services;

public class SecretService(IMongoClient mongoClient) : ISecretService
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("secrets");

    public async Task<Secret?> GetSecret(string userId, int secretId)
    {
        var userCollection = _database.GetCollection<Secret>(userId);
        var search = await userCollection.FindAsync(x => x.SecretId == secretId);
        return search.FirstOrDefault();
    }

    public async Task<ICollection<Secret>> GetSecrets(string userId)
    {
        var userCollection = _database.GetCollection<Secret>(userId).AsQueryable();
        return await userCollection.OrderByDescending(x => x.CreatedDate).Take(10).ToListAsync();
    }

    public async Task<int> SetSecret(string userId, Secret secret)
    {
        var collections = 
            await _database.ListCollectionsAsync(new ListCollectionsOptions
            {
                Filter = new BsonDocument("name", userId)
            });

        if (!await collections.AnyAsync())
        {
            await _database.CreateCollectionAsync(userId);
            await _database.GetCollection<Secret>(userId)
                .Indexes.CreateOneAsync(
                    new CreateIndexModel<Secret>(
                        Builders<Secret>.IndexKeys.Ascending(x => x.CreatedDate),
                        new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(1) })
                    );
        }
        
        var userCollection = _database.GetCollection<Secret>(userId);
        var secretToSave = secret with
        {
            SecretId = await userCollection.AsQueryable()
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.SecretId).FirstOrDefaultAsync() + 1
        };
        await userCollection.InsertOneAsync(secretToSave);
        return secret.SecretId;
    }

    public Task DeleteSecret(string userId, int secretId)
    {
        var userCollection = _database.GetCollection<Secret>(userId);
        return userCollection.DeleteOneAsync(x => x.SecretId == secretId);
    }
}