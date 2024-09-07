using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SecretSharing.Secrets.API.Models;
using SecretSharing.Secrets.API.Services;
using SecretSharing.ServiceDefaults.Attribute;

namespace SecretSharing.Secrets.API.Endpoints;

public static class Secrets
{
    public static void RegisterSecretsApi(this WebApplication app)
    {
        var secretsApi = app.MapGroup("/secrets")
            .RequireAuthorization(auth =>
            {
                auth.RequireAuthenticatedUser();
            });

        secretsApi.MapGet("/", async
            ([FromServices] ISecretService secretService, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Results.Unauthorized();

            var secrets = await secretService.GetSecrets(userId);

            return secrets.Count == 0
                ? Results.NoContent()
                : Results.Ok(secrets);
        });
        
        secretsApi.MapGet("/{secretId}", async
            ([FromServices] ISecretService secretService, HttpContext http, int secretId) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Results.Unauthorized();

            var secret = await secretService.GetSecret(userId, secretId);

            await secretService.DeleteSecret(userId, secretId);

            return secret == null
                ? Results.NotFound()
                : Results.Ok(secret);
        })
        .AddEndpointFilter<EventLoggerFilter>()
        .WithMetadata(new Event("secret.read"));
        
        secretsApi.MapPost("/{userId}", async
            ([FromServices] ISecretService secretService, HttpContext http, string userId, [FromBody] string secret) =>
        {
            var email = http.User.FindFirst(ClaimTypes.Email)!.Value;

            var id = await secretService.SetSecret(userId, new Secret(0, email, DateTime.UtcNow, secret));

            return Results.Created($"/secrets/{id}", secret);
        })
        .AddEndpointFilter<EventLoggerFilter>()
        .WithMetadata(new Event("secret.create"));

    }
}