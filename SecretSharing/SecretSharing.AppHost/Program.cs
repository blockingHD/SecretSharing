using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithPersistence(TimeSpan.FromSeconds(1));

var messaging = builder.AddRabbitMQ("messaging");

var userPostgres = builder.AddPostgres("user-postgres");

var userDatabase = userPostgres.AddDatabase("userdb");

var userApi = builder.AddProject<SecretSharing_User_API>("userapi")
    .WithReference(userDatabase)
    .WithReference(userPostgres)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();

var secretApi = builder.AddProject<SecretSharing_Secrets_API>("secretsapi")
    .WithReference(cache)
    .WithReference(messaging);

var loggerPostgres = builder.AddPostgres("logger-postgres");

var logDatabase = loggerPostgres.AddDatabase("logdb");

var loggingFunction =
    builder.AddProject<SecretSharing_Worker>("logging-function")
        .WithReference(messaging)
        .WithReference(loggerPostgres)
        .WithReference(logDatabase);

var angular = builder.AddNpmApp("angular", "../SecretSharing.Angular")
    .WithReference(userApi)
    .WithReference(secretApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.Environment.IsDevelopment())
{
    // angular
    //     .WithHttpEndpoint(env: "PORT", port: 4587);
    //
    // cache.WithDataVolume();
    // userPostgres.WithDataVolume();
    // loggerPostgres.WithDataVolume();
}

builder.Build().Run();