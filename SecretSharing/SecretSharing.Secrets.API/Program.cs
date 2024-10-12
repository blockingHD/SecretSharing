using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using MongoDB.Driver;
using SecretSharing.Secrets.API.Endpoints;
using SecretSharing.Secrets.API.Services;

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
    settings.ReadPreference = ReadPreference.Secondary;
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
database.RunCommand<BsonDocument>("{ replSetInitiate: {_id: \"rs0\",members: [{_id: 0, host: \"host.docker.internal:61640\", \"votes\" : 1},{_id: 1, host: \"host.docker.internal:61673\", \"votes\" : 1},{_id: 2, host: \"host.docker.internal:61689\", \"votes\" : 1}]}}");

app.Run();