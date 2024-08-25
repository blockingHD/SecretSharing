using System.Text.Json.Serialization;

namespace SecretSharing.API.models;

public class User
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }
}

[JsonSerializable(typeof(User[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}