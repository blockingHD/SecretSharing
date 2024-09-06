using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var userPostgres = builder.AddPostgres("userPostgres")
    .WithDataVolume();
var userDatabase = userPostgres.AddDatabase("userdb");

var userApi = builder.AddProject<SecretSharing_User_API>("userapi")
    .WithReference(userDatabase)
    .WithReference(userPostgres)
    .WithExternalHttpEndpoints();

var secretApi = builder.AddProject<SecretSharing_Secrets_API>("secrets")
    .WithReference(cache);
    

builder.AddNpmApp("angular", "../SecretSharing.Angular")
    .WithReference(userApi)
    .WithReference(secretApi)
    .WithHttpEndpoint(env: "PORT", port: 4587)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();