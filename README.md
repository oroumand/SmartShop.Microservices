# کارگاه معماری Modular Monolith با ASP.NET Core

این ریپازیتوری مربوط به یک کارگاه عملی معماری نرم‌افزار است.

در این کارگاه یک مینی فروشگاه اینترنتی با معماری Modular Monolith توسعه داده می‌شود. هدف اصلی، آموزش عملی مفاهیم معماری نرم‌افزار شامل تصمیم‌گیری معماری، مرزبندی ماژول‌ها، Onion Architecture، تست معماری، تست پرفورمنس، Docker و یکپارچه‌سازی AI برای جستجوی هوشمند محصولات است.

## هدف کارگاه

هدف این پروژه ساخت یک فروشگاه کامل و production-ready نیست. هدف این است که نشان دهیم چگونه می‌توان یک مسئله واقعی را به تصمیمات معماری، ماژول‌های مستقل، ساختار کد قابل نگهداری و تست‌های قابل اجرا تبدیل کرد.

## سناریوی محصول

محصول نمونه، یک مینی فروشگاه اینترنتی به نام SmartShop است.

قابلیت‌های اصلی:

- مشاهده محصولات
- جستجوی معمولی محصولات
- جستجوی هوشمند و معنایی محصولات با استفاده از RAG
- ثبت سفارش
- شبیه‌سازی پرداخت
- اجرای تست‌های معماری
- اجرای تست‌های پرفورمنس
- اجرای کل سیستم با Docker Compose

## سبک معماری

سبک اصلی معماری پروژه، Modular Monolith است.

در این سبک، سیستم به صورت یک واحد deploy می‌شود، اما از نظر کد، دیتابیس، قراردادها و قوانین وابستگی، به ماژول‌های مستقل تقسیم می‌شود.

## ماژول‌ها

- Catalog
- Ordering
- Payments
- AiSearch
- SharedKernel
- ModuleContracts

## تکنولوژی‌ها

- ASP.NET Core 10
- SQL Server
- Entity Framework Core
- Qdrant
- Docker و Docker Compose
- k6
- Architecture Tests

## ساختار کلی ریپازیتوری

```text
src/
  SmartShop.Api/
  Modules/
  BuildingBlocks/

tests/
  SmartShop.ArchitectureTests/
  SmartShop.IntegrationTests/

docs/
  adr/
  diagrams/

perf/
  k6/

docker/
```

## جریان کارگاه

1. شناخت محصول و نیازمندی‌ها
2. استخراج ویژگی‌های معماری
3. تعیین مرز ماژول‌ها
4. ثبت تصمیمات معماری با ADR
5. ساخت Solution
6. پیاده‌سازی ماژول‌ها با Onion Architecture
7. نوشتن Architecture Tests
8. پیاده‌سازی جستجوی هوشمند با RAG
9. اجرای Performance Tests
10. Dockerize کردن اپلیکیشن
11. بررسی مسیر مهاجرت احتمالی به Microservices

## اجرای پروژه با SQL Server محلی

در این مرحله از کارگاه هنوز Docker Compose اضافه نشده است. فرض فعلی این است که SQL Server روی سیستم توسعه‌دهنده به صورت محلی نصب و در حال اجرا است.

Connection String پیش‌فرض پروژه در `src/SmartShop.Api/appsettings.json` از Windows Authentication و سرور `localhost` استفاده می‌کند:

```json
"SmartShopDb": "Server=localhost;Database=SmartShop;Trusted_Connection=True;TrustServerCertificate=True"
```

اگر از SQL Express استفاده می‌کنید، مقدار `Server=localhost` را به این مقدار تغییر دهید:

```text
Server=localhost\SQLEXPRESS
```

اگر از SQL Authentication استفاده می‌کنید، Connection String را متناسب با نام کاربری و رمز عبور SQL Server خود جایگزین کنید.

برای build و اجرای API:

```bash
dotnet build
dotnet run --project src/SmartShop.Api/SmartShop.Api.csproj
```

در زمان اجرای برنامه، Migrationهای ماژول Catalog اعمال می‌شوند و داده‌های نمونه محصولات در صورت خالی بودن جدول seed می‌شوند.

نمونه آدرس‌ها:

```http
GET http://localhost:{PORT}/health
GET http://localhost:{PORT}/api/catalog/products
GET http://localhost:{PORT}/api/catalog/products/search?query=laptop
```

## وضعیت فعلی جستجوی هوشمند

ماژول AiSearch در این مرحله wire شده است، اما روی این ماشین تست runtime نشده است.

برای تست runtime در مرحله‌های بعد، این ماژول به OpenAI API Key و Qdrant در حال اجرا نیاز دارد. فعلاً معیار اعتبارسنجی فقط موفق بودن `dotnet build` است و نباید OpenAI یا Qdrant در زمان build یا startup صدا زده شوند.

Endpointهای اضافه‌شده:

```http
POST http://localhost:{PORT}/api/ai-search/reindex
GET http://localhost:{PORT}/api/ai-search/products?query=laptop&limit=5
```

## رابط تست API با Scalar

در محیط Development، مستندات و رابط تست API با Scalar فعال است.

- رابط Scalar در این آدرس در دسترس است:

```http
http://localhost:{PORT}/scalar
```

- خروجی OpenAPI JSON در این آدرس در دسترس است:

```http
http://localhost:{PORT}/openapi/v1.json
```

هنگام اجرای API، مرورگر باید به صورت خودکار صفحه Scalar را باز کند. اگر مرورگر خودکار باز نشد، به صورت دستی به آدرس `http://localhost:{PORT}/scalar` یا در صورت استفاده از HTTPS به `https://localhost:{PORT}/scalar` بروید.

## تست‌های معماری

تست‌های معماری در پروژه `tests/SmartShop.ArchitectureTests` قرار دارند.

این تست‌ها مرزهای Modular Monolith را کنترل می‌کنند و اجازه نمی‌دهند لایه‌های داخلی ماژول‌ها به شکل اشتباه به هم وابسته شوند. برای مثال Domain نباید به Infrastructure وابسته شود، Application نباید EF Core یا Endpointها را بشناسد، و ماژول‌ها نباید مستقیم به پروژه‌های داخلی ماژول‌های دیگر reference بدهند.

برای اجرای فقط تست‌های معماری:

```bash
dotnet test tests/SmartShop.ArchitectureTests/SmartShop.ArchitectureTests.csproj
```

## اسکریپت‌های پرفورمنس

اسکریپت‌های k6 برای مرحله‌های بعدی کارگاه در مسیر `perf/k6` قرار دارند.

این اسکریپت‌ها در build معمولی پروژه اجرا نمی‌شوند و برای اجرای `dotnet build` به نصب k6 نیاز نیست.

نمونه اجرای بعدی، زمانی که API در حال اجرا باشد و k6 نصب شده باشد:

```bash
k6 run perf/k6/smoke.js
k6 run -e BASE_URL=http://localhost:5217 perf/k6/order-payment-flow.js
```

## اجرای کامل پروژه

برای اجرای کامل پروژه روی ماشین runtime، از این سندها استفاده کنید:

- `docs/runtime-setup.md`
- `docs/configuration.md`
- `perf/k6/README.md`

Docker Compose در این مرحله عمدا اضافه نشده است. تست runtime ماژول AiSearch به Qdrant در حال اجرا و تنظیمات OpenAI نیاز دارد.

اعتبارسنجی معمول فعلی:

```bash
dotnet build
dotnet test tests/SmartShop.ArchitectureTests/SmartShop.ArchitectureTests.csproj
```

## تست جریان ثبت سفارش

ماژول Ordering نیز از همان SQL Server محلی و همان Connection String با کلید `SmartShopDb` استفاده می‌کند. در زمان اجرای API، Migrationهای این ماژول هم روی schema مخصوص `ordering` اعمال می‌شوند.

برای تست ساده جریان سفارش:

1. API را اجرا کنید.
2. این آدرس را صدا بزنید و یک شناسه محصول کپی کنید:

```http
GET http://localhost:{PORT}/api/catalog/products
```

3. با شناسه محصول کپی‌شده یک سفارش ثبت کنید:

```http
POST http://localhost:{PORT}/api/orders
Content-Type: application/json

{
  "customerId": "11111111-1111-1111-1111-111111111111",
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

4. سفارش‌ها را دریافت کنید:

```http
GET http://localhost:{PORT}/api/orders
```

5. جزئیات یک سفارش را دریافت کنید:

```http
GET http://localhost:{PORT}/api/orders/{id}
```

شماره پورت واقعی در خروجی console هنگام اجرای برنامه نمایش داده می‌شود.

## وضعیت فعلی

این ریپازیتوری در حال آماده‌سازی برای کارگاه عملی معماری نرم‌افزار است.
