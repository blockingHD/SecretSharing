using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("messaging");

var userPostgres = builder.AddPostgres("user-postgres");

var userDatabase = userPostgres.AddDatabase("userdb");

var mongoDb = builder.AddMongoDB("mongo", port: 61640)
    .WithArgs("--replSet", "rs0", "--bind_ip_all");

var mongoDb2 = builder.AddMongoDB("mongo2", port: 61673)
    .WithArgs("--replSet", "rs0", "--bind_ip_all");

var mongoDb3 = builder.AddMongoDB("mongo3", port: 61689)
    .WithArgs("--replSet", "rs0", "--bind_ip_all");

var userApi = builder.AddProject<SecretSharing_User_API>("userapi")
    .WithReference(userDatabase)
    .WithReference(userPostgres)
    .WithReference(messaging)
    .WithExternalHttpEndpoints();

var secretApi = builder.AddProject<SecretSharing_Secrets_API>("secretsapi")
    .WithReference(mongoDb)
    .WithReference(mongoDb2)
    .WithReference(mongoDb3)
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
    // .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.Environment.IsDevelopment())
{
    angular
        .WithHttpEndpoint(env: "PORT", port: 4587);
    
    // mongoDb.WithDataVolume();
    // userPostgres.WithDataVolume();
    // loggerPostgres.WithDataVolume();
}

builder.Build().Run();