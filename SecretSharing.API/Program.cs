using Microsoft.Identity.Web;
using SecretSharing.API;
using SecretSharing.API.models;
using User = SecretSharing.API.Endpoints.User;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.AddNpgsqlDbContext<UserDbContext>("userdb");

var app = builder.Build(); 
app.UseAuthentication();
app.UseAuthorization();

User.RegisterUserApi(app);

app.Run();