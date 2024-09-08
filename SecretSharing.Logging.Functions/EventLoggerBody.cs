using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SecretSharing.ServiceDefaults.Attribute;

public record EventLoggerBody
{
    public string UserId { get; init; }
    public string Path { get; init; }

    public EventLoggerBody(string userId, string path)
    {
        UserId = userId;
        Path = path;
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public static implicit operator string(EventLoggerBody body)
    {
        return body.ToString();
    }
    
    public static EventLoggerBody CreateFromHttpContext(HttpContext http)
    {
        var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var path = http.Request.Path;
        
        return new EventLoggerBody(userId, path);
    }
}