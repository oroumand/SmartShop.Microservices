# Checkpoint 01 — اولین Microservice

در این checkpoint، قابلیت `Loyalty` از ابتدا به‌صورت یک سرویس مستقل ساخته می‌شود. هنوز هیچ پیام یا امتیازی وارد سیستم نمی‌شود؛ هدف این مرحله تثبیت مرز سرویس، مالکیت داده و قرارداد خواندن است.

## چیزی که باید دیده شود

- `SmartShop.Api` همان Modular Monolith قبلی است.
- `SmartShop.Loyalty.Api` یک process و deployable مستقل است.
- Loyalty دیتابیس `SmartShopLoyalty` را در اختیار دارد و به جدول‌های SmartShop دسترسی مستقیم ندارد.
- دو endpoint خواندنی وجود دارد:
  - `GET /api/loyalty/customers/{customerId}`
  - `GET /api/loyalty/customers/{customerId}/transactions`
- تا وقتی integration event اضافه نشده، balance برابر صفر است.

## اجرای محلی

```bash
cp .env.example .env
docker compose up --build
```

بعد از healthy شدن سرویس‌ها:

```http
GET http://localhost:8081/health/live
GET http://localhost:8081/health/ready
GET http://localhost:8081/api/loyalty/customers/11111111-1111-1111-1111-111111111111
```

## سؤال کارگاهی

اگر Loyalty هنوز داده‌ای تولید نمی‌کند، چرا از همین ابتدا دیتابیس جدا دارد؟

پاسخ مورد انتظار این نیست که «Microservice همیشه باید دیتابیس جدا داشته باشد». پاسخ این است که استقلال deploy و تغییر schema فقط وقتی واقعی است که سرویس دیگری مالک storage آن نباشد. هزینه این تصمیم، حذف join و transaction مستقیم میان سرویس‌هاست؛ هزینه‌ای که در checkpointهای بعدی آشکار می‌شود.

## پایان دموی مدرس

روی این نکته مکث کنید: جدا کردن Solution Folder یا پروژه C# مرز runtime ایجاد نمی‌کند. شواهد مرز runtime در این checkpoint عبارت‌اند از process، port، configuration، health check، image و database مستقل.
