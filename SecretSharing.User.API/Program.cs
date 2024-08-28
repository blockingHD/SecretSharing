using Microsoft.AspNetCore.Authentication.JwtBearer;
using SecretSharing.User.API;
using SecretSharing.User.API.models;
using User = SecretSharing.User.API.Endpoints.User;

var builder = WebApplication.CreateSlimBuilder(args);

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
});

builder.AddNpgsqlDbContext<UserDbContext>("userdb");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

User.RegisterUserApi(app);

app.Run();