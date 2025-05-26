using NetShield.RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetShield.RateLimiter.Options
{
    /// <summary>
    /// RateLimiter middleware için yapılandırma seçenekleri
    /// </summary>
    public class RateLimiterOptions
    {
        /// <summary>
        /// Bir IP adresi için izin verilen maksimum istek sayısı
        /// </summary>
        public int MaxRequests { get; set; } = 10;

        /// <summary>
        /// İstek sayısının sıfırlanacağı zaman aralığı
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);
    }
}
