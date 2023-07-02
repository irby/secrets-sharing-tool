using CustomEnvironmentConfig;
using SecretsSharingTool.Core;
using SecretsSharingTool.Core.Configuration;
using SecretsSharingTool.Data;
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

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy
                .WithOrigins("http://localhost:4200", "http://spa")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
