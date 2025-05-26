# NetShield.RateLimiter

## 🚀 Açıklama

**NetShield.RateLimiter**, ASP.NET Core uygulamaları için geliştirilmiş IP tabanlı bir **rate limiting (istek sınırlama)** middleware paketidir. Bu paket sayesinde sunucunuzu aşırı isteklerden koruyabilir, belirli bir süre zarfında aynı IP'den gelen istekleri sınırlandırabilirsiniz.

---

## 🎯 Özellikler

* 🔐 IP adresine göre istek sayısını takip eder.
* 🚫 Belirlenen limit aşıldığında **429 Too Many Requests** hatası döner.
* 🧠 MemoryCache altyapısını kullanır (dağıtık olmayan senaryolar için idealdir).
* 🛠️ Basit ve esnek yapı – kolayca entegre edilebilir.
* ⚙️ Opsiyonel konfigürasyon desteği: Limit ve süreyi kendin belirleyebilirsin.

---

## 🎞️ Kurulum

Paketin NuGet üzerinden kurulumu:

```bash
dotnet add package NetShield.RateLimiter
```

---

## ⚙️ Kullanımı

Aşağıdaki örnek, middleware’in nasıl entegre edildiğini göstermektedir:

```csharp
using Microsoft.Extensions.Caching.Memory;
using NetShield.RateLimiter;
using NetShield.RateLimiter.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. MemoryCache servisini ekle
builder.Services.AddMemoryCache();

var app = builder.Build();

// 2. RateLimiterOptions oluştur
var rateLimiterOptions = new RateLimiterOptions
{
    MaxRequests = 5,                // Maksimum 5 istek
    Period = TimeSpan.FromSeconds(30) // 30 saniyelik zaman diliminde
};

// 3. Middleware’i pipeline’a ekle
app.UseMiddleware<RateLimiterMiddleware>(
    app.Services.GetRequiredService<IMemoryCache>(),
    rateLimiterOptions
);

// 4. Test amaçlı bir endpoint
app.MapGet("/", () => "Merhaba, Rate Limiter çalışıyor!");

app.Run();
```

---

## 🧪 Testler

Paketle birlikte gelen test sınıfı şunları kontrol eder:

* Limit aşılmadığında istek geçer ve sonraki middleware çalışır.
* Limit aşıldığında istek engellenir ve 429 hatası döner.

---

## 📄 Lisans

MIT License – Özgürce kullanabilir ve geliştirebilirsiniz.

---

## 👨‍💻 Geliştirici

Bu proje **Ömer Dumlupınar** tarafından geliştirilmiştir.
İletişim: \[[omerdumlupinar0@gmail.com](mailto:omerdumlupinar0@gmail.com)]

---