# Financial Stock Watchlist API

Bu proje, Rasyonet staj backend değerlendirmesi için hazırlanmış küçük ve anlaşılır bir ASP.NET Core Web API projesidir.

Amaç; kullanıcıların takip etmek istedikleri hisse senetlerini bir izleme listesine ekleyebilmesi, Finnhub üzerinden güncel fiyat verilerini çekebilmesi, fiyat geçmişini SQLite veritabanında saklayabilmesi ve en çok yükselen hisseleri listeleyebilmesidir.

## Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- Finnhub API

## Neden Finnhub Kullanıldı?

Finnhub, finansal piyasa verilerine erişmek için sade ve kullanımı kolay bir API sağlar. Ücretsiz kullanım seviyesi bu proje için yeterlidir. Özellikle quote endpoint'i sayesinde bir hisse senedinin güncel fiyatı, önceki kapanış fiyatı ve günlük değişim yüzdesi tek istekle alınabilir.

## Neden SQLite Kullanıldı?

SQLite, bu proje için basit ve pratik bir veritabanı çözümüdür. Ayrı bir veritabanı sunucusu kurmaya gerek yoktur. Bu yüzden küçük ölçekli, lokal çalıştırılacak ve mülakatta kolayca anlatılacak projeler için uygundur.

## Mimari

Projede basit bir Controller-Service-Repository yapısı kullanılmıştır.

- `Controllers`: HTTP isteklerini karşılar ve uygun status code döndürür.
- `Services`: İş kurallarını içerir.
- `Repositories`: Veritabanı işlemlerini yapar.
- `Models`: Entity Framework Core entity sınıflarını içerir.
- `DTOs`: API request ve response modellerini içerir.
- `Data`: `AppDbContext` sınıfını içerir.
- `Configuration`: Konfigürasyon modellerini içerir.

Bu yapı sayesinde controller içinde iş mantığı tutulmaz. Controller sadece isteği alır, service katmanını çağırır ve sonucu HTTP cevabına dönüştürür.

## Tasarım Desenleri

Projede Repository Pattern kullanılmıştır. `StockRepository`, veritabanı erişim kodunu business logic kodundan ayırır.

Ayrıca `IFinancialDataService` interface'i Strategy benzeri basit bir soyutlama sağlar. `StockService`, doğrudan Finnhub sınıfına değil bu interface'e bağlıdır. Böylece ileride Finnhub yerine başka bir finansal veri sağlayıcı kullanılmak istenirse business logic büyük ölçüde değişmeden kalabilir.

## Kurulum

Paketleri geri yüklemek için:

```bash
dotnet restore
```

## Finnhub API Key Ekleme

Önce https://finnhub.io üzerinden ücretsiz bir API key alın.

Lokal geliştirme için API key'i environment variable olarak verebilirsiniz:

```powershell
$env:Finnhub__ApiKey="your-api-key"
```

Alternatif olarak `RasyonetInternshipApi/appsettings.Development.json` dosyasına ekleyebilirsiniz:

```json
{
  "Finnhub": {
    "ApiKey": "your-api-key"
  }
}
```

Gerçek API key'leri versiyon kontrol sistemine eklenmemelidir.

## Migration ve Veritabanı

Eğer `dotnet-ef` aracı yüklü değilse:

```bash
dotnet tool install --global dotnet-ef
```

SQLite veritabanını oluşturmak veya güncellemek için:

```bash
dotnet ef database update --project RasyonetInternshipApi
```

Bu projede uygulama başlarken migration'lar otomatik olarak uygulanır. Bu tercih, lokal demo sürecini kolaylaştırmak için yapılmıştır.

## Projeyi Çalıştırma

```bash
dotnet run --project RasyonetInternshipApi
```

Swagger arayüzü:

```text
https://localhost:7298/swagger
```

veya

```text
http://localhost:5119/swagger
```

## Endpoint Listesi

### Tüm hisseleri getir

```http
GET /api/stocks
```

### Sembole göre hisse getir

```http
GET /api/stocks/{symbol}
```

### İzleme listesine hisse ekle

```http
POST /api/stocks
```

Request body:

```json
{
  "symbol": "AAPL",
  "companyName": "Apple Inc."
}
```

### Hisse sil

```http
DELETE /api/stocks/{symbol}
```

### Hisse fiyatını Finnhub üzerinden güncelle

```http
POST /api/stocks/{symbol}/refresh
```

Bu endpoint Finnhub'dan güncel fiyat bilgisini çeker, `Stock` kaydını günceller ve ayrıca `StockPriceHistory` tablosuna yeni bir kayıt ekler.

### En çok yükselen hisseleri getir

```http
GET /api/analytics/top-gainers?count=5
```

Bu endpoint hisseleri `ChangePercent` değerine göre azalan şekilde sıralar.

## Örnek İstekler

Hisse ekleme:

```http
POST /api/stocks
Content-Type: application/json

{
  "symbol": "AAPL",
  "companyName": "Apple Inc."
}
```

Fiyat güncelleme:

```http
POST /api/stocks/AAPL/refresh
```

En çok yükselenleri getirme:

```http
GET /api/analytics/top-gainers?count=5
```

## Hata Yönetimi

- `400 Bad Request`: Geçersiz symbol veya hatalı request.
- `404 Not Found`: İstenen hisse bulunamadı.
- `409 Conflict`: Hisse zaten izleme listesinde mevcut.
- `502 Bad Gateway`: Finnhub üzerinden geçerli veri alınamadı.

API client'larına raw exception mesajları dönülmez.

## Bilinen Trade-off'lar

- Authentication eklenmedi, çünkü bu değerlendirme için kapsam dışında.
- Frontend eklenmedi, çünkü proje backend API odaklıdır.
- Generic repository, MediatR, CQRS, Clean Architecture veya DDD gibi ileri seviye yapılar bilinçli olarak kullanılmadı.
- Proje küçük, okunabilir ve mülakatta kolay anlatılabilir tutuldu.
- Şirket adı kullanıcıdan alınır. Finnhub entegrasyonu sadece fiyat verisine odaklanır.
