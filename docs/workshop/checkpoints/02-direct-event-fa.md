# Checkpoint 02 — Integration Event مستقیم

در این مرحله، پس از موفق شدن پرداخت، رویداد نسخه‌دار `PaymentSucceededV1` روی RabbitMQ منتشر می‌شود و Loyalty با دریافت آن امتیاز مشتری را محاسبه می‌کند.

## جریان موفق

1. سفارش با یک `CustomerId` پایدار ثبت می‌شود.
2. Payments سفارش را پرداخت می‌کند.
3. رکورد Payment ذخیره و Order به Paid تبدیل می‌شود.
4. `PaymentSucceededV1` منتشر می‌شود.
5. Loyalty پیام را consume می‌کند، account را می‌سازد و به ازای هر ۱۰۰ واحد مبلغ یک امتیاز می‌دهد.

## دمو

```bash
docker compose up --build
```

- SmartShop API: `http://localhost:8080`
- Loyalty API: `http://localhost:8081`
- RabbitMQ Management: `http://localhost:15672` با `smartshop/smartshop_local_123`

پس از ایجاد سفارش و پرداخت، balance را با `CustomerId` سفارش بخوانید:

```http
GET http://localhost:8081/api/loyalty/customers/11111111-1111-1111-1111-111111111111
```

## آزمایش شکست

RabbitMQ را متوقف کنید و دوباره پرداخت بزنید. پرداخت ممکن است در دیتابیس موفق شده باشد، اما endpoint خطا می‌دهد و event از دست می‌رود. سپس RabbitMQ را بالا بیاورید: چیزی وجود ندارد که event گمشده را دوباره بسازد.

این checkpoint عمداً ناقص است. مسئله‌ای که باید از دانشجو بپرسیم:

> چطور state و intent انتشار event را بدون distributed transaction اتمیک کنیم؟

پاسخ checkpoint بعدی: Transactional Outbox. همچنین تحویل broker می‌تواند تکراری باشد؛ بنابراین consumer نیز باید idempotent شود.
