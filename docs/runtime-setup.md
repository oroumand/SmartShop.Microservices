# راهنمای اجرای کامل SmartShop

## هدف این سند

این سند یک runbook کامل برای اجرای پروژه کارگاهی SmartShop روی ماشینی است که پیشنیازهای لازم را دارد. هدف این راهنما این است که در زمان تست runtime، ترتیب آماده‌سازی سرویس‌ها، تنظیمات، اجرای API، تست دستی با Scalar، تست AiSearch و اجرای k6 روشن و قابل تکرار باشد.

این سند برای اجرای بعدی پروژه است و جایگزین validation فعلی با `dotnet build` و تست‌های معماری نمی‌شود.

## پیشنیازها

- .NET SDK سازگار با target framework پروژه
- SQL Server محلی یا SQL Server Express
- Docker فقط برای اجرای Qdrant در مرحله تست runtime بعدی
- OpenAI API key برای تست runtime ماژول AiSearch
- k6 فقط برای تست‌های پرفورمنس بعدی
- Git

## تنظیم Connection String

بهتر است Connection String و تنظیمات حساس از طریق environment variable تنظیم شوند، نه با commit کردن secret یا config واقعی داخل ریپازیتوری.

نمونه برای SQL Server محلی با Windows Authentication:

```text
ConnectionStrings__SmartShopDb=Server=localhost;Database=SmartShop;Trusted_Connection=True;TrustServerCertificate=True
```

نمونه برای SQL Express:

```text
ConnectionStrings__SmartShopDb=Server=localhost\SQLEXPRESS;Database=SmartShop;Trusted_Connection=True;TrustServerCertificate=True
```

نمونه برای SQL Authentication:

```text
ConnectionStrings__SmartShopDb=Server=localhost;Database=SmartShop;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

## تنظیم OpenAI برای AiSearch

برای تست endpointهایی که embedding تولید می‌کنند، باید OpenAI تنظیم شده باشد:

```text
AiSearch__OpenAI__ApiKey=YOUR_OPENAI_API_KEY
AiSearch__OpenAI__BaseUrl=https://api.openai.com
AiSearch__OpenAI__Model=text-embedding-3-small
AiSearch__OpenAI__Dimensions=1536
```

کلید واقعی OpenAI را commit نکنید. تا وقتی `AiSearch__OpenAI__ApiKey` تنظیم نشده باشد، endpointهای AiSearch که به embedding نیاز دارند در runtime با خطای قابل فهم شکست می‌خورند.

## اجرای Qdrant بدون Docker Compose

در این مرحله Docker Compose عمدا اضافه نشده است. دستورهای زیر فقط برای ماشینی هستند که بعدا قرار است تست runtime کامل روی آن انجام شود.

```bash
docker volume create smartshop-qdrant-storage
```

```bash
docker run -d --name smartshop-qdrant -p 6333:6333 -p 6334:6334 -v smartshop-qdrant-storage:/qdrant/storage qdrant/qdrant
```

برای اجرای دوباره container موجود:

```bash
docker start smartshop-qdrant
```

برای دیدن logها:

```bash
docker logs smartshop-qdrant
```

برای تست ساده دسترسی:

```bash
curl http://localhost:6333
```

BaseUrl مورد انتظار برای پروژه:

```text
AiSearch__Qdrant__BaseUrl=http://localhost:6333
```

## اجرای API

ابتدا build و تست معماری را اجرا کنید:

```bash
dotnet build
```

```bash
dotnet test tests/SmartShop.ArchitectureTests/SmartShop.ArchitectureTests.csproj
```

سپس API را اجرا کنید:

```bash
dotnet run --project src/SmartShop.Api/SmartShop.Api.csproj
```

Migrationهای دیتابیس برای Catalog، Ordering و Payments در زمان startup از طریق module initializerها اجرا می‌شوند.

در محیط Development، Scalar باید از مسیر زیر در دسترس باشد:

```http
/scalar
```

OpenAPI JSON نیز باید از مسیر زیر در دسترس باشد:

```http
/openapi/v1.json
```

## تست دستی با Scalar

ترتیب پیشنهادی برای تست دستی:

1. `GET /health`
2. `GET /api/catalog/products`
3. `POST /api/orders`
4. `GET /api/orders`
5. `POST /api/payments`
6. `GET /api/orders/{id}`
7. `POST /api/ai-search/reindex`
8. `GET /api/ai-search/products?query=laptop&limit=5`

نمونه body برای `POST /api/orders`:

```json
{
  "customerName": "Ali Reza",
  "customerEmail": "ali@example.com",
  "items": [
    {
      "productId": "PRODUCT_ID_FROM_CATALOG",
      "quantity": 1
    }
  ]
}
```

نمونه body برای `POST /api/payments`:

```json
{
  "orderId": "ORDER_ID_FROM_ORDERING",
  "method": "FakeGateway"
}
```

## تست AiSearch

برای تست AiSearch این شرایط باید برقرار باشد:

- Catalog باید product داشته باشد.
- Qdrant باید در حال اجرا باشد.
- OpenAI API key باید تنظیم شده باشد.
- ابتدا endpoint مربوط به reindex را صدا بزنید.
- سپس endpoint جستجو را صدا بزنید.

ترتیب پیشنهادی:

1. `GET /api/catalog/products`
2. `POST /api/ai-search/reindex`
3. `GET /api/ai-search/products?query=laptop&limit=5`

اگر OpenAI key خالی یا اشتباه باشد، endpoint باید خطای واضحی برگرداند. اگر Qdrant خاموش باشد یا `AiSearch__Qdrant__BaseUrl` اشتباه باشد، endpoint باید خطای واضحی درباره عدم دسترسی به Qdrant برگرداند.

## اجرای k6 در آینده

k6 برای build پروژه لازم نیست. این تست‌ها را فقط زمانی اجرا کنید که API و dependencyهای runtime با موفقیت بالا آمده باشند.

```bash
k6 run perf/k6/smoke.js
```

```bash
k6 run -e BASE_URL=http://localhost:5217 perf/k6/catalog-load.js
```

```bash
k6 run -e BASE_URL=http://localhost:5217 perf/k6/order-payment-flow.js
```

```bash
k6 run -e BASE_URL=http://localhost:5217 perf/k6/ai-search-smoke.js
```

اسکریپت `ai-search-smoke.js` به Qdrant در حال اجرا و تنظیمات OpenAI نیاز دارد.

## عیبیابی سریع

مشکلات رایج:

- Connection String مربوط به SQL Server اشتباه است.
- دیتابیس از اجرای قبلی باقی مانده و migrationهای قدیمی یا ناسازگار دارد.
- container مربوط به Qdrant در حال اجرا نیست.
- OpenAI API key خالی است.
- Scalar بعد از اضافه شدن endpointها هنوز نسخه قبلی را نشان می‌دهد و نیاز به restart API یا hard refresh مرورگر دارد.
- تست‌های معماری fail می‌شوند چون یک ماژول مستقیم به ماژول دیگر reference داده است.

برای reset کردن دیتابیس محلی تستی می‌توانید از دستور زیر استفاده کنید:

```bash
dotnet ef database drop --startup-project src/SmartShop.Api/SmartShop.Api.csproj --force
```

هشدار: این دستور دیتابیس محلی و داده‌های تستی شما را حذف می‌کند. فقط وقتی اجرا شود که از حذف داده‌های local مطمئن هستید.

## چکلیست نهایی اجرای کارگاه

- [ ] `dotnet build` پاس می‌شود.
- [ ] تست‌های معماری پاس می‌شوند.
- [ ] API start می‌شود.
- [ ] Scalar باز می‌شود.
- [ ] Catalog محصولات را برمی‌گرداند.
- [ ] سفارش ساخته می‌شود.
- [ ] پرداخت ساخته می‌شود.
- [ ] سفارش به وضعیت Paid می‌رسد.
- [ ] Qdrant در حال اجرا است.
- [ ] OpenAI key تنظیم شده است.
- [ ] AiSearch reindex کار می‌کند.
- [ ] AiSearch search کار می‌کند.
- [ ] k6 smoke test اجرا می‌شود.
