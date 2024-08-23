using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var hashiVaultContainer = builder.AddContainer("secretsharing-vault", "hashicorp/vault", "latest")
    .WithHttpEndpoint(env: "PORT", targetPort: 8200, name: "vault")
    .WithExternalHttpEndpoints();

var hashiVaultEndpoint = hashiVaultContainer.GetEndpoint("vault");

var cache = builder.AddRedis("cache");

var secretSharingApi = builder.AddProject<SecretSharing_API>("secretsharingapi")
    .WithReference(cache)
    .WithReference(hashiVaultEndpoint)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("angular", "../SecretSharing.Angular")
    .WithReference(secretSharingApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();