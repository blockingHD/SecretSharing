using SecretSharing.Secrets.API.Models;

namespace SecretSharing.Secrets.API.Services;

public interface ISecretService
{
    Task<Secret?> GetSecret(string userId, int secretId);
    Task<ICollection<Secret>> GetSecrets(string userId);
    Task<int> SetSecret(string userId, Secret secret);
}