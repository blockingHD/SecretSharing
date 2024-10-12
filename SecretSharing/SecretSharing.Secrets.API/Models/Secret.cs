using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using StackExchange.Redis;

namespace SecretSharing.Secrets.API.Models;

public record Secret
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int SecretId { get; init; }
    public string SenderEmail { get; init; }
    [BsonElement("createdAt")]
    public DateTime CreatedDate { get; init; }
    public string? SecretValue { get; init; }
    
    public Secret(int id, string senderEmail, DateTime createdDate, string secretValue)
    {
        SecretId = id;
        SenderEmail = senderEmail;
        CreatedDate = createdDate;
        SecretValue = secretValue;
    }
    
    public HashEntry[] ToHashEntryArray() =>
    [
        new HashEntry("SenderEmail", SenderEmail),
        new HashEntry("CreatedDate", CreatedDate.ToString("O")),
        new HashEntry("SecretValue", SecretValue)
    ];
}