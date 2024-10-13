#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecretSharing.User.API.models;

public class User
{
    [JsonIgnore]
    public int Id { get; set; }
    [MaxLength(100)]
    public string UserId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }
    public byte[] Salt { get; set; }
    public byte[] IV { get; set; }
}

[JsonSerializable(typeof(User[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}