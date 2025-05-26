using Microsoft.Extensions.DependencyInjection;
using NetShield.RateLimiter.Middleware;
using NetShield.RateLimiter.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetShield.RateLimiter.Extensions
{
    public static class RateLimiterExtensions
    {
        // Bu metot, middleware'i HTTP pipeline'a eklemeyi sağlar
        public static IApplicationBuilder UseNetShieldRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }

        // Bu metot, gerekli servisleri (memory cache gibi) servis koleksiyonuna ekler
        public static IServiceCollection AddNetShieldRateLimiter(this IServiceCollection services)
        {
            // Bellek tabanlı cache servisini ekliyoruz, middleware bunu kullanacak
            services.AddMemoryCache();

            return services;
        }
    }
}
