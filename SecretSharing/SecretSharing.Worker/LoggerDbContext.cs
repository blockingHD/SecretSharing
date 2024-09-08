using Microsoft.EntityFrameworkCore;

namespace SecretSharing.Worker;

public class LoggerDbContext(DbContextOptions<LoggerDbContext> options) : DbContext(options)
{
    public DbSet<Log> Logs { get; set; }
}