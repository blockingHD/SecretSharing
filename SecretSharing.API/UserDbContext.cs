using Microsoft.EntityFrameworkCore;
using SecretSharing.API.models;

namespace SecretSharing.API;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}