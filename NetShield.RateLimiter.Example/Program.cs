using Microsoft.Extensions.Caching.Memory;
using NetShield.RateLimiter.Middleware;
using NetShield.RateLimiter.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. MemoryCache servisini ekle
builder.Services.AddMemoryCache();

var app = builder.Build();

// 2. RateLimiterOptions oluþtur
var rateLimiterOptions = new RateLimiterOptions
{
    MaxRequests = 10, // Maksimum 5 istek
    Period = TimeSpan.FromSeconds(30) // 30 saniyede
};

// 3. Middleware’i pipeline’a ekle
app.UseMiddleware<RateLimiterMiddleware>(
    app.Services.GetRequiredService<IMemoryCache>(),
    rateLimiterOptions
);

app.MapGet("/", () => "Merhaba, NetShield.RateLimiter örneði çalýþýyor!");


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
