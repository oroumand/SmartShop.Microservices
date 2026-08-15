# سرفصل نهایی کارگاه شش‌ساعته Microservices

## تعریف کارگاه

این کارگاه ادامه‌ی مستقیم کارگاه Modular Monolith است. نقطه‌ی شروع، SmartShop موجود با ماژول‌های Catalog، Ordering، Payments و AiSearch است. شرکت‌کنندگان ابتدا یک قابلیت جدید به نام Loyalty را از روی نیاز کسب‌وکاری تحلیل و به‌صورت Microservice طراحی می‌کنند؛ سپس یک قابلیت موجود، Payments، را به‌شکل مرحله‌ای از Monolith استخراج می‌کنند.

کارگاه فهرست ابزارها نیست. روایت آن یک تصمیم معماری پیوسته است:

> مسئله → نیازمندی → ویژگی معماری → مرز سرویس → قرارداد → شکست → بازطراحی → ADR → کد → آزمون Trade-off

مدت کل ۶ ساعت است: سه جلسه‌ی ۱۲۰ دقیقه‌ای که در هر جلسه ۵ دقیقه استراحت در نظر گرفته شده است.

---

## مخاطب و پیش‌نیاز

مخاطب، توسعه‌دهنده یا معمار نرم‌افزاری است که مفاهیم پایه‌ی معماری را می‌داند و کارگاه Modular Monolith را گذرانده است.

پیش‌نیاز دانشی:

- Boundary، Cohesion و Coupling
- Onion Architecture و Dependency Rule
- مالکیت داده و Schema-per-Module
- قرارداد صریح بین ماژول‌ها
- ADR و Trade-off Analysis
- ASP.NET Core، EF Core و Docker در سطح کارگاهی

پیش‌نیاز اجرایی مدرس:

- Docker Desktop یا Docker Engine با Compose
- Git
- یک REST client مانند فایل‌های `.http`، Postman یا Scalar
- پورت‌های `8080`، `8081`، `8082`، `8088`، `1433`، `5672` و `15672` آزاد
- clone شدن Repository و pull شدن imageها پیش از شروع کلاس

---

## خروجی قابل سنجش

شرکت‌کننده در پایان باید بتواند:

1. با شواهد کسب‌وکاری و عملیاتی توضیح دهد چرا یک Capability باید یا نباید Microservice شود.
2. Context، مالک داده، API و Integration Event یک سرویس را مشخص کند.
3. برای یک تعامل مشخص، Sync یا Async را با ذکر Failure Mode انتخاب کند.
4. تفاوت Command، Event و Query را در قراردادهای بین سرویس‌ها تشخیص دهد.
5. Partial Failure، Temporal Coupling و Eventual Consistency را روی سناریوی واقعی توضیح دهد.
6. خطر Dual Write را در کد پیدا و Transactional Outbox را پیشنهاد کند.
7. مصرف‌کننده‌ی At-least-once را به‌صورت Idempotent طراحی کند.
8. استخراج مرحله‌ای Payments را با Strangler Fig و بدون Big-bang Rewrite طراحی کند.
9. Unit، Integration، Contract و End-to-End Test را در جای درست قرار دهد.
10. یک Trade-off Ledger بنویسد و هزینه‌ی Microservices را در تصمیم نهایی لحاظ کند.

## شواهد یادگیری

| خروجی | شاهدی که شرکت‌کننده تولید می‌کند |
|---|---|
| تشخیص مرز سرویس | Context Canvas یک‌صفحه‌ای Loyalty |
| مالکیت داده | جدول «مالک / کپی مجاز / دسترسی ممنوع» |
| انتخاب ارتباط | Decision Matrix برای Payment → Loyalty |
| قرارداد | طرح `PaymentSucceededV1` با داده و version |
| قابلیت اطمینان | نمودار Transaction Boundary برای Outbox و Inbox |
| مهاجرت | Route Migration Plan برای Payments |
| تصمیم معماری | یک ADR کوتاه با Context، Decision و Consequences |
| جمع‌بندی | Trade-off Ledger نهایی |

---

## چرخه‌ی ثابت آموزش

هر مفهوم با این ریتم ارائه می‌شود:

1. **مسئله:** مدرس یک نیاز یا failure قابل مشاهده ارائه می‌کند.
2. **پاسخ اولیه:** گروه‌ها ۵ تا ۸ دقیقه راه‌حل خود را ثبت می‌کنند.
3. **تئوری Just-in-time:** فقط مفاهیمی تدریس می‌شوند که برای همان تصمیم لازم‌اند.
4. **بازطراحی:** گروه پاسخ خود را با معیارهای جدید اصلاح می‌کند.
5. **راه‌حل مدرس:** تصمیم پیشنهادی همراه Alternatives و Consequences نمایش داده می‌شود.
6. **ثبت تصمیم:** خروجی به ADR، Contract یا Diagram تبدیل می‌شود.
7. **اثبات در کد:** یک diff کوچک یا failure demo تصمیم را قابل مشاهده می‌کند.

قانون تسهیل‌گری: مدرس پاسخ نهایی را پیش از ثبت پاسخ اولیه‌ی گروه‌ها نشان نمی‌دهد.

---

# جلسه‌ی اول — از نیاز کسب‌وکاری تا اولین سرویس مستقل

## سؤال محوری

آیا Loyalty باید یک ماژول دیگر داخل SmartShop باشد یا ارزش استقلال آن از هزینه‌ی سیستم توزیع‌شده بیشتر است؟

## خروجی جلسه

هر گروه در پایان جلسه باید این چهار خروجی را داشته باشد:

- Context Canvas سرویس Loyalty
- Data Ownership Matrix
- ADR ایجاد Loyalty Microservice
- قرارداد HTTP خواندن balance و history

## Run Sheet — جلسه اول

| دقیقه | مدت | فعالیت |
|---:|---:|---|
| 0 | 8 | اتصال به کارگاه قبل |
| 8 | 10 | Product Brief |
| 18 | 10 | تمرین Trigger و Failure Policy |
| 28 | 14 | Business Capability و هزینه استقلال |
| 42 | 12 | Architecture Characteristics |
| 54 | 10 | تمرین Module یا Microservice |
| 64 | 5 | استراحت |
| 69 | 14 | Boundary و Coupling |
| 83 | 11 | Data Ownership |
| 94 | 11 | Context Canvas |
| 105 | 7 | راه‌حل مدرس و ADR |
| 112 | 4 | API-first و قرارداد |
| 116 | 4 | Demo و Exit Ticket |

## Product Brief قابل نمایش

SmartShop می‌خواهد نرخ بازگشت مشتری را افزایش دهد. پس از پرداخت موفق، مشتری به ازای هر ۱۰۰ واحد مبلغ، یک امتیاز کامل می‌گیرد. قواعد امتیاز ممکن است مستقل از Ordering تغییر کنند و در آینده کانال‌های دیگری نیز از Loyalty استفاده کنند. خرابی Loyalty نباید پرداخت موفق را Rollback کند.

ابهام‌هایی که عمداً باز می‌مانند:

- آیا ایمیل شناسه‌ی مشتری است؟
- Trigger «ثبت سفارش» است یا «پرداخت موفق»؟
- اگر Loyalty چند دقیقه unavailable باشد چه می‌شود؟
- آیا Payments باید balance را در response برگرداند؟
- Refund و خرج امتیاز در v1 هستند؟

## تمرین‌های جلسه اول

### تمرین ۱ — Requirement Questions

زمان: ۱۰ دقیقه. خروجی: حداکثر پنج سؤال و سه فرض.

پاسخ مورد انتظار مدرس:

- Trigger، پرداخت موفق است.
- `CustomerId` شناسه‌ی پایدار است؛ ایمیل attribute قابل تغییر است.
- تأخیر کوتاه امتیاز پذیرفتنی است.
- گم‌شدن دائمی و امتیاز تکراری پذیرفتنی نیست.
- Redemption، Tier، Expiration، Refund و Campaign خارج از v1 هستند.

### تمرین ۲ — Module یا Microservice

هر گروه یکی از سه تصمیم را انتخاب می‌کند:

1. ماژول جدید در Monolith
2. سرویس مستقل از ابتدا
3. ماژول داخلی با Extraction Point مشخص

تصمیم فقط وقتی معتبر است که حداقل سه شاهد از این فهرست داشته باشد: Rate of Change، Availability، Data Ownership، Scaling، Team Ownership، Security Boundary یا Reuse Channel.

### تمرین ۳ — Context Canvas

Canvas باید شامل Responsibility، Data، Inbound Contract، Outbound Contract، Dependency، Consistency Policy، Failure Policy و Non-goal باشد.

## تصمیم مرجع مدرس

- Loyalty از ابتدا سرویس مستقل است؛ این یک قانون عمومی برای قابلیت‌های جدید نیست.
- Loyalty مالک Account، Balance، Transaction و Processed Message است.
- هیچ سرویس دیگری به دیتابیس Loyalty متصل نمی‌شود.
- API عمومی v1 فقط query ارائه می‌دهد.
- تخصیص امتیاز بعداً از طریق Integration Event انجام خواهد شد.
- Unavailability سرویس Loyalty، payment را fail نمی‌کند.

## Exit Ticket جلسه اول

هر نفر در یک جمله پاسخ می‌دهد:

> کدام نیاز این سناریو، هزینه‌ی ساخت یک سرویس مستقل را توجیه می‌کند و چه هزینه‌ی جدیدی ایجاد می‌شود؟

---

# جلسه‌ی دوم — ارتباط، شکست جزئی و سازگاری نهایی

## سؤال محوری

Payments چگونه موفقیت پرداخت را به Loyalty اعلام کند، بدون اینکه Availability پرداخت را به Loyalty گره بزند؟

## خروجی جلسه

- Communication Decision Matrix
- قرارداد `PaymentSucceededV1`
- Consistency و Failure Policy
- ADR ارتباط غیرهم‌زمان و RabbitMQ

## Run Sheet — جلسه دوم

| دقیقه | مدت | فعالیت |
|---:|---:|---|
| 0 | 8 | مرور Exit Ticket و معماری فعلی |
| 8 | 12 | تمرین چهار گزینه‌ی ارتباطی |
| 20 | 16 | Sync، Async، Query، Command و Event |
| 36 | 10 | بازطراحی و انتخاب مدل ارتباط |
| 46 | 12 | طراحی `PaymentSucceededV1` |
| 58 | 8 | Demo Checkpoint 02: Happy Path |
| 66 | 5 | استراحت |
| 71 | 13 | Eventual Consistency و Delivery Semantics |
| 84 | 12 | Failure Demo: broker یا Loyalty خاموش |
| 96 | 12 | Timeout، Retry و Circuit Breaker |
| 108 | 7 | Service Discovery: local و production |
| 115 | 5 | جمع‌بندی و سؤال پل به جلسه سوم |

## تمرین ارتباط

چهار گزینه:

1. Payments به Loyalty درخواست HTTP بدهد.
2. Payments مستقیماً جدول Loyalty را تغییر دهد.
3. Payments یک Integration Event منتشر کند.
4. منطق Loyalty داخل Payments اجرا شود.

برای هر گزینه، گروه‌ها این موارد را می‌نویسند:

- Coupling ایجادشده
- رفتار در زمان failure
- مدل consistency
- داده‌ی موردنیاز
- اثری که روی response time پرداخت دارد

## تئوری ضروری

- Query برای دریافت اطلاعات و بدون intent تغییر
- Command برای درخواست انجام یک کار توسط گیرنده‌ی مشخص
- Event برای اعلام واقعیتی که رخ داده است
- Temporal Coupling در ارتباط هم‌زمان
- Partial Failure؛ ممکن است فقط یکی از اجزای جریان fail شود
- Eventual Consistency به‌عنوان تصمیم محصول و معماری
- At-most-once و At-least-once؛ Exactly-once business effect با idempotency
- Integration Event در برابر Domain Event
- قرارداد self-contained بدون Share کردن Domain Model

## قرارداد مرجع

`PaymentSucceededV1` شامل:

- `EventId`
- `OccurredAtUtc`
- `PaymentId`
- `OrderId`
- `CustomerId`
- `Amount`

چرا این داده‌ها؟ Loyalty نباید برای محاسبه‌ی امتیاز به دیتابیس Payments یا Ordering query بزند. چرا داده‌های بیشتر نه؟ Event نباید snapshot کامل Order یا Customer باشد.

## Demo اول — Happy Path

1. Order با `CustomerId` مشخص ساخته می‌شود.
2. Payment موفق می‌شود.
3. `PaymentSucceededV1` منتشر می‌شود.
4. Loyalty پیام را دریافت می‌کند.
5. Balance و history تغییر می‌کنند.

## Demo دوم — Failure Injection

1. RabbitMQ متوقف می‌شود.
2. Payment endpoint فراخوانی می‌شود.
3. Payment در دیتابیس ثبت شده، ولی publish انجام نشده است.
4. endpoint ممکن است خطا بدهد و retry کاربر خطر duplicate payment ایجاد کند.
5. RabbitMQ برمی‌گردد، اما message گمشده خودکار ساخته نمی‌شود.

سؤال پل:

> چگونه تغییر Payment و intent انتشار event را بدون distributed transaction اتمیک کنیم؟

## Resilience — سطح موردنیاز کارگاه

- Timeout: بودجه‌ی انتظار محدود برای call هم‌زمان
- Retry: فقط transient failure و فقط با توجه به idempotency
- Circuit Breaker: توقف موقت callهایی که به احتمال زیاد fail می‌شوند
- Backoff و Jitter: فقط معرفی، نه تنظیم عمیق
- Service Discovery: Docker DNS در demo؛ Consul/Kubernetes فقط مقایسه

## Exit Ticket جلسه دوم

هر گروه دو failure window را روی sequence جریان مشخص می‌کند: یکی در Producer و یکی در Consumer.

---

# جلسه‌ی سوم — پیام‌رسانی قابل‌اعتماد و مهاجرت مرحله‌ای

## سؤال محوری

چگونه هم پیام گم نشود، هم اثر آن دوباره اعمال نشود، و هم Payments بدون Big-bang Rewrite استخراج شود؟

## خروجی جلسه

- Transaction Boundary Diagram برای Outbox و Inbox
- Payment Extraction Plan
- Strangler Route Map
- Test Pyramid متناسب با سیستم توزیع‌شده
- Trade-off Ledger نهایی

## Run Sheet — جلسه سوم

| دقیقه | مدت | فعالیت |
|---:|---:|---|
| 0 | 10 | Lost Message و Duplicate Message |
| 10 | 15 | Dual Write و Transactional Outbox |
| 25 | 10 | Idempotent Consumer و Inbox |
| 35 | 10 | تمرین Transaction Boundary |
| 45 | 14 | Demo Checkpoint 03 و replay امن |
| 59 | 5 | استراحت |
| 64 | 10 | مسئله‌ی استخراج Payments |
| 74 | 14 | Strangler Fig و Branch by Abstraction |
| 88 | 14 | Demo Checkpoint 04 و 05 |
| 102 | 10 | Test Strategy و Observability |
| 112 | 6 | Trade-off Ledger |
| 118 | 2 | پیام پایانی |

## آزمایش آغاز جلسه

- Failure A: commit دیتابیس انجام شده، publish انجام نشده است.
- Failure B: publish انجام شده، اما producer پیش از علامت‌گذاری outbox crash کرده است.
- Failure C: consumer تغییر business را commit کرده، اما پیش از Ack crash کرده است.

شرکت‌کننده باید تشخیص دهد Outbox مشکل A را حل می‌کند، اما B را به duplicate تبدیل می‌کند؛ Idempotency اثر B و C را کنترل می‌کند.

## تمرین Transaction Boundary

گروه‌ها باید دو مرز تراکنش رسم کنند:

### Producer

- Payment
- Outbox Message
- یک Local Transaction

### Consumer

- Loyalty Transaction
- Balance
- Processed Message
- یک Local Transaction

پاسخ نامعتبر: یک تراکنش مشترک بین SQL Server و RabbitMQ یا بین دیتابیس Payments و Loyalty.

## Demo Checkpoint 03

1. Broker خاموش می‌شود.
2. Payment و Outbox با هم commit می‌شوند.
3. Broker روشن می‌شود.
4. Worker پیام pending را publish می‌کند.
5. همان event دوباره تحویل می‌شود.
6. Balance فقط یک بار تغییر می‌کند.

عبارت کلیدی مدرس:

> Outbox تحویل Exactly-once ایجاد نمی‌کند؛ intent را durable می‌کند و duplicate را به مسئله‌ای قابل مدیریت تبدیل می‌کند.

## مسئله‌ی استخراج Payments

Payments اکنون در Monolith است و از قرارداد in-process Ordering استفاده می‌کند. هدف این نیست که همه‌چیز را جدا کنیم؛ هدف حذف یک coupling مشخص و انتقال ownership با کمترین blast radius است.

پرسش گروهی:

- اولین seam استخراج کجاست؟
- کدام route باید جابه‌جا شود؟
- Payments برای اعتبارسنجی Order چه داده‌ای لازم دارد؟
- Order چه زمانی Paid می‌شود؟
- دیتابیس قبلی چگونه از dual ownership خارج می‌شود؟
- rollback مهاجرت چیست؟

## تصمیم مرجع استخراج

- یک Host مستقل برای Payments ساخته می‌شود.
- Payments مالک دیتابیس خود می‌شود.
- اطلاعات قابل پرداخت Order از HTTP contract داخلی Ordering خوانده می‌شود.
- این call دارای timeout و resilience محدود است.
- تغییر وضعیت Order با `PaymentSucceededV1` انجام می‌شود.
- Gateway فقط route پرداخت را به Host جدید می‌فرستد.
- Catalog، Ordering و AiSearch همچنان در Monolith باقی می‌مانند.
- rollback با بازگرداندن route ممکن است؛ data ownership باید پیش از cutover روشن باشد.

## Demo Checkpoint 04 و 05

- `SmartShop.Api`: Catalog، Ordering و AiSearch
- `SmartShop.Payments.Api`: Payments
- `SmartShop.Loyalty.Api`: Loyalty
- `SmartShop.Gateway`: یک public entry point
- RabbitMQ: fan-out رویداد به Ordering و Loyalty
- SQL Server: سه database منطقی با owner مشخص در محیط کارگاه

ابتدا route قدیمی نشان داده می‌شود، سپس route Payments در YARP تغییر می‌کند. client URL خود را عوض نمی‌کند.

## Test Strategy

| نوع تست | چه چیزی را اثبات می‌کند | چه چیزی را اثبات نمی‌کند |
|---|---|---|
| Unit | قواعد امتیاز و Aggregate | wiring و persistence |
| Architecture | dependency rule و مرز پروژه‌ها | رفتار runtime |
| Integration | EF، migration، outbox، inbox و adapter | کل journey کاربر |
| Contract | سازگاری HTTP و event schema | availability واقعی |
| End-to-End | یک مسیر حیاتی | همه edge caseها |

## Observability Minimum

- `CorrelationId` از request تا message
- Structured Log با `OrderId`، `PaymentId` و `EventId`
- readiness برای database و broker
- liveness بدون dependency خارجی
- metric برای outbox backlog و consumer failure
- trace توزیع‌شده فقط در حد مشاهده‌ی مسیر، نه آموزش کامل OpenTelemetry

## Trade-off Ledger پایانی

در برابر استقلال Loyalty و Payments این هزینه‌ها اضافه شدند:

- network contract
- latency و partial failure
- broker و topology
- eventual consistency
- outbox، inbox و cleanup
- gateway
- deployment و configuration بیشتر
- observability و on-call پیچیده‌تر
- test suite چندلایه‌تر

پرسش پایانی:

> اگر نیاز استقلال تغییر و availability حذف شود، آیا باز هم این هزینه‌ها توجیه دارند؟

---

# اولویت محتوایی مدرس

## Must Teach

- Microservice به‌عنوان Business Capability
- مرز سرویس و مالکیت داده
- Sync در برابر Async با Failure Mode
- Partial Failure و Eventual Consistency
- Integration Event نسخه‌دار
- Dual Write، Outbox و Idempotency
- Strangler Fig و مهاجرت route-by-route
- حداقل Test Strategy و Observability

## Mention Only

- Consul و Kubernetes Service Discovery
- Dead-letter Queue و poison message policy
- Saga و Process Manager
- gRPC در برابر HTTP/JSON
- API Composition
- Schema Registry
- OpenTelemetry details
- CI/CD مستقل و deployment strategies

## Omit From This Workshop

- پیاده‌سازی Kubernetes
- Service Mesh
- Event Sourcing
- CQRS عمیق
- Distributed Transaction و 2PC
- پیاده‌سازی کامل Authentication/Authorization
- Multi-region و geo-replication
- Autoscaling و capacity planning
- Dapr یا abstraction frameworkهای مشابه

دلیل حذف: هرکدام مسیر آموزشی مستقلی می‌سازند و تمرکز کارگاه را از تصمیم معماری و failure handling می‌گیرند.

---

# جمله‌های کلیدی مدرس

- «Microservice یک deployment unit با مالکیت روشن است، نه یک پوشه‌ی کوچک‌تر.»
- «مرز خوب، چیزهایی را کنار هم می‌گذارد که با هم تغییر می‌کنند.»
- «Database-per-Service درباره‌ی ownership است، نه الزاماً یک server فیزیکی جدا.»
- «Async coupling را حذف نمی‌کند؛ شکل و زمان آن را تغییر می‌دهد.»
- «Eventual Consistency باید برای محصول قابل توضیح و قابل مشاهده باشد.»
- «Retry بدون idempotency می‌تواند خطا را تکثیر کند.»
- «Broker جای Transaction Boundary را تعیین نمی‌کند.»
- «Outbox پیام را دقیقاً یک بار تحویل نمی‌دهد؛ intent انتشار را از دست‌رفتن‌ناپذیر می‌کند.»
- «Strangler یک مهاجرت کنترل‌شده است، نه اسم دیگری برای rewrite.»
- «استقلال وقتی ارزشمند است که هزینه‌ی عملیات توزیع‌شده را توجیه کند.»

# ضدالگوهای کلامی

مدرس نباید بگوید:

- «هر ماژول خوب باید Microservice شود.»
- «Microservices همیشه scalableتر است.»
- «Docker داشتن یعنی Microservices بودن.»
- «Async همیشه بهتر از Sync است.»
- «RabbitMQ سرویس‌ها را decouple می‌کند.» بدون توضیح نوع coupling باقی‌مانده
- «Eventual Consistency یعنی داده بالاخره حتماً درست می‌شود.»
- «Queue یعنی Exactly-once.»
- «Retry مشکل availability را حل می‌کند.»
- «Gateway همان Service Discovery است.»
- «اول سرویس‌ها را جدا می‌کنیم و بعداً observability اضافه می‌کنیم.»

---

# معیار آمادگی برای تولید اسلاید

Storyboard فقط زمانی تولید می‌شود که این موارد ثابت باشند:

- Product Brief و non-goalها
- خروجی قابل سنجش هر جلسه
- Run Sheet مجموعاً ۱۲۰ دقیقه برای هر جلسه
- تمرین و پاسخ مرجع هر بخش
- ترتیب Checkpointهای کد
- Must Teach، Mention Only و Omit
- Failure Demo و reset path هر جلسه

این سند منبع اصلی storyboard، speaker notes و تطبیق checkpointهای Repository خواهد بود.
