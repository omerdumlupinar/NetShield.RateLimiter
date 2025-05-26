using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NetShield.RateLimiter.Middleware;
using NetShield.RateLimiter;
using System.Threading.Tasks;
using Xunit;
using NetShield.RateLimiter.Options;

namespace NetShield.RateLimiter.Tests
{
    // RateLimiterMiddleware için birim testleri içerir
    public class RateLimiterMiddlewareTests
    {
        // Bu test, istek sayýsý limitin altýnda olduðunda middleware'in isteði geçirdiðini doðrular
        [Fact]
        public async Task Allows_Request_When_Limit_Not_Reached()
        {
            // Bellek içi cache oluþturuluyor
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Bu flag, middleware zincirindeki bir sonraki adýmýn çaðrýlýp çaðrýlmadýðýný kontrol eder
            bool nextCalled = false;

            // Middleware zincirindeki bir sonraki adýmý temsil eden delegate
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true; // Eðer çaðrýlýrsa true yapýlýr
                return Task.CompletedTask;
            };

            // RateLimiter için konfigürasyon: 10 istek, 1 dakikalýk periyot
            var options = new RateLimiterOptions
            {
                MaxRequests = 10,
                Period = TimeSpan.FromMinutes(1)
            };

            // Middleware örneði oluþturuluyor
            var middleware = new RateLimiterMiddleware(next, memoryCache, options);

            // Test amaçlý bir HTTP context oluþturuluyor ve IP atanýyor
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            // Middleware çaðrýlýyor
            await middleware.InvokeAsync(context);

            // next delegate çalýþtý mý? (Beklenen: çalýþmalý çünkü limit aþýlmamýþ)
            Assert.True(nextCalled);
        }

        // Bu test, limit aþýldýðýnda middleware'in isteði engellediðini ve 429 döndürdüðünü doðrular
        [Fact]
        public async Task Blocks_Request_When_Limit_Reached()
        {
            // Bellek içi cache oluþturuluyor
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Sonraki middleware çalýþtý mý diye kontrol edecek flag
            bool nextCalled = false;

            // Sonraki adýmý temsil eden delegate
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true; // Çaðrýlýrsa iþaretle
                return Task.CompletedTask;
            };

            // RateLimiter için konfigürasyon: 10 istek, 1 dakikalýk süre
            var options = new RateLimiterOptions
            {
                MaxRequests = 10,
                Period = TimeSpan.FromMinutes(1)
            };

            // Middleware örneði oluþturuluyor
            var middleware = new RateLimiterMiddleware(next, memoryCache, options);

            // Test context'i ve IP ayarlanýyor
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            // Simülasyon: IP için cache'e doðrudan limit deðeri yazýlýyor (limit aþýlmýþ durumda)
            memoryCache.Set("127.0.0.1", 10, TimeSpan.FromMinutes(1));

            // Middleware çaðrýlýyor
            await middleware.InvokeAsync(context);

            // next delegate çaðrýldý mý? (Beklenen: çaðrýlmamalý çünkü limit aþýlmýþ)
            Assert.False(nextCalled);

            // HTTP cevabýnda 429 Too Many Requests kodu döndü mü?
            Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        }
    }
}
