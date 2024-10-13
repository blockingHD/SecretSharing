using Microsoft.EntityFrameworkCore;
using Npgsql;
using SecretSharing.ServiceDefaults;
using SecretSharing.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<LoggerDbContext>("logdb");

var host = builder.Build();

IEventLogger? connection = null;
while (connection is null)
{
    try
    {
        connection = host.Services.GetRequiredService<IEventLogger>();
    }
    catch
    {
        await Task.Delay(1000);
    }
}

using var scope = host.Services.CreateScope();

bool canConnect;
var tries = 10;
var dbConnection = new NpgsqlConnection(host.Services
    .GetRequiredService<IConfiguration>().GetConnectionString("logger-postgres"));
do
{
    try
    {
        await dbConnection.OpenAsync();
        await new NpgsqlCommand("SELECT 1", dbConnection).ExecuteNonQueryAsync();
        canConnect = true;
    }
    catch
    {
        canConnect = false;
        await Task.Delay(5000);
    }
} while (!canConnect && tries-- > 0);

var context = scope.ServiceProvider.GetRequiredService<LoggerDbContext>();
await context.Database.MigrateAsync();

host.Run();