using System.Buffers.Text;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecretSharing.User.API.Endpoints;

public static class User
{
    public static void RegisterUserApi(this WebApplication app)
    {
        var userApi = app.MapGroup("/user")
            .RequireAuthorization(auth =>
            {
                auth.RequireAssertion(
                    context => context.User.FindFirst(
                        c => c is { Type: "scope" } && c.Value.Contains("read:user")) != null);

                auth.RequireAuthenticatedUser();
            });

        userApi.MapGet("/keys", async
            ([FromServices] UserDbContext db, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Results.Unauthorized();
            

            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            return user == null ? Results.NotFound() : Results.Ok(user);
        });

        userApi.MapPost("/keys",
            async ([FromServices] UserDbContext db, HttpContext http, [FromBody] UserKeysRequest request) =>
            {
                var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user != null) 
                    return Results.Conflict();
                
                user = new models.User
                {
                    UserId = userId,
                    PublicKey = Convert.FromBase64String(request.PublicKey),
                    PrivateKey = Convert.FromBase64String(request.PrivateKey),
                    Salt = Convert.FromBase64String(request.salt),
                    IV = Convert.FromBase64String(request.iv)
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Created();

            });
    }
}

public record UserKeysRequest(string PublicKey, string PrivateKey, string salt, string iv);