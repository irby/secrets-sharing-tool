using CustomEnvironmentConfig;
using SecretsSharingTool.Core;
using SecretsSharingTool.Core.Configuration;
using SecretsSharingtool.Data;
using SecretsSharingTool.Web.HostedServices;

var builder = WebApplication.CreateBuilder(args);

var configuration = ConfigurationParser.ParsePosix<DatabaseConfiguration>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddCoreServices()
    .AddDatabase(configuration.BuildConnectionString())
    .AddDatabaseAccessor();

builder.Services.AddHostedService<CleanseSecretsHostedService>();

builder.Services.ApplyPendingMigrations();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
