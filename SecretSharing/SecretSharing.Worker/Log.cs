#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SecretSharing.Worker;

[Index(nameof(EventId))]
[Index(nameof(UserId))]
public class Log
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string EventId { get; set; }
    [MaxLength(100)]
    public string UserId { get; set; }
    [MaxLength(256)]
    public string Path { get; set; }
}