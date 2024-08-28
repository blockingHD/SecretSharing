using Microsoft.EntityFrameworkCore;
using SecretSharing.User.API.models;

namespace SecretSharing.User.API;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<models.User> Users { get; set; }
}