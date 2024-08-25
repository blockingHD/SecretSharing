using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecretSharing.API.Endpoints;

public static class User
{
    public static void RegisterUserApi(this WebApplication app)
    {
        var userApi = app.MapGroup("/user");

        userApi.MapGet("/", [Authorize] async
            ([FromServices] UserDbContext db, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Results.Unauthorized();
            }
            
            var userGuid = Guid.Parse(userId);
            
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
            
            return user == null ? Results.NotFound() : Results.Ok(user);
        });
    }
}