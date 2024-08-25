using System.Text.Json.Serialization;

namespace SecretSharing.API.models;

public class User
{
    [JsonIgnore]
    public int Id { get; set; }
    public string UserId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }
}

[JsonSerializable(typeof(User[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}