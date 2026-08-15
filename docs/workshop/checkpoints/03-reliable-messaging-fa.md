# Checkpoint 03 — Outbox و Idempotent Consumer

این checkpoint دو failure window را می‌بندد:

- Payments، تغییر state و intent انتشار event را در یک دیتابیس transaction ذخیره می‌کند.
- Loyalty، اثر business و ثبت MessageId پردازش‌شده را در یک دیتابیس transaction ذخیره می‌کند.

## سمت Producer

`PayOrderService` دیگر مستقیماً RabbitMQ را صدا نمی‌زند. Payment و `OutboxMessage` با یک `SaveChangesAsync` در دیتابیس Payments ثبت می‌شوند. worker پس‌زمینه پیام‌های publish‌نشده را می‌خواند، publish می‌کند و `ProcessedAtUtc` می‌زند.

اگر process بعد از publish و قبل از ثبت `ProcessedAtUtc` crash کند، همان پیام دوباره publish خواهد شد. Outbox «exactly once delivery» نمی‌دهد؛ احتمال گم‌شدن intent را با احتمال duplicate عوض می‌کند.

## سمت Consumer

Loyalty پیش از اعمال پیام، `EventId` را در `ProcessedMessages` بررسی می‌کند. ثبت inbox و افزایش balance با یک `SaveChangesAsync` انجام می‌شوند. همچنین `SourcePaymentId` unique است تا دو event متفاوت برای یک payment دوبار امتیاز نسازند.

## آزمایش کارگاهی

1. RabbitMQ را متوقف کنید.
2. یک سفارش جدید را پرداخت کنید.
3. رکورد Payment و Outbox را در دیتابیس ببینید؛ درخواست پرداخت باید مستقل از دسترس‌پذیری Loyalty باشد.
4. RabbitMQ را بالا بیاورید.
5. worker پیام pending را publish می‌کند و balance بالا می‌رود.
6. همان payload را با همان `EventId` دوباره وارد queue کنید؛ balance نباید تغییر کند.

## نکته مدرس

عبارت دقیق برای کلاس:

> RabbitMQ به‌تنهایی قابل‌اعتمادبودن فرایند business ما را حل نمی‌کند. reliability از همکاری producer، broker و consumer به‌دست می‌آید.

در این نمونه هنوز محدودیت‌هایی داریم: cleanup/retention، poison-message policy، dead-letter queue، locking برای چند outbox worker و schema registry عمداً به‌عنوان production hardening باقی مانده‌اند.
