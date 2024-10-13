using Microsoft.EntityFrameworkCore;

namespace SecretSharing.User.API;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<models.User> Users { get; set; }
}