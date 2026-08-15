# مرز ماژول‌ها در SmartShop

## هدف این سند

این سند مرز ماژول‌های SmartShop را مشخص می‌کند.

سیستم با سبک Modular Monolith پیاده‌سازی می‌شود. بنابراین همه ماژول‌ها در نهایت در یک اپلیکیشن اجرا و deploy می‌شوند، اما از نظر کد، دیتابیس، قراردادها و قوانین وابستگی باید مستقل باقی بمانند.

## قرارداد ساختار داخلی هر ماژول

هر ماژول سه بخش اصلی دارد:

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

## ماژول Catalog

### مسئولیت

ماژول Catalog مالک اطلاعات محصولات است.

### مالکیت‌ها

- Product
- نام محصول
- توضیحات محصول
- قیمت محصول
- دسته‌بندی محصول
- وضعیت فعال یا غیرفعال بودن محصول
- جستجوی کلمه‌ای محصول
- schema دیتابیس catalog

### چیزهایی که مالک آن نیست

- سفارش
- پرداخت
- بردارهای جستجوی معنایی
- وضعیت پرداخت
- هویت کاربر

### قابلیت‌های عمومی

- دریافت لیست محصولات
- دریافت جزئیات محصول
- جستجوی محصولات با کلمه کلیدی
- ارائه اطلاعات لازم محصول از طریق contractهای explicit

## ماژول Ordering

### مسئولیت

ماژول Ordering مالک چرخه سفارش است.

### مالکیت‌ها

- Order
- OrderItem
- وضعیت سفارش
- محاسبه مبلغ سفارش
- schema دیتابیس ordering

### چیزهایی که مالک آن نیست

- اطلاعات اصلی محصول
- جستجوی محصول
- جزئیات پرداخت
- جستجوی برداری

### قابلیت‌های عمومی

- ایجاد سفارش
- دریافت سفارش
- تغییر وضعیت سفارش پس از پرداخت موفق

## ماژول Payments

### مسئولیت

ماژول Payments مالک پرداخت‌ها و وضعیت پرداخت است.

### مالکیت‌ها

- Payment
- PaymentStatus
- شبیه‌سازی پرداخت
- schema دیتابیس payment

### چیزهایی که مالک آن نیست

- جزئیات آیتم‌های سفارش
- اطلاعات محصول
- درگاه پرداخت واقعی در نسخه کارگاهی

### قابلیت‌های عمومی

- شبیه‌سازی پرداخت
- دریافت وضعیت پرداخت

## ماژول AiSearch

### مسئولیت

ماژول AiSearch مالک جستجوی معنایی محصولات است.

### مالکیت‌ها

- ProductSearchDocument
- abstraction تولید embedding
- ارتباط با Qdrant
- endpoint جستجوی هوشمند
- schema دیتابیس ai در صورت نیاز به metadata

### چیزهایی که مالک آن نیست

- اطلاعات اصلی محصول
- چرخه سفارش
- چرخه پرداخت

### قابلیت‌های عمومی

- index کردن محصولات برای جستجوی معنایی
- جستجوی محصولات با زبان طبیعی
- برگرداندن شناسه محصول و امتیاز شباهت

## SharedKernel

### مسئولیت

SharedKernel فقط شامل building blockهای عمومی و پایدار است.

### موارد مجاز

- Entity پایه
- ValueObject پایه
- DomainEvent abstraction
- Result type
- Guard helper
- Clock abstraction

### موارد غیرمجاز

- منطق بیزینسی
- مفهوم خاص محصول
- مفهوم خاص سفارش
- مفهوم خاص پرداخت
- مفهوم خاص AI

## ModuleContracts

### مسئولیت

ModuleContracts شامل contractهای explicit برای ارتباط کنترل‌شده بین ماژول‌ها است.

### موارد مجاز

- DTOهای عمومی بین ماژول‌ها
- Integration event contractها
- abstractionهای لازم برای ارتباط ماژول‌ها در صورت نیاز

### موارد غیرمجاز

- EF Core Entity
- DbContext
- پیاده‌سازی Infrastructure
- مدل داخلی Domain یک ماژول

## قوانین وابستگی مجاز

```text
SmartShop.Api -> Module Endpoints

Module.Endpoints -> Module.Core.Application

Module.Core.Application -> Module.Core.Domain

Module.Infra.Data -> Module.Core.Application
Module.Infra.Data -> Module.Core.Domain

Module -> SmartShop.SharedKernel
Module -> SmartShop.ModuleContracts
```

## قوانین وابستگی غیرمجاز

```text
Module.Core.Domain -> Module.Core.Application
Module.Core.Domain -> Module.Infra.Data
Module.Core.Application -> Module.Infra.Data
Module.Core.Application -> Module.Endpoints

Any Module -> Another Module's Infra project
Any Module -> Another Module's Domain project

SharedKernel -> Any Module
```

## مرز دیتابیس

برای سادگی کارگاه، همه ماژول‌ها از یک SQL Server Database استفاده می‌کنند، اما هر ماژول schema خودش را دارد.

```text
catalog
ordering
payment
ai
```

این تصمیم باعث می‌شود سیستم ساده بماند، اما ownership داده‌ها برای هر ماژول مشخص باشد.

## مسیر مهاجرت احتمالی به Microservices

یک ماژول زمانی کاندیدای استخراج به Microservice می‌شود که:

- قابلیت بیزینسی مشخصی داشته باشد.
- مالک schema دیتابیس خودش باشد.
- contractهای explicit داشته باشد.
- سایر ماژول‌ها به جزئیات داخلی آن وابسته نباشند.
- دلیل واقعی برای independent deployment یا independent scaling وجود داشته باشد.
