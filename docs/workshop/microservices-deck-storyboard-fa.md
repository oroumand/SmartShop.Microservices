# Storyboard اسلایدهای کارگاه Microservices

## مشخصات Deck

- یک فایل PPTX با ۷۲ اسلاید و سه Section مستقل
- نسبت تصویر 16:9 و ابعاد 1280×720
- فارسی RTL؛ کد، endpoint و نام contractها LTR
- عنوان اصلی: IranYekanX ExtraBold یا ExtraBlack
- عنوان اسلاید: IranYekanX Bold
- زیرعنوان: IranYekanX Medium
- متن: IranYekanX Regular
- هویت بصری: زمینه‌ی سفید گرم، متن سرمه‌ای تیره، Accent فیروزه‌ای، هشدار مرجانی و نکته‌ی زرد
- هر اسلاید یک پیام اصلی؛ توضیحات کامل در Speaker Notes
- Diagramها با shapeهای قابل ویرایش؛ Screenshot فقط برای UI یا خروجی runtime

## سیستم بصری

| نقش | رنگ پیشنهادی |
|---|---|
| Background | `#F7FAF9` |
| Primary text | `#102A43` |
| Secondary text | `#52667A` |
| Microservice / decision | `#0EA5A4` |
| Healthy / accepted | `#23B26D` |
| Failure / coupling | `#EF6351` |
| Exercise / question | `#F4B942` |
| Surface | `#FFFFFF` |
| Hairline | `#D9E4E8` |

الگوهای تکرارشونده:

- **Problem:** یک سؤال بزرگ و یک نشانه‌ی هشدار
- **Exercise:** نوار زرد، زمان‌سنج و Deliverable واضح
- **Theory:** یک مفهوم، یک Diagram، حداکثر سه گزاره
- **Decision:** Context / Decision / Consequences
- **Demo:** Goal / Command / Expected Signal / Reset
- **Checkpoint:** شماره tag و diff موردنظر

---

# اسلایدهای مشترک — ۱ تا ۵

## 1 — عنوان کارگاه

- پیام: «از Modular Monolith تا Microservices؛ با تصمیم، شکست و مهاجرت واقعی»
- بصری: سه deployment unit در حال جداشدن از یک هسته، بدون نمایش جزئیات فنی
- Note: معرفی کوتاه مدرس و ارتباط مستقیم با کارگاه قبلی

## 2 — چیزی که امروز نمی‌سازیم

- پیام: هدف، شکستن Monolith به تعداد زیادی سرویس نیست
- بصری: دو تصویر متقابل؛ «Distributed Monolith» در برابر «Intentional Services»
- Note: Microservices هدف نیست؛ ابزار استقلال است

## 3 — چیزی که در پایان خواهیم داشت

- پیام: SmartShop.Api + Payments + Loyalty + Gateway + RabbitMQ
- بصری: معماری نهایی با پنج جزء اصلی و سه database ownership zone
- Note: فقط نتیجه را preview کنید؛ جزئیات را توضیح ندهید

## 4 — روش کارگاه

- پیام: مسئله → تمرین → تئوری → بازطراحی → تصمیم → کد
- بصری: چرخه‌ی شش‌مرحله‌ای
- Interaction: توافق با کلاس که پاسخ مدرس قبل از تمرین نشان داده نمی‌شود

## 5 — نقشه‌ی سه جلسه

- پیام: Boundary / Communication / Reliability & Migration
- بصری: سه ستون با سؤال محوری هر جلسه
- Note: زمان استراحت و شکل مشارکت را توضیح دهید

---

# جلسه‌ی اول — اسلایدهای ۶ تا ۲۷

## 6 — کاور جلسه اول

- عنوان: «یک Capability جدید؛ Module یا Microservice؟»
- بصری: عدد بزرگ 01 و Loyalty به‌عنوان کارت جداشونده

## 7 — SmartShop در نقطه‌ی شروع

- پیام: چهار ماژول، یک process، یک deployment
- بصری: Container diagram ساده‌ی Modular Monolith فعلی
- Note: مزایای معماری قبلی را به رسمیت بشناسید

## 8 — درخواست جدید محصول

- پیام: «بعد از پرداخت موفق، مشتری امتیاز بگیرد»
- بصری: Order receipt → point token
- Note: هنوز هیچ راه‌حل فنی ارائه نشود

## 9 — Product Brief و Constraints

- محتوا: نرخ بازگشت، یک امتیاز به ازای ۱۰۰ واحد، تغییر مستقل قواعد، failure نباید payment را rollback کند
- بصری: چهار کارت requirement

## 10 — تمرین: اول سؤال، بعد طراحی

- زمان: ۱۰ دقیقه
- Deliverable: پنج سؤال و سه فرض
- بصری: Exercise card با countdown و سه prompt

## 11 — ابهام‌های تعیین‌کننده

- پیام: Trigger، Customer identity، consistency و scope
- بصری: چهار sticky-note که پاسخ‌های کلاس روی آن جمع می‌شوند
- Note: پاسخ مرجع را مرحله‌ای reveal کنید

## 12 — Module با Microservice برابر نیست

- پیام: مرز منطقی، مرز runtime و مرز ownership سه چیز متفاوت‌اند
- بصری: جدول سه‌ردیفی Module / Process / Service

## 13 — Microservice یعنی Business Capability

- پیام: سرویس حول کاری شکل می‌گیرد که کسب‌وکار انجام می‌دهد
- بصری: Capability map کوچک با Loyalty برجسته
- Note: اندازه‌ی کد را معیار ندانید

## 14 — استقلال رایگان نیست

- پیام: شبکه، deployment، observability، security، on-call و testing اضافه می‌شوند
- بصری: ترازوی «Independence» و «Distributed-system Cost»

## 15 — Architecture Characteristics

- پیام: برای Loyalty چهار ویژگی مهم‌اند: Change Independence، Availability، Explainable Consistency و Operability
- بصری: Radar یا ranked bars؛ نه امتیازدهی علمی، بلکه اولویت نسبی

## 16 — تمرین: Module یا Microservice؟

- زمان: ۱۰ دقیقه
- انتخاب‌ها: Module / Microservice / Module with Extraction Point
- Deliverable: تصمیم + سه شاهد

## 17 — شواهدی که تصمیم را معتبر می‌کنند

- محتوا: Rate of Change، Availability، Data Ownership، Team Ownership، Scaling، Reuse Channel
- بصری: Decision lens؛ معیارهای قوی و ضعیف

## 18 — چهار نوع Coupling

- محتوا: Temporal، Data، Contract و Organizational
- بصری: چهار کارت با یک مثال SmartShop برای هرکدام

## 19 — سرنخ‌های Boundary

- پیام: چیزهایی را کنار هم بگذار که با هم تغییر می‌کنند
- بصری: change-frequency heatmap ساده برای Ordering / Payments / Loyalty

## 20 — چه کسی مالک کدام داده است؟

- پیام: Ownership با Read Access فرق دارد
- بصری: Data Ownership Matrix با Order، Payment، LoyaltyAccount و CustomerId

## 21 — Database-per-Service دقیقاً یعنی چه؟

- پیام: مالکیت مستقل schema و migration؛ نه الزاماً server فیزیکی جدا
- بصری: یک SQL Server با سه database منطقی و ownerهای جدا
- Note: cross-service join و transaction مستقیم ممنوع

## 22 — تمرین: Context Canvas

- زمان: ۱۱ دقیقه
- Deliverable: Responsibility، Data، Inbound، Outbound، Dependency، Consistency، Failure، Non-goal
- بصری: Canvas خالی قابل تکمیل

## 23 — Context Canvas مرجع Loyalty

- پیام: Earn و Query داخل مرز؛ Order و Payment بیرون مرز
- بصری: Canvas تکمیل‌شده با رنگ ownership

## 24 — تصمیم را ثبت می‌کنیم

- محتوا: Context / Decision / Consequences برای ADR-0013 و ADR-0014
- بصری: ADR card با یک consequence مثبت و دو هزینه

## 25 — Public Contract نسخه اول

- محتوا: دو GET endpoint برای balance و history
- بصری: API contract card با response خلاصه
- Note: endpoint کسب امتیاز عمومی نداریم

## 26 — Checkpoint 01: مرز runtime را ببین

- محتوا: process، port، image، configuration، health و database مستقل
- بصری: checklist کنار diff tree پروژه Loyalty
- Demo: اجرای health و balance صفر

## 27 — Exit Ticket جلسه اول

- سؤال: «کدام نیاز، هزینه‌ی سرویس مستقل را توجیه می‌کند و چه هزینه‌ای اضافه شد؟»
- بصری: یک کارت سؤال؛ بدون پاسخ روی اسلاید

---

# جلسه‌ی دوم — اسلایدهای ۲۸ تا ۴۹

## 28 — کاور جلسه دوم

- عنوان: «وقتی شبکه وارد معماری می‌شود»
- بصری: عدد 02 و یک connection ناپایدار بین Payments و Loyalty

## 29 — مسئله‌ی امروز

- سؤال: اگر Payment موفق شود و Loyalty خاموش باشد چه باید شود؟
- بصری: payment سبز، loyalty قرمز، response نامعلوم

## 30 — تمرین: چهار راه ارتباط

- گزینه‌ها: HTTP مستقیم، دیتابیس مشترک، Integration Event، منطق داخل Payments
- Deliverable: coupling، failure، consistency و latency هر گزینه

## 31 — مقایسه‌ی چهار گزینه

- بصری: Decision Matrix؛ سطرها availability/coupling/consistency/complexity
- پیام: هیچ گزینه‌ای بدون هزینه نیست

## 32 — Query، Command و Event

- بصری: سه کارت با فعل/زمان/مالک تصمیم
- مثال‌ها: GetOrderPaymentInfo / ChargeOrder / PaymentSucceeded

## 33 — ارتباط Sync چه زمانی درست است؟

- پیام: caller برای ادامه، پاسخ همین حالا را لازم دارد
- بصری: request/response sequence با timeout budget

## 34 — ارتباط Async چه زمانی درست است؟

- پیام: producer واقعیتی را اعلام می‌کند و منتظر واکنش مصرف‌کننده نیست
- بصری: event fan-out به Ordering و Loyalty

## 35 — Temporal Coupling را ببین

- بصری: دو timeline؛ Sync نیازمند هم‌زمانی، Async دارای queue buffer
- Note: Async coupling را حذف نمی‌کند

## 36 — تمرین: Event را طراحی کنید

- زمان: ۱۰ دقیقه
- Deliverable: نام، version، fields و معنای رخداد
- Constraint: مصرف‌کننده حق query مستقیم دیتابیس producer را ندارد

## 37 — `PaymentSucceededV1`

- محتوا: EventId، OccurredAtUtc، PaymentId، OrderId، CustomerId، Amount
- بصری: JSON envelope قابل خواندن

## 38 — قرارداد Event خوب چه ویژگی دارد؟

- محتوا: past tense، immutable fact، consumer-oriented data، explicit version، no domain model sharing
- بصری: پنج checkmark دور contract

## 39 — Checkpoint 02: معماری انتشار مستقیم

- بصری: Payment DB commit → Publish → Loyalty consume
- هشدار: فاصله‌ی قرمز میان commit و publish

## 40 — Demo: Happy Path

- Goal: payment تا loyalty balance
- Expected Signal: Payment 201، event در broker، balance افزایش‌یافته
- Reset: customer ثابت و database clean

## 41 — Eventual Consistency یک Timeline است

- بصری: T0 Payment succeeded، T1 response، T2 event، T3 balance updated
- پیام: inconsistency window باید برای محصول پذیرفتنی باشد

## 42 — Delivery Guarantee با Business Effect فرق دارد

- بصری: At-most-once / At-least-once / Exactly-once-effect comparison
- Note: exactly-once delivery را وعده ندهید

## 43 — Demo: Failure Injection

- Action: RabbitMQ را خاموش کن و پرداخت بزن
- Observe: Payment ذخیره، publish fail، event بازسازی نمی‌شود
- بصری: چهار مرحله با Expected/Actual

## 44 — پنجره‌ی Lost Message

- بصری: sequence diagram با crash در فاصله‌ی DB commit و broker publish
- سؤال: transaction واقعی کجاست؟

## 45 — Timeout: بودجه‌ی انتظار

- پیام: timeout از latency budget می‌آید، نه از حدس
- بصری: 800ms budget تقسیم‌شده میان gateway، service و dependency

## 46 — Retry می‌تواند خطا را تکثیر کند

- پیام: transient + bounded + backoff + idempotent
- بصری: safe retry checklist و duplicate payment warning

## 47 — Circuit Breaker جلوی شکست آبشاری را می‌گیرد

- بصری: Closed → Open → Half-open state diagram
- Note: breaker داده را repair نمی‌کند

## 48 — Service Discovery در این کارگاه

- محتوا: Docker DNS در local؛ Kubernetes Service یا Consul در production context
- بصری: جدول «نام سرویس را چه کسی resolve می‌کند؟»

## 49 — سؤال پل به جلسه سوم

- سؤال بزرگ: «چگونه state و intent انتشار را اتمیک کنیم، بدون 2PC؟»
- بصری: Payment + Outbox به‌صورت جای خالی

---

# جلسه‌ی سوم — اسلایدهای ۵۰ تا ۷۲

## 50 — کاور جلسه سوم

- عنوان: «Reliability و Migration بدون Big Bang»
- بصری: عدد 03، یک پیام تکراری و یک route در حال جابه‌جایی

## 51 — سه Failure Window

- A: commit بدون publish
- B: publish بدون علامت‌گذاری outbox
- C: consumer commit بدون ack
- بصری: سه mini-sequence کنار هم

## 52 — مسئله‌ی Dual Write

- پیام: دو resource مستقل، یک transaction محلی ندارند
- بصری: Database و Broker با شکاف قرمز

## 53 — Transactional Outbox

- پیام: state و publish intent را در یک database transaction ذخیره کن
- بصری: Payment + OutboxMessage داخل یک boundary

## 54 — سمت Producer دقیقاً چه اتمیک است؟

- بصری: Local transaction، poller، publish، mark processed
- Note: crash بعد از publish هنوز duplicate می‌سازد

## 55 — چرا Consumer دوباره پیام را می‌بیند؟

- بصری: commit → crash → no ack → redelivery
- پیام: redelivery رفتار عادی At-least-once است

## 56 — Idempotent Consumer و Inbox

- پیام: business effect و ProcessedMessage در یک transaction
- بصری: LoyaltyTransaction + Balance + Inbox boundary

## 57 — تمرین: Transaction Boundary را رسم کنید

- زمان: ۱۰ دقیقه
- Deliverable: دو boundary مستقل Producer و Consumer
- Anti-answer: transaction مشترک SQL + RabbitMQ

## 58 — Demo Checkpoint 03

- Action: broker down → payment → broker up → replay duplicate
- Expected: outbox pending سپس processed؛ balance فقط یک بار
- Reset: script مشخص

## 59 — Outbox چه چیزی را حل نمی‌کند؟

- محتوا: ordering، poison message، retention، multi-worker locking، schema evolution
- بصری: «Solved / Still yours»

## 60 — حالا Payments را استخراج کنیم

- مسئله: module موجود به Ordering contract و shared database متصل است
- بصری: coupling map قبل از extraction

## 61 — تمرین: اولین Seam کجاست؟

- زمان: ۱۰ دقیقه
- Deliverable: route، contract، data owner، rollback و cutover signal

## 62 — Strangler Fig

- پیام: route-by-route replacement با rollback کوچک
- بصری: gateway که بخشی از traffic را به service جدید می‌فرستد

## 63 — Route Map قبل و بعد

- قبل: همه routeها به Monolith
- بعد: `/api/payments/**` به Payments، `/api/loyalty/**` به Loyalty، بقیه به Monolith
- بصری: جدول mapping دقیق

## 64 — یک Sync Contract هنوز لازم است

- پیام: Payments برای تصمیم charge، OrderPaymentInfo را همین حالا لازم دارد
- بصری: Payments → internal Ordering HTTP با timeout/retry/breaker
- Note: retry روی operation خواندنی امن‌تر است

## 65 — نتیجه‌ی Payment با Event پخش می‌شود

- پیام: Ordering و Loyalty مستقل `PaymentSucceededV1` را مصرف می‌کنند
- بصری: fan-out و database ownerهای مجزا

## 66 — Demo Checkpoint 04 و 05

- Goal: یک URL عمومی، سه backend
- Observe: route destination، payment database، eventual order/loyalty update
- Reset/Rollback: route Payments به Monolith برگردد

## 67 — Data Cutover و Rollback

- محتوا: backfill، ownership freeze، cutover، verify، rollback window
- بصری: migration timeline پنج‌مرحله‌ای
- Note: dual ownership طولانی ممنوع

## 68 — Test Strategy

- بصری: پنج لایه Unit / Architecture / Integration / Contract / E2E
- پیام: هر تست سؤال متفاوتی را جواب می‌دهد

## 69 — Observability Minimum

- محتوا: CorrelationId، structured logs، outbox backlog metric، consumer failures، trace
- بصری: یک trace از Gateway تا Payment و دو consumer

## 70 — Liveness با Readiness یکی نیست

- بصری: جدول dependency-aware health checks
- پیام: liveness نباید با outage یک dependency process را restart-loop کند

## 71 — Trade-off Ledger

- بصری: دو ستون Independence Gained / Complexity Added
- Interaction: کلاس سه مورد به هر ستون اضافه می‌کند

## 72 — مدل ذهنی پایانی

- پیام اصلی: «مرز و ownership را پیدا کن؛ failure را طراحی کن؛ سپس deploy را جدا کن.»
- بصری: سه گام Boundary → Failure Model → Independent Deployment
- Footer: Repository و checkpoint tags

---

# قواعد Speaker Notes

برای هر اسلاید Notes باید این ساختار را داشته باشد:

1. **هدف:** شرکت‌کننده پس از اسلاید چه چیزی را بفهمد؟
2. **متن پیشنهادی:** ۴۵ تا ۱۸۰ ثانیه گفتار طبیعی مدرس
3. **سؤال:** یک سؤال برای فعال‌کردن کلاس، در صورت نیاز
4. **Reveal:** ترتیب نمایش اجزای اسلاید
5. **Transition:** یک جمله برای وصل‌شدن به اسلاید بعد
6. **Do not say:** فقط در اسلایدهای مستعد سوءبرداشت
7. **Source:** لینک منبع معتبر در صورت استفاده از ادعای بیرونی

# قواعد Demo Slide

هر Demo فقط چهار چیز نشان می‌دهد:

- Goal
- Action یا Command
- Expected Signal
- Reset / Recovery

جزئیات طولانی command و troubleshooting در Runbook قرار می‌گیرد، نه روی اسلاید.

# معیار شروع تولید PPTX

- تعداد اسلایدها: ۷۲
- تطبیق شماره اسلاید با سه Run Sheet انجام شده است
- هر تمرین Deliverable دارد
- هر failure demo دارای Expected Signal است
- محتوای Mention Only به اسلایدهای عمیق تبدیل نشده است
- اسلاید ۳ معماری نهایی را فقط preview می‌کند و اسلایدهای ۶۳ تا ۶۷ آن را توضیح می‌دهند
