using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SecretSharing.User.API;
using SecretSharing.User.API.Endpoints;
using SecretSharing.User.API.models;
using User = SecretSharing.User.API.Endpoints.User;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dev-kbivp6b1lxnmfboj.uk.auth0.com/";
        options.Audience = "api://userapi";
    });

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.AddNpgsqlDbContext<UserDbContext>("userdb");

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterUserApi();

using var scope = app.Services.CreateScope();

bool canConnect;
var tries = 10;
var connection = new NpgsqlConnection(app.Configuration.GetConnectionString("userPostgres"));
do
{
    try
    {
        await connection.OpenAsync();
        await new NpgsqlCommand("SELECT 1", connection).ExecuteNonQueryAsync();
        canConnect = true;
    }
    catch
    {
        canConnect = false;
        await Task.Delay(5000);
    }
} while (!canConnect && tries-- > 0);

var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
await context.Database.MigrateAsync();

app.Run();