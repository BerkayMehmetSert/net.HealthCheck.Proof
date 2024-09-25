using Api.Application.Configuration;
using Api.Application.Configuration.HealthChecks;
using Api.Application.Service;
using Api.Application.Settings;
using Api.Infrastructure.DbContext;
using Api.Infrastructure.Repository;
using Api.Integration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

// Configurations
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
var healthCheckConnString = builder.Configuration.GetConnectionString("HealthCheck");
var connectionString = builder.Configuration.GetConnectionString("Default");
var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
builder.Services.AddSingleton(resolver => 
    resolver.GetRequiredService<IOptions<RedisSettings>>().Value);

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// MemoryCache
builder.Services.AddMemoryCache();

// Repository
builder.Services.AddScoped<IPostRepository, PostRepository>();

// Service
builder.Services.AddScoped<IPostService, PostService>()
    .AddSingleton<ICacheService, CacheService>()
    .AddSingleton<ICacheDatabaseProvider, CacheDatabaseProvider>();
// Refit
builder.Services.AddRefitClient<IJsonPlaceholderApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"));

// HealthChecks
builder.Services.ConfigureHealthChecks(connectionString, healthCheckConnString, redisSettings);
builder.Services.AddSingleton<RedisHealthCheck>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status200OK,
        },
    });

    endpoints.MapHealthChecksUI(options =>
    {
        options.UIPath = "/status";
        options.AsideMenuOpened = false;
    });

    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();