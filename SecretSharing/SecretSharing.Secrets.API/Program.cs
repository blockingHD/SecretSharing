using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using MongoDB.Bson;
using MongoDB.Driver;
using SecretSharing.Secrets.API.Endpoints;
using SecretSharing.Secrets.API.Services;
using SecretSharing.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISecretService, SecretService>();
builder.AddRabbitMQClient("messaging");
builder.AddMongoDBClient("mongo", configureClientSettings: settings =>
{
    settings.ReplicaSetName = "rs0";
    settings.DirectConnection = true;
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterSecretsApi();

var settings = new MongoClientSettings
{
    Server = MongoServerAddress.Parse(app.Configuration.GetConnectionString("mongo")!.Replace("mongodb://", "")),
    DirectConnection = true,
    ReplicaSetName = "rs0"
};
var mongoClient = new MongoClient(settings);
var database = mongoClient.GetDatabase("admin");
var members = app.Configuration.GetSection("ConnectionStrings")
    .GetChildren()
    .Where(x => x.Key.Contains("mongo"))
    .Select(x => x.Value!.Replace("mongodb://", ""))
    .Select(x => x.Contains("localhost") ? new UriEndPoint(new Uri(x.Replace("localhost", "host.docker.internal"))) : new UriEndPoint(new Uri(x)))
    .ToList();
var config = JsonSerializer.Serialize(new {
    _id = "rs0",
    members = members.Select((x, i) => new { _id = i, host = x.ToString() })
});

database.RunCommand<BsonDocument>($"{{ replSetInitiate: {config}}}");

app.Run();