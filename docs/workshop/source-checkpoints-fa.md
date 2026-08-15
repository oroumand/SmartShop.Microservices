# Checkpointهای سورس کارگاه

## هدف

شرکت‌کنندگان نباید زمان کارگاه را صرف تایپ حجم زیادی از Boilerplate کنند. Repository باید در هر نقطه حساس یک Checkpoint قابل بازگشت داشته باشد و اختلاف هر Checkpoint فقط همان مفهوم آموزشی را نشان دهد.

## راهبرد Git

- `main`: نسخه نهایی و قابل اجرای کارگاه.
- Tagها: نقاط شروع و پایان هر مرحله.
- Commitها: هر تصمیم معماری یا قابلیت آموزشی در یک Commit متمرکز.
- شاخه‌های طولانی‌مدت آموزشی ایجاد نمی‌شوند تا تاریخچه خطی و قابل دنبال‌کردن بماند.

## Checkpoint صفر: Baseline

Tag پیشنهادی: `workshop-00-modular-monolith`

- همان SmartShop اجراشده در کارگاه قبلی.
- Catalog، Ordering، Payments و AiSearch داخل یک Host.
- هیچ Loyalty Service وجود ندارد.

## Checkpoint اول: سرویس مستقل Loyalty

Tag پیشنهادی: `workshop-01-loyalty-service`

- Host مستقل Loyalty.
- Domain و Persistence مستقل.
- API مشاهده مانده و گردش امتیاز.
- دیتابیس مستقل.
- Docker Compose اولیه.

## Checkpoint دوم: Integration Event مستقیم

Tag پیشنهادی: `workshop-02-direct-event`

- قرارداد `PaymentSucceededV1`.
- انتشار مستقیم رویداد پس از ثبت Payment.
- مصرف رویداد در Loyalty.
- نمایش عمدی ریسک Dual Write.

این Checkpoint عمداً طراحی نهایی نیست و برای ایجاد مسئله آموزشی استفاده می‌شود.

## Checkpoint سوم: Outbox و Idempotency

Tag پیشنهادی: `workshop-03-reliable-messaging`

- Outbox در مرز Payments.
- Background Publisher.
- ثبت شناسه پیام پردازش‌شده در Loyalty.
- Ack پیام پس از Commit تراکنش Loyalty.
- امکان بازپخش امن پیام.

## Checkpoint چهارم: استخراج Payments

Tag پیشنهادی: `workshop-04-extract-payments`

- Host مستقل Payments.
- دیتابیس مستقل Payments.
- قرارداد HTTP داخلی برای خواندن اطلاعات قابل پرداخت سفارش.
- مصرف `PaymentSucceededV1` در Ordering.
- حذف وابستگی مستقیم Payments به Infrastructure ماژول Ordering.

## Checkpoint پنجم: Strangler Gateway

Tag پیشنهادی: `workshop-05-strangler-gateway`

- YARP Gateway.
- Routeهای Catalog، Ordering و AiSearch به Modular Monolith.
- Route Payments به سرویس Payments.
- Route Loyalty به سرویس Loyalty.
- یک Endpoint عمومی واحد برای مصرف‌کننده.

## Checkpoint ششم: Production Readiness پایه

Tag پیشنهادی: `workshop-06-production-readiness`

- Timeout، Retry و Circuit Breaker برای ارتباط هم‌زمان.
- Health Check سرویس، دیتابیس و Broker.
- Correlation و Trace پایه.
- Unit، Architecture، Contract و Integration Testهای منتخب.
- Runbook اجرای دمو و سناریوی خرابی.

## قواعد نمایش کد

- در کلاس فقط Diff مرتبط با مفهوم جاری نمایش داده شود.
- Migrationها، فایل‌های تولیدشده و تنظیمات طولانی از قبل آماده باشند.
- هر Demo با یک دستور یا Script کوتاه اجرا شود.
- برای هر Failure Demo، مسیر بازگشت و Reset مشخص باشد.
- Secret واقعی در Repository قرار نگیرد.
- نسخه نهایی تنها منبع شروع آموزش نیست؛ Checkpoint صفر نقطه شروع دانشجو است.

