# بک‌لاگ تصمیمات معماری

این سند فهرست تصمیمات معماری‌ای را نگهداری می‌کند که در طول کارگاه ثبت خواهند شد.

هر تصمیم معماری در قالب ADR یا Architecture Decision Record مستند می‌شود.

## فهرست ADRها

| شناسه | عنوان | وضعیت |
|---|---|---|
| ADR-0001 | استفاده از معماری Modular Monolith | Accepted |
| ADR-0002 | استفاده از ASP.NET Core 10 | Accepted |
| ADR-0003 | استفاده از SQL Server | Accepted |
| ADR-0004 | استفاده از Schema-per-Module | Accepted |
| ADR-0005 | استفاده از Onion Architecture داخل هر ماژول | Accepted |
| ADR-0006 | استفاده از قراردادهای explicit بین ماژول‌ها | Accepted |
| ADR-0007 | استفاده از ارتباط in-process بین ماژول‌ها | Accepted |
| ADR-0008 | استفاده از Qdrant برای Vector Search | Proposed |
| ADR-0009 | ایزوله کردن AI Integration در ماژول AiSearch | Proposed |
| ADR-0010 | استفاده از Architecture Tests برای کنترل مرزها | Proposed |
| ADR-0011 | استفاده از k6 برای Performance Testing | Proposed |
| ADR-0012 | استفاده از Docker Compose برای اجرای local | Proposed |
| ADR-0013 | ایجاد Loyalty به‌عنوان Microservice مستقل | Accepted |
| ADR-0014 | استفاده از Database-per-Service برای سرویس‌های مستقل | Accepted |
| ADR-0015 | انتشار رویداد پرداخت موفق برای Ordering و Loyalty | Accepted |
| ADR-0016 | استفاده از Outbox و مصرف‌کننده Idempotent | Accepted |
| ADR-0017 | استخراج Payments با Strangler Gateway | Accepted |

## قالب استاندارد ADR

هر ADR باید با ساختار زیر نوشته شود:

```text
# ADR-XXXX: عنوان تصمیم

## وضعیت

Accepted

## زمینه

چه مسئله‌ای داریم؟
چه محدودیت‌هایی داریم؟
چه چیزی باعث شده این تصمیم مهم باشد؟

## تصمیم

چه تصمیمی گرفته شد؟

## پیامدها

پیامدهای مثبت و منفی این تصمیم چیست؟

## گزینه‌های بررسی‌شده

چه گزینه‌های دیگری بررسی شدند و چرا انتخاب نشدند؟

## تصمیمات مرتبط

این تصمیم به کدام ADRهای دیگر مرتبط است؟
```

## وضعیت‌های مجاز

- Proposed
- Accepted
- Superseded
- Deprecated
