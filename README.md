# SmartShop Microservices

SmartShop یک نمونه‌ی اجرایی ASP.NET Core برای نمایش مهاجرت تدریجی از Modular Monolith به معماری service-based است. این مخزن فقط سورس، تصمیم‌های معماری، قراردادها، تست‌ها و مستندات فنی اجرا را نگه می‌دارد.

## معماری فعلی

- `SmartShop.Api`: قابلیت‌های Catalog، Ordering و AiSearch در Modular Monolith
- `SmartShop.Payments.Api`: سرویس مستقل Payments با database اختصاصی و Transactional Outbox
- `SmartShop.Loyalty.Api`: سرویس مستقل Loyalty با database اختصاصی و Idempotent Consumer/Inbox
- `SmartShop.Gateway`: ورودی عمومی مبتنی بر YARP و مسیر مهاجرت Strangler
- `RabbitMQ`: انتشار `PaymentSucceededV1` برای Ordering و Loyalty
- `SQL Server`: یک server محلی با databaseهای مستقل برای ownership سرویس‌ها

## جریان پرداخت

1. Client سفارش را از طریق Gateway در Ordering ثبت می‌کند.
2. Payments، projection داخلی سفارش را با HTTP و resilience policy می‌خواند.
3. Payment و intent انتشار event در یک transaction محلی ذخیره می‌شوند.
4. Outbox worker رویداد `PaymentSucceededV1` را در RabbitMQ منتشر می‌کند.
5. Ordering وضعیت سفارش را idempotently به Paid تغییر می‌دهد.
6. Loyalty همان event را idempotently مصرف و امتیاز مشتری را محاسبه می‌کند.

## اجرای محلی

پیش‌نیازها: Docker Engine و Docker Compose v2.

```bash
docker compose up --build -d
docker compose ps
```

ورودی عمومی سیستم:

```text
http://localhost:8088
```

Health endpointها:

```text
GET http://localhost:8088/health/live
GET http://localhost:8080/health/ready
GET http://localhost:8081/health/ready
GET http://localhost:8082/health/ready
```

## Route map

| مسیر | مقصد |
|---|---|
| `/api/payments/**` | Payments |
| `/api/loyalty/**` | Loyalty |
| `/api/**` | SmartShop.Api |

مسیرهای `/internal/**` از Gateway عبور نمی‌کنند و فقط برای ارتباط داخلی سرویس‌ها هستند.

## ساخت و تست

```bash
dotnet restore SmartShop.sln
dotnet build SmartShop.sln --no-restore
dotnet test SmartShop.sln --no-build
docker compose config --quiet
```

## مستندات فنی

- تصمیم‌های معماری: [`docs/adr`](docs/adr)
- تنظیمات: [`docs/configuration.md`](docs/configuration.md)
- راه‌اندازی runtime: [`docs/operations-runbook.md`](docs/operations-runbook.md)
- مرز قابلیت‌ها: [`docs/module-boundaries.md`](docs/module-boundaries.md)

## Checkpoint tags

تگ‌های `checkpoint-00` تا `checkpoint-06` snapshotهای فنی تکامل معماری هستند و هرکدام باید مستقل build شوند.
