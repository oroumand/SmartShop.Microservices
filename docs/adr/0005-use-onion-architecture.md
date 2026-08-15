# ADR-0005: استفاده از Onion Architecture داخل هر ماژول

## وضعیت

Accepted

## زمینه

هر ماژول SmartShop باید استقلال داخلی داشته باشد و منطق بیزینس آن به زیرساخت‌هایی مثل EF Core، SQL Server، Qdrant یا OpenAI وابسته نباشد.

اگر منطق بیزینس مستقیماً به زیرساخت وابسته شود، تست‌پذیری و قابلیت نگهداری کاهش پیدا می‌کند. همچنین استخراج یک ماژول در آینده سخت‌تر خواهد شد.

در این کارگاه می‌خواهیم شرکت‌کننده‌ها ببینند که Modular Monolith فقط تقسیم فولدرها نیست؛ هر ماژول باید از داخل هم معماری تمیز داشته باشد.

## تصمیم

داخل هر ماژول از Onion Architecture استفاده می‌کنیم.

ساختار داخلی هر ماژول به این شکل است:

```text
ModuleName/
  01.Core/
    SmartShop.ModuleName.Core.Domain/
    SmartShop.ModuleName.Core.Application/

  02.Infra/
    SmartShop.ModuleName.Infra.Data/
    SmartShop.ModuleName.Infra.OtherAdapter/

  03.Endpoints/
    SmartShop.ModuleName.Endpoints/
```

قوانین اصلی:

- Domain مرکز ماژول است و به هیچ لایه‌ای وابسته نیست.
- Application به Domain وابسته است و use caseها را پیاده‌سازی می‌کند.
- Infra به Application و Domain وابسته است و adapterهای بیرونی را پیاده‌سازی می‌کند.
- Endpoints به Application وابسته است و APIهای ماژول را expose می‌کند.
- وابستگی‌ها باید به سمت Core باشند، نه برعکس.

## پیامدها

### پیامدهای مثبت

- منطق بیزینس از زیرساخت جدا می‌شود.
- تست Domain و Application ساده‌تر می‌شود.
- EF Core، SQL Server، Qdrant و OpenAI به Core نشت نمی‌کنند.
- تغییر زیرساخت با اثر کمتر روی Core امکان‌پذیر می‌شود.
- ساختار داخلی هر ماژول برای شرکت‌کننده‌ها قابل فهم است.
- مسیر استخراج ماژول به سرویس مستقل در آینده شفاف‌تر می‌شود.

### پیامدهای منفی

- تعداد پروژه‌ها و فایل‌ها بیشتر می‌شود.
- برای پروژه خیلی کوچک ممکن است over-engineering به نظر برسد.
- نیاز به توضیح دقیق قوانین وابستگی دارد.
- در کارگاه باید مراقب باشیم زمان زیادی صرف ceremony نشود.

## گزینه‌های بررسی‌شده

### Layered Architecture ساده

مزیت‌ها:

- آشنا و ساده
- تعداد پروژه کمتر

دلیل انتخاب نشدن:

- احتمال وابستگی Core به Infrastructure بیشتر است.
- استقلال هر ماژول را به اندازه کافی نشان نمی‌دهد.
- برای هدف آموزشی Onion و dependency inversion ضعیف‌تر است.

### Clean Architecture در سطح کل Solution

مزیت‌ها:

- ساختار تمیز
- جداسازی خوب concernها

دلیل انتخاب نشدن:

- ممکن است تمرکز را از ماژول‌ها به لایه‌های کلان Solution ببرد.
- هدف ما این است که هر ماژول از داخل مستقل باشد، نه اینکه کل سیستم فقط یک Clean Architecture بزرگ باشد.

### Onion Architecture داخل هر ماژول

مزیت‌ها:

- استقلال داخلی ماژول‌ها
- جداسازی Core از Infra
- مناسب برای Architecture Tests
- مناسب برای آموزش مرزبندی در Modular Monolith

دلیل انتخاب:

- با هدف کارگاه و ساختار پیشنهادی Solution بیشترین هم‌خوانی را دارد.

## تصمیمات مرتبط

- ADR-0001: استفاده از معماری Modular Monolith
- ADR-0004: استفاده از Schema-per-Module
- ADR-0010: استفاده از Architecture Tests برای کنترل مرزها
