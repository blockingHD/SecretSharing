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

        
    }
}