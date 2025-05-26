using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetShield.RateLimiter.Models
{
    public class RateLimitRule
    {
        public string Endpoint { get; set; } = "*";
        public int Limit { get; set; } = 100;
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);
    }
}
