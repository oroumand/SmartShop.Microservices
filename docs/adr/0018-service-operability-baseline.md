# ADR-0018: تعریف حداقل baseline عملیاتی سرویس‌ها

## وضعیت

Accepted

## زمینه

جداکردن deployment unitها failure modeهای شبکه، broker و database را وارد سیستم می‌کند. صرف داشتن endpoint و container برای اداره‌پذیری کافی نیست؛ اپراتور باید بتواند یک request را دنبال کند، readiness را از زنده‌بودن process تفکیک کند و backlog پیام‌های منتشرنشده را ببیند.

## تصمیم

- همه‌ی HTTP hostها هدر `X-Correlation-ID` را می‌پذیرند یا تولید می‌کنند و آن را در log scope و response قرار می‌دهند.
- `/health/live` فقط زنده‌بودن process را نشان می‌دهد و dependency outage باعث restart loop نمی‌شود.
- `/health/ready` dependencyهای لازم برای دریافت traffic را بررسی می‌کند.
- Payments، RabbitMQ را شرط readiness قرار نمی‌دهد؛ Transactional Outbox هنگام outage broker پذیرش محلی payment را ممکن می‌کند.
- Payments در `/ops/outbox` تعداد پیام‌های pending، تعداد تلاش‌های شکست‌خورده و عمر قدیمی‌ترین پیام pending را گزارش می‌کند.
- مرز استخراج Payments و idempotency اثر پرداخت با architecture test و unit test محافظت می‌شوند.

## پیامدها

### مثبت

- تشخیص failure windowها با signalهای مشخص ممکن است.
- orchestrator می‌تواند restart و traffic admission را با semantics متفاوت انجام دهد.
- رشد backlog Outbox بدون query دستی database دیده می‌شود.
- regression در مرز Monolith و Payments سریع‌تر کشف می‌شود.

### منفی

- یک building block وب مشترک به hostها اضافه می‌شود.
- readiness Gateway در مدل فعلی به سلامت هر سه backend وابسته است و partial availability را مدل نمی‌کند.
- endpoint عملیاتی باید در محیط production پشت authorization یا شبکه مدیریتی قرار گیرد.

## گزینه‌های بررسی‌شده

### یک endpoint واحد `/health`

رد شد؛ زیرا failure dependency را با crash process یکی می‌کند و می‌تواند restart loop بسازد.

### قراردادن RabbitMQ در readiness Payments

رد شد؛ زیرا با تصمیم Outbox ناسازگار است و هنگام outage broker ظرفیت bufferشدن محلی را از بین می‌برد.

### اتکا به logهای بدون Correlation ID

رد شد؛ زیرا دنبال‌کردن یک request میان Gateway و چند backend قابل اتکا نیست.

## تصمیمات مرتبط

- ADR-0015
- ADR-0016
- ADR-0017
