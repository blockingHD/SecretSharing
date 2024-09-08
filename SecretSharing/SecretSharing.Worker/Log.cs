using Microsoft.EntityFrameworkCore;

namespace SecretSharing.Worker;

[Index(nameof(EventId))]
[Index(nameof(UserId))]
public class Log
{
    public int Id { get; set; }
    public string EventId { get; set; }
    public string UserId { get; set; }
    public string Path { get; set; }
}