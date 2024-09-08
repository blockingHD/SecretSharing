using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithDataVolume()
    .WithPersistence(TimeSpan.FromSeconds(1));

var messaging = builder.AddRabbitMQ("messaging");

var userPostgres = builder.AddPostgres("userPostgres")
    .WithDataVolume();

var userDatabase = userPostgres.AddDatabase("userdb");

var userApi = builder.AddProject<SecretSharing_User_API>("userapi")
    .WithReference(userDatabase)
    .WithReference(userPostgres)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();

var secretApi = builder.AddProject<SecretSharing_Secrets_API>("secretsapi")
    .WithReference(cache)
    .WithReference(messaging);

var loggerPostgres = builder.AddPostgres("loggerPostgres")
    .WithDataVolume();

var logDatabase = loggerPostgres.AddDatabase("logdb");

var loggingFunction =
    builder.AddProject<SecretSharing_Worker>("loggingFunction")
        .WithReference(messaging)
        .WithReference(loggerPostgres)
        .WithReference(logDatabase);

builder.AddNpmApp("angular", "../SecretSharing.Angular")
    .WithReference(userApi)
    .WithReference(secretApi)
    .WithHttpEndpoint(env: "PORT", port: 4587)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();