using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NetShield.RateLimiter.Options;
using System;
using System.Threading.Tasks;

namespace NetShield.RateLimiter.Middleware
{
    /// <summary>
    /// IP bazlı istek hız sınırlaması (Rate Limiting) yapan middleware sınıfı.
    /// </summary>
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;       // Pipeline'daki sonraki middleware'i temsil eder
        private readonly IMemoryCache _cache;         // Bellek içi cache servisi
        private readonly RateLimiterOptions _options; // Kullanıcıdan gelen konfigürasyon parametreleri

        /// <summary>
        /// Middleware yapıcısı. İlgili servisler ve ayarları alır.
        /// </summary>
        /// <param name="next">Bir sonraki middleware'i temsil eden RequestDelegate</param>
        /// <param name="cache">IMemoryCache servisi</param>
        /// <param name="options">Rate limiting ayarlarını tutan konfigürasyon nesnesi</param>
        public RateLimiterMiddleware(RequestDelegate next, IMemoryCache cache, RateLimiterOptions options)
        {
            _next = next;
            _cache = cache;
            _options = options;
        }

        /// <summary>
        /// HTTP isteği geldiğinde çağrılan asenkron metot.
        /// İstek yapan IP'nin limitini kontrol eder, aşılırsa 429 döner.
        /// </summary>
        /// <param name="context">HTTP bağlamı (request & response bilgileri)</param>
        /// <returns>İşlemin asenkron tamamlanma görevi</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // İstek yapan kullanıcının IP adresini al, yoksa "unknown" olarak ata
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Cache'den IP için kayıtlı istek sayısını çek (yoksa 0 döner)
            var count = _cache.Get<int?>(ip) ?? 0;

            // İstek sayısı, konfigürasyonda belirlenen maksimum limiti geçti mi?
            if (count >= _options.MaxRequests)
            {
                // HTTP 429 - Too Many Requests durum kodunu ayarla
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // İstek reddedildiğini kullanıcıya mesajla bildir
                await context.Response.WriteAsync("Too many requests.");

                // İstek işlem hattı burada durur, sonraki middleware çağrılmaz
                return;
            }

            // Limit aşılmamışsa, cache'deki sayacı 1 arttır ve cache süresi olarak konfigürasyondaki periyodu ver
            _cache.Set(ip, count + 1, _options.Period);

            // İstek zincirindeki sonraki middleware veya endpoint çağrılır
            await _next(context);
        }
    }
}
