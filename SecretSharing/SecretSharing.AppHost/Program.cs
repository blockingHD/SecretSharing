using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var userPostgres = builder.AddPostgres("userPostgres");
var userDatabase = userPostgres.AddDatabase("userdb");

var secretsDatabase = builder.AddPostgres("secrets")
    .AddDatabase("secretsdb");

var secretSharingApi = builder.AddProject<SecretSharing_User_API>("userapi")
    .WithReference(cache)
    .WithReference(userDatabase)
    .WithReference(userPostgres)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("angular", "../SecretSharing.Angular")
    .WithReference(secretSharingApi)
    .WithHttpEndpoint(env: "PORT", port: 4587)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();