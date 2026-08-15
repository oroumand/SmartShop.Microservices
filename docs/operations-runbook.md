# SmartShop Operations Runbook

این سند برای اجرای تکرارپذیر topology، health checkها و failure scenarioهای SmartShop است.

## پیش‌نیازها

- Docker Engine و Docker Compose v2
- `curl`
- `jq` برای اسکریپت‌های end-to-end
- پورت‌های آزاد `1433`, `5672`, `15672`, `8080`, `8081`, `8082`, `8088`

## راه‌اندازی

```bash
./scripts/dev/up.sh
```

ورودی عمومی `http://localhost:8088` است. پورت‌های مستقیم فقط برای تشخیص و مشاهده‌ی ownership باز مانده‌اند:

| Component | URL |
|---|---|
| Gateway | `http://localhost:8088` |
| Modular Monolith | `http://localhost:8080` |
| Loyalty | `http://localhost:8081` |
| Payments | `http://localhost:8082` |
| RabbitMQ Management | `http://localhost:15672` |

## Health semantics

- `/health/live`: process قادر به پاسخ‌گویی است و dependencyها در تصمیم restart دخالت ندارند.
- `/health/ready`: dependencyهای لازم برای دریافت traffic بررسی می‌شوند.
- Payments، RabbitMQ را شرط readiness نمی‌داند؛ چون Outbox اجازه می‌دهد هنگام outage broker همچنان payment را locally commit کند.
- Gateway در readiness هر سه backend را بررسی می‌کند. در محیطی با partial routing می‌توان این policy را per-route کرد.

## Happy path

```bash
./scripts/dev/smoke-test.sh
```

خروجی موفق شامل `correlationId`، `orderId`، `paymentId`، وضعیت `Paid` و balance مثبت است.

برای دنبال‌کردن همان درخواست در logها:

```bash
docker compose logs gateway smartshop-api payments-api loyalty-api | grep CORRELATION_ID
```

مقدار `CORRELATION_ID` را با مقدار خروجی اسکریپت جایگزین کنید.

## Outbox failure and recovery

```bash
./scripts/dev/outbox-failure-demo.sh
```

سناریو RabbitMQ را متوقف می‌کند، یک payment را با موفقیت در database ثبت می‌کند، backlog را از `GET http://localhost:8082/ops/outbox` نشان می‌دهد، broker را بالا می‌آورد و تا drainشدن backlog صبر می‌کند.

سیگنال‌های مورد انتظار:

1. درخواست payment هنگام outage با `201` تمام می‌شود.
2. `pendingCount` در diagnostics بزرگ‌تر از صفر است.
3. پس از recovery، Outbox worker event را publish می‌کند.
4. `pendingCount` به صفر می‌رسد.
5. Ordering و Loyalty اثر event را اعمال می‌کنند.

## تشخیص خطا

```bash
docker compose ps
docker compose logs --tail=200 payments-api
docker compose logs --tail=200 loyalty-api
docker compose logs --tail=200 smartshop-api
docker compose logs --tail=200 rabbitmq
curl --fail-with-body http://localhost:8082/ops/outbox
```

اگر migration شکست خورد، ابتدا connection string و health SQL Server را بررسی کنید. برای پاک‌کردن کامل state محلی فقط با تأیید صریح اجرا کنید:

```bash
./scripts/dev/reset.sh --yes
```

این دستور volume محلی SQL Server را حذف می‌کند و داده قابل بازیابی نیست.

## خاموش‌کردن بدون حذف داده

```bash
docker compose down
```
