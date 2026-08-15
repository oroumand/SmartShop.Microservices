# ADR-0007: استفاده از ارتباط in-process بین ماژول‌ها

## وضعیت

Accepted

## زمینه

SmartShop یک Modular Monolith است. بنابراین همه ماژول‌ها داخل یک process و یک deployable واحد اجرا می‌شوند.

در چنین ساختاری، می‌توان ارتباط بین ماژول‌ها را به شکل‌های مختلفی انجام داد:

- فراخوانی مستقیم serviceهای داخلی
- ارتباط از طریق contractهای explicit
- eventهای درون‌پردازه‌ای
- message broker خارجی
- HTTP call بین سرویس‌ها

از آنجا که در نسخه کارگاهی، سیستم هنوز به Microservices تبدیل نشده است، اضافه کردن message broker یا ارتباط HTTP بین ماژول‌ها پیچیدگی غیرضروری ایجاد می‌کند.

در عین حال، نباید اجازه دهیم ماژول‌ها بدون کنترل به implementation داخلی هم وابسته شوند.

## تصمیم

در نسخه اول SmartShop، ارتباط بین ماژول‌ها به صورت in-process و از طریق contractهای explicit انجام می‌شود.

قانون اصلی این است:

ماژول‌ها نباید به Infrastructure، DbContext یا Domain داخلی یکدیگر وابسته شوند.

در صورت نیاز به ارتباط بین ماژول‌ها، از یکی از روش‌های زیر استفاده می‌کنیم:

- قراردادهای موجود در `SmartShop.ModuleContracts`
- application-level abstractions
- in-process events
- services ثبت‌شده در Composition Root

پروژه `SmartShop.Api` نقش Composition Root را دارد و dependencyها را wire می‌کند.

## پیامدها

### پیامدهای مثبت

- ارتباط بین ماژول‌ها ساده و قابل فهم باقی می‌ماند.
- نیازی به message broker در نسخه کارگاهی نیست.
- اجرای local ساده‌تر است.
- debugging آسان‌تر است.
- latency شبکه‌ای بین ماژول‌ها وجود ندارد.
- همچنان می‌توان مرزهای ماژول‌ها را با contract و Architecture Tests کنترل کرد.

### پیامدهای منفی

- ماژول‌ها independent deployment ندارند.
- isolation runtime بین ماژول‌ها وجود ندارد.
- اگر قوانین رعایت نشوند، coupling داخلی افزایش پیدا می‌کند.
- برای مهاجرت به Microservices در آینده، باید ارتباط in-process به ارتباط remote یا event-driven تبدیل شود.

## گزینه‌های بررسی‌شده

### ارتباط HTTP بین ماژول‌ها

مزیت‌ها:

- شبیه‌تر به Microservices
- آماده‌تر برای استخراج سرویس‌ها

دلیل انتخاب نشدن:

- در یک monolith باعث پیچیدگی غیرضروری می‌شود.
- latency و failure mode اضافی ایجاد می‌کند.
- برای کارگاه 4 ساعته مناسب نیست.

### Message Broker از ابتدا

مزیت‌ها:

- decoupling بیشتر
- مناسب برای event-driven communication

دلیل انتخاب نشدن:

- setup و توضیح آن زمان‌بر است.
- consistency و failure handling پیچیده‌تر می‌شود.
- تمرکز کارگاه را از Modular Monolith دور می‌کند.

### فراخوانی مستقیم implementation داخلی ماژول‌ها

مزیت‌ها:

- ساده‌ترین راه برای کدنویسی سریع

دلیل انتخاب نشدن:

- مرز ماژول‌ها را از بین می‌برد.
- coupling شدید ایجاد می‌کند.
- با هدف آموزش معماری سازگار نیست.

### ارتباط in-process با contractهای explicit

مزیت‌ها:

- ساده است.
- برای Modular Monolith طبیعی است.
- مرزها را حفظ می‌کند.
- برای آموزش مناسب است.

دلیل انتخاب:

- بهترین trade-off برای نسخه کارگاهی SmartShop است.

## تصمیمات مرتبط

- ADR-0001: استفاده از معماری Modular Monolith
- ADR-0006: استفاده از قراردادهای explicit بین ماژول‌ها
- ADR-0010: استفاده از Architecture Tests برای کنترل مرزها
