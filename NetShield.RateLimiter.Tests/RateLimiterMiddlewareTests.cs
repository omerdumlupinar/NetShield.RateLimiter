using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NetShield.RateLimiter.Middleware;
using NetShield.RateLimiter;
using System.Threading.Tasks;
using Xunit;
using NetShield.RateLimiter.Options;

namespace NetShield.RateLimiter.Tests
{
    // RateLimiterMiddleware i�in birim testleri i�erir
    public class RateLimiterMiddlewareTests
    {
        // Bu test, istek say�s� limitin alt�nda oldu�unda middleware'in iste�i ge�irdi�ini do�rular
        [Fact]
        public async Task Allows_Request_When_Limit_Not_Reached()
        {
            // Bellek i�i cache olu�turuluyor
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Bu flag, middleware zincirindeki bir sonraki ad�m�n �a�r�l�p �a�r�lmad���n� kontrol eder
            bool nextCalled = false;

            // Middleware zincirindeki bir sonraki ad�m� temsil eden delegate
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true; // E�er �a�r�l�rsa true yap�l�r
                return Task.CompletedTask;
            };

            // RateLimiter i�in konfig�rasyon: 10 istek, 1 dakikal�k periyot
            var options = new RateLimiterOptions
            {
                MaxRequests = 10,
                Period = TimeSpan.FromMinutes(1)
            };

            // Middleware �rne�i olu�turuluyor
            var middleware = new RateLimiterMiddleware(next, memoryCache, options);

            // Test ama�l� bir HTTP context olu�turuluyor ve IP atan�yor
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            // Middleware �a�r�l�yor
            await middleware.InvokeAsync(context);

            // next delegate �al��t� m�? (Beklenen: �al��mal� ��nk� limit a��lmam��)
            Assert.True(nextCalled);
        }

        // Bu test, limit a��ld���nda middleware'in iste�i engelledi�ini ve 429 d�nd�rd���n� do�rular
        [Fact]
        public async Task Blocks_Request_When_Limit_Reached()
        {
            // Bellek i�i cache olu�turuluyor
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Sonraki middleware �al��t� m� diye kontrol edecek flag
            bool nextCalled = false;

            // Sonraki ad�m� temsil eden delegate
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true; // �a�r�l�rsa i�aretle
                return Task.CompletedTask;
            };

            // RateLimiter i�in konfig�rasyon: 10 istek, 1 dakikal�k s�re
            var options = new RateLimiterOptions
            {
                MaxRequests = 10,
                Period = TimeSpan.FromMinutes(1)
            };

            // Middleware �rne�i olu�turuluyor
            var middleware = new RateLimiterMiddleware(next, memoryCache, options);

            // Test context'i ve IP ayarlan�yor
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            // Sim�lasyon: IP i�in cache'e do�rudan limit de�eri yaz�l�yor (limit a��lm�� durumda)
            memoryCache.Set("127.0.0.1", 10, TimeSpan.FromMinutes(1));

            // Middleware �a�r�l�yor
            await middleware.InvokeAsync(context);

            // next delegate �a�r�ld� m�? (Beklenen: �a�r�lmamal� ��nk� limit a��lm��)
            Assert.False(nextCalled);

            // HTTP cevab�nda 429 Too Many Requests kodu d�nd� m�?
            Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        }
    }
}
