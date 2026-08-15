# راهنمای مدرس و متن پیشنهادی کارگاه Microservices

این سند برای اجرای سه جلسه‌ی دو ساعته نوشته شده است و با فایل `SmartShop-Microservices-Workshop-FA.pptx` و شماره‌ی اسلایدهای آن تطابق دارد. متن‌ها قرار نیست کلمه‌به‌کلمه حفظ شوند؛ ترتیب استدلال، سؤال‌ها، مکث‌ها و جمله‌های کلیدی باید حفظ شوند.

## قرارداد اجرای کارگاه

- پاسخ مرجع را پیش از ثبت پاسخ اولیه‌ی گروه‌ها نشان ندهید.
- هر واژه‌ی فنی باید پاسخ یک مسئله‌ی دیده‌شده باشد.
- در هر Demo ابتدا Goal و Expected Signal را بگویید، سپس کد یا ترمینال را باز کنید.
- اگر Demo شکست خورد، مسئله را با diagram و log ثبت‌شده ادامه دهید؛ زمان کلاس را صرف debugging زنده نکنید.
- در پایان هر تصمیم، یک Gain و حداقل یک Cost را روی تخته نگه دارید.
- عبارت «بهترین راه» را با «تصمیم مناسب این constraintها» جایگزین کنید.

---

# جلسه‌ی اول — Boundary و Ownership

## خروجی روی تخته

تا پایان جلسه باید این چهار artifact دیده شوند:

1. Context Canvas سرویس Loyalty
2. Data Ownership Matrix
3. ADR ایجاد Loyalty به‌صورت سرویس مستقل
4. قرارداد query برای Balance و History

## اسلاید ۱ — عنوان کارگاه

**متن پیشنهادی**

«این کارگاه ادامه‌ی همان داستان SmartShop است. در کارگاه قبل یاد گرفتیم داخل یک process هم می‌توانیم مرز، مالکیت و قرارداد داشته باشیم. امروز از همان نقطه‌ی سالم شروع می‌کنیم و فقط جایی deployment را جدا می‌کنیم که یک نیاز واقعی هزینه‌ی آن را توجیه کند. بنابراین موضوع اصلی امروز Docker یا RabbitMQ نیست؛ موضوع، تصمیم معماری در حضور failure است.»

**Transition:** «قبل از اینکه بگوییم چه می‌سازیم، یک سوءبرداشت رایج را حذف کنیم.»

## اسلاید ۲ — چیزی که نمی‌سازیم

**متن پیشنهادی**

«اگر پروژه‌ها را جدا کنیم اما برای هر تغییر مجبور باشیم همه را با هم release کنیم، اگر schema مشترک داشته باشیم یا یک outage کوچک کل جریان را متوقف کند، فقط یک Distributed Monolith ساخته‌ایم. استقلال باید قابل مشاهده باشد: چه کسی مالک داده است؟ چه چیزی جدا deploy می‌شود؟ کدام failure به کجا سرایت می‌کند؟»

**سؤال کلاس:** «شما در پروژه‌های واقعی کدام نشانه‌ی Distributed Monolith را دیده‌اید؟»

**نکته‌ی تسهیل‌گری:** دو پاسخ بگیرید و بحث را باز نکنید؛ این سؤال فقط دانش موجود کلاس را فعال می‌کند.

## اسلاید ۳ — معماری پایان کار

**متن پیشنهادی**

«در انتها client یک ورودی عمومی دارد. Catalog، Ordering و AI Search فعلاً در SmartShop.Api می‌مانند. Payments و Loyalty process و data owner مستقل دارند. RabbitMQ واقعیت موفقیت پرداخت را پخش می‌کند. الان این تصویر را توضیح نمی‌دهم؛ می‌خواهم در پایان بتوانیم برای هر فلش دلیل و Failure Policy ارائه کنیم.»

**Transition:** «برای رسیدن به این تصویر، یک ریتم ثابت داریم.»

## اسلاید ۴ — روش کارگاه

**متن پیشنهادی**

«در هر بخش ابتدا مسئله را می‌بینید. چند دقیقه با دانش فعلی پاسخ می‌دهید. بعد فقط تئوری لازم برای همان مسئله را می‌گیریم، طراحی را اصلاح می‌کنیم، تصمیم را ثبت می‌کنیم و در کد یا failure demo نتیجه را می‌بینیم. جواب من قبل از تمرین نشان داده نمی‌شود، چون هدف حفظ الگو نیست؛ هدف ساختن استدلال است.»

**توافق با کلاس:** پاسخ گروه باید کوتاه، قابل نقد و همراه حداقل یک فرض باشد.

## اسلاید ۵ — نقشه‌ی سه جلسه

**متن پیشنهادی**

«جلسه‌ی اول درباره‌ی Boundary است: آیا Loyalty اصلاً سرویس شود؟ جلسه‌ی دوم درباره‌ی ارتباط و شکست شبکه است. جلسه‌ی سوم پیام‌رسانی قابل اعتماد و مهاجرت Payments را پوشش می‌دهد. هر جلسه ۱۱۵ دقیقه کار و پنج دقیقه استراحت دارد.»

**Transition:** «بیایید از وضعیت موجود شروع کنیم، نه از معماری ایده‌آل.»

## اسلاید ۶ — کاور جلسه اول

**متن پیشنهادی**

«سؤال این جلسه عمداً دو جواب معتبر دارد. Loyalty می‌تواند Module باشد و می‌تواند Service باشد. پاسخ درست را نه اندازه‌ی کد، بلکه constraintهای کسب‌وکار و عملیات تعیین می‌کنند.»

## اسلاید ۷ — نقطه‌ی شروع SmartShop

**متن پیشنهادی**

«این Modular Monolith مشکل ما نیست؛ نقطه‌ی شروع خوب ماست. چهار ماژول، یک process و یک deployment داریم. قراردادهای داخلی و ownership تا حد خوبی روشن‌اند. استخراج خوب از یک monolith مرزبندی‌شده بسیار کم‌خطرتر از استخراج از کد درهم است.»

**Do not say:** «Monolith بد است.»

## اسلاید ۸ — درخواست محصول

**متن پیشنهادی**

«تنها چیزی که محصول گفته این است: بعد از پرداخت موفق، مشتری امتیاز بگیرد. هنوز نمی‌دانیم service می‌خواهیم، event می‌خواهیم یا حتی جدول جدید. پنج دقیقه آینده را صرف راه‌حل نکنید؛ دنبال سؤال‌هایی بگردید که جوابشان معماری را تغییر می‌دهد.»

## اسلاید ۹ — Product Brief

**متن پیشنهادی**

«قاعده‌ی اولیه ساده است: به ازای هر صد واحد پرداخت، یک امتیاز کامل. اما سه constraint مهم‌تر از خود فرمول‌اند: قواعد Loyalty مستقل تغییر می‌کنند؛ outage آن نباید Payment موفق را rollback کند؛ و هر Earn باید قابل توضیح باشد. این‌ها بعداً انتخاب مرز، event و storage را شکل می‌دهند.»

**سؤال:** «کدام constraint از نظر شما بیشترین اثر معماری را دارد؟ چرا؟»

## اسلاید ۱۰ — تمرین Requirement Questions

**دستور اجرا**

«در گروه‌های دو یا سه نفره، حداکثر پنج سؤال و سه فرض بنویسید. سؤال خوب چیزی است که اگر پاسخ آن عوض شود، طراحی هم عوض شود. درباره‌ی ابزار سؤال نپرسید.»

**در دقیقه‌ی ۶ یادآوری کنید:** «Trigger، identity، consistency و scope را چک کنید.»

**خروجی:** عکس یا متن کوتاه هر گروه؛ ارائه‌ی شفاهی طولانی ممنوع.

## اسلاید ۱۱ — ابهام‌های تعیین‌کننده

**متن پیشنهادی**

«پاسخ مرجع ما این است: Trigger، PaymentSucceeded است؛ CustomerId شناسه‌ی پایدار است؛ چند ثانیه eventual consistency پذیرفتنی است؛ نسخه‌ی اول فقط Earn دارد. Refund، expiration، tier و campaign خارج از scope هستند. دقت کنید که non-goal کم‌اهمیت نیست؛ جلوی رشد بی‌قاعده‌ی سرویس را می‌گیرد.»

## اسلاید ۱۲ — Module و Microservice

**متن پیشنهادی**

«Module یک مرز منطقی است. Process یک مرز runtime است. Service علاوه بر این دو، مالکیت و مسئولیت عملیاتی دارد. اگر فقط یک پروژه‌ی جدا در solution بسازیم، هنوز service نداریم. اگر process جدا باشد اما data ownership مشترک بماند، استقلال ما ناقص است.»

**جمله‌ی کلیدی:** «Microservice یک deployment unit با مالکیت روشن است، نه یک پوشه‌ی کوچک‌تر.»

## اسلاید ۱۳ — Business Capability

**متن پیشنهادی**

«مرز را حول کاری می‌کشیم که کسب‌وکار انجام می‌دهد. Loyalty مسئول Earn، Balance و History است. تعداد endpoint یا class معیار اندازه‌ی سرویس نیست. اگر این سه رفتار با یک زبان، یک owner و یک cadence تغییر می‌کنند، cohesion خوبی دارند.»

## اسلاید ۱۴ — هزینه‌ی استقلال

**متن پیشنهادی**

«سمت چپ چیزهایی است که می‌خریم: release مستقل، containment شکست و autonomy تیم. سمت راست هزینه‌ی خرید است: network، security، observability، deployment، on-call و تست پیچیده‌تر. از این لحظه هر بار واژه‌ی microservice را به کار می‌بریم، باید هر دو کفه را هم‌زمان ببینیم.»

## اسلاید ۱۵ — Architecture Characteristics

**متن پیشنهادی**

«در این سناریو، Change Independence مهم‌ترین driver است. Availability مهم است چون Payment نباید به Loyalty گره بخورد. Consistency باید برای محصول قابل توضیح باشد. Operability نیز شرط ورود به production است. اعداد روی اسلاید اندازه‌گیری نیستند؛ فقط اولویت نسبی بحث‌اند.»

**سؤال:** «اگر Change Independence را حذف کنیم، آیا هنوز service مستقل را انتخاب می‌کنید؟»

## اسلاید ۱۶ — تمرین Module یا Microservice

**دستور اجرا**

«یکی از سه گزینه را انتخاب کنید: Module، Microservice یا Module با Extraction Point. تصمیم بدون سه شاهد پذیرفته نیست. شواهد باید به نیاز یا عملیات وصل باشند، نه به علاقه‌ی فنی.»

**در جمع‌بندی:** از یک گروه طرفدار Module و یک گروه طرفدار Service بخواهید هر کدام ۶۰ ثانیه دفاع کنند.

## اسلاید ۱۷ — Lens تصمیم

**متن پیشنهادی**

«Rate of Change، Availability و Data Ownership شواهد قوی این سناریو هستند. Team Ownership، Scale Profile و Reuse Channel می‌توانند تصمیم را تقویت کنند. تعداد class، مد بازار یا علاقه به broker شاهد معماری نیستند.»

## اسلاید ۱۸ — چهار نوع Coupling

**متن پیشنهادی**

«Coupling فقط call هم‌زمان نیست. Temporal یعنی دو طرف هم‌زمان آماده باشند. Data یعنی schema یا model مشترک. Contract یعنی تغییر قرارداد دیگری را می‌شکند. Organizational یعنی برای هر تغییر مجبور به هماهنگی چند تیمیم. Async فقط نوع اول را کم می‌کند؛ سه نوع دیگر هنوز نیازمند طراحی‌اند.»

## اسلاید ۱۹ — سرنخ Boundary

**متن پیشنهادی**

«قاعده‌ی کاربردی: چیزهایی را کنار هم بگذار که با هم تغییر می‌کنند. قواعد سفارش بیشتر با Ordering تغییر می‌کنند، قواعد درگاه با Payments و کمپین و امتیاز با Loyalty. این heatmap حقیقت علمی نیست؛ ابزار آشکارکردن change coupling است.»

## اسلاید ۲۰ — Data Ownership Matrix

**متن پیشنهادی**

«Ordering مالک Order است، Payments مالک Payment و Loyalty مالک LoyaltyAccount. CustomerId یک شناسه‌ی مشترک است، نه یک موجودیت مشترک قابل‌ویرایش. Reference داشتن به معنی مالک بودن نیست. سرویس مصرف‌کننده حق ندارد برای راحتی به جدول producer join بزند.»

## اسلاید ۲۱ — Database-per-Service

**متن پیشنهادی**

«Database-per-Service قبل از اینکه topology زیرساخت باشد، قانون ownership است. ممکن است در محیط کارگاه هر سه database روی یک SQL Server باشند. مهم این است که schema، migration و write access صاحب مشخص دارند. Shared server ممکن است؛ Shared ownership نه.»

**Do not say:** «برای هر سرویس حتماً server فیزیکی جدا لازم است.»

## اسلاید ۲۲ — تمرین Context Canvas

**دستور اجرا**

«هشت خانه را کامل کنید: Responsibility، Data، Inbound، Outbound، Dependency، Consistency، Failure و Non-goal. هر خانه باید حداکثر دو خط باشد. اگر چیزی را نمی‌دانید با علامت سؤال ثبت کنید، نه با حدس پنهان.»

## اسلاید ۲۳ — Canvas مرجع

**متن پیشنهادی**

«داخل مرز Earn، Balance و History است. Account، Transaction و Inbox داده‌ی مالک هستند. ورودی آینده PaymentSucceededV1 و خروجی فعلی query API است. خرابی consumer باعث توقف مصرف می‌شود، نه rollback پرداخت. Redeem و campaign engine عمداً بیرون‌اند.»

## اسلاید ۲۴ — ADR

**متن پیشنهادی**

«Context می‌گوید چه forcesی داریم. Decision می‌گوید چه انتخاب کردیم. Consequences هزینه و نتیجه را صریح می‌کند. ADR حکم ابدی نیست؛ حافظه‌ی تصمیم است. تصمیم مرجع ما Loyalty مستقل است، چون change cadence و failure policy آن این هزینه را توجیه می‌کند.»

**روی تخته بنویسید:** `ADR-0013: Loyalty as an independently deployable service`.

## اسلاید ۲۵ — Public Contract

**متن پیشنهادی**

«نسخه‌ی اول فقط دو query عمومی دارد: balance و history. Endpoint عمومی برای Earn نداریم، چون Earn فرمان مشتری نیست؛ واکنش Loyalty به یک واقعیت Payment است. این تمایز بعداً ما را به event می‌رساند.»

## اسلاید ۲۶ — Demo Checkpoint 01

**قبل از اجرا بگویید**

«در این Demo فقط استقلال runtime را اثبات می‌کنیم، نه messaging را. انتظار داریم process، port، configuration، health و database مستقل ببینیم.»

**ترتیب نمایش**

1. tag `workshop-01`
2. پروژه‌ی `SmartShop.Loyalty.Api`
3. health endpoint
4. query موجودی یک CustomerId جدید
5. connection string و database مستقل

**جمله‌ی پایان:** «هنوز امتیازی تولید نمی‌شود؛ مرز را ساخته‌ایم، ارتباط را نه.»

## اسلاید ۲۷ — Exit Ticket

**متن پیشنهادی**

«در یک جمله بنویسید چه Gainی هزینه‌ی Service مستقل را توجیه کرد و چه Costی اضافه شد. پاسخ فقط با نام یک تکنولوژی قابل قبول نیست.»

**پاسخ مرجع:** استقلال تغییر و failure؛ در برابر network، operation و consistency پیچیده‌تر.

---

# جلسه‌ی دوم — Communication و Failure

## اسلاید ۲۸ — کاور جلسه دوم

«در جلسه‌ی قبل مرز را انتخاب کردیم. امروز شبکه وارد سیستم می‌شود و هر call می‌تواند فقط در یک طرف موفق شود. سؤال ما این نیست که HTTP بهتر است یا RabbitMQ؛ سؤال این است که intent ارتباط و رفتار failure چیست.»

## اسلاید ۲۹ — مسئله‌ی امروز

«Payment موفق شده اما Loyalty خاموش است. برای کاربر چه پاسخی می‌دهیم؟ Payment در چه وضعیتی می‌ماند؟ چه کسی recovery را انجام می‌دهد؟ سه پاسخ جدا می‌خواهیم: product، data و operation.»

## اسلاید ۳۰ — تمرین چهار راه

**دستور اجرا:** «برای چهار گزینه coupling، failure، consistency و latency را بنویسید. انتخاب سریع ممنوع؛ ابتدا failure mode هر گزینه را پیدا کنید.»

**زمان:** ۸ دقیقه تحلیل، ۲ دقیقه جمع‌بندی.

## اسلاید ۳۱ — Decision Matrix

«HTTP ساده است اما availability و latency را گره می‌زند. Shared DB سرعت اولیه دارد اما ownership را می‌شکند. Event temporal coupling را کم می‌کند اما contract و consistency و broker را اضافه می‌کند. گذاشتن منطق Loyalty داخل Payments هزینه را پنهان و boundary را خراب می‌کند. برای constraint ما، Event بهترین trade-off است.»

## اسلاید ۳۲ — Query، Command و Event

«Query اطلاعات می‌خواهد و intent تغییر ندارد. Command از گیرنده‌ی مشخص می‌خواهد کاری انجام دهد. Event می‌گوید واقعیتی رخ داده است. PaymentSucceeded تصمیم Loyalty برای دادن امتیاز نیست؛ واقعیت متعلق به Payments است.»

## اسلاید ۳۳ — Sync

«Sync زمانی درست است که caller بدون پاسخ همین حالا نمی‌تواند ادامه دهد. پس deadline، timeout و availability دو طرف وارد تجربه‌ی کاربر می‌شوند. Sync ضدالگو نیست؛ coupling آن باید آگاهانه باشد.»

## اسلاید ۳۴ — Async

«در Async، Payments واقعیت را منتشر می‌کند و برای پاسخ Loyalty منتظر نمی‌ماند. Queue فاصله‌ی زمانی را buffer می‌کند. اما broker، backlog، redelivery و contract evolution به مسئولیت ما اضافه می‌شوند.»

## اسلاید ۳۵ — Temporal Coupling

«در مسیر Sync، A و B باید در یک بازه‌ی زمانی آماده باشند. در Async، producer publish می‌کند و consumer بعداً می‌تواند واکنش دهد. دقت کنید Queue ظرفیت بی‌نهایت ندارد و coupling را حذف نمی‌کند؛ زمان آن را تغییر می‌دهد.»

## اسلاید ۳۶ — تمرین طراحی Event

**دستور اجرا:** «نام، version، fields و معنای event را بنویسید. Consumer حق query مستقیم database producer را ندارد. فقط داده‌ای را حمل کنید که برای واکنش لازم است.»

**سؤال نقد:** «اگر یک field حذف شود، کدام consumer می‌شکند؟»

## اسلاید ۳۷ — PaymentSucceededV1

«EventId برای idempotency، OccurredAtUtc برای زمان رخداد، PaymentId و OrderId برای correlation، CustomerId برای حساب Loyalty و Amount برای محاسبه‌ی امتیاز لازم‌اند. snapshot کامل Order یا Customer نمی‌فرستیم.»

## اسلاید ۳۸ — ویژگی قرارداد Event

«نام در زمان گذشته، واقعیت immutable، داده‌ی کافی برای consumer، version صریح و نداشتن domain model مشترک. consumer-oriented data به معنی consumer-owned semantics نیست؛ معنای رخداد را producer تعیین می‌کند.»

## اسلاید ۳۹ — Checkpoint 02

«نسخه‌ی ساده را عمداً ناقص می‌سازیم: DB commit و بعد publish. فاصله‌ی قرمز بین این دو transaction boundary واقعی سیستم است. فعلاً مشکل را حل نکنید؛ باید آن را ببینیم.»

## اسلاید ۴۰ — Happy Path Demo

**قبل از اجرا:** «سه signal می‌خواهیم: response پرداخت، event broker و تغییر balance.»

**ترتیب:** Payment ایجاد کنید؛ log انتشار را نشان دهید؛ log مصرف را ببینید؛ balance و history را query کنید.

**پرسش:** «این Demo چه چیزی را اثبات نکرد؟» پاسخ: رفتار failure و duplicate.

## اسلاید ۴۱ — Eventual Consistency

«Consistency نهایی یک شعار نیست؛ timeline است. Payment در T0 موفق می‌شود، response در T1 برمی‌گردد، event در T2 می‌رسد و balance در T3 تغییر می‌کند. محصول باید پنجره‌ی T1 تا T3 را بپذیرد و UI بتواند آن را توضیح دهد.»

## اسلاید ۴۲ — Delivery و Effect

«At-most-once ممکن است loss بدهد. At-least-once ممکن است duplicate بدهد. چیزی که ما می‌خواهیم exactly-once business effect است، نه وعده‌ی exactly-once delivery. این اثر با idempotency در دامنه ساخته می‌شود.»

## اسلاید ۴۳ — Failure Injection

**قبل از اجرا:** «انتظار داریم Payment ذخیره شود، publish fail شود و بعد از برگشت broker هیچ replay خودکاری نداشته باشیم.»

**حین اجرا:** broker را متوقف کنید؛ Payment را بزنید؛ DB را نشان دهید؛ broker را برگردانید؛ نبودن event را مشاهده کنید.

**Do not fix:** این failure باید تا اسلاید ۵۳ باز بماند.

## اسلاید ۴۴ — Lost Message Window

«Transaction دیتابیس با موفقیت تمام شده است. process قبل از publish crash می‌کند. broker هیچ اطلاعی از intent ما ندارد. سؤال درست این نیست که چرا RabbitMQ پیام را نگه نداشت؛ پیام هرگز به RabbitMQ نرسیده است.»

## اسلاید ۴۵ — Timeout

«Timeout عدد تصادفی نیست؛ از latency budget می‌آید. اگر deadline کاربر ۸۰۰ میلی‌ثانیه است، همه‌ی لایه‌ها نمی‌توانند جداگانه ۸۰۰ میلی‌ثانیه صبر کنند. budget باید میان gateway، service، dependency و margin تقسیم شود.»

## اسلاید ۴۶ — Retry

«Retry فقط برای transient failure، محدود، با backoff و jitter و روی operation idempotent. اگر Charge را بدون idempotency تکرار کنیم، resilience تبدیل به duplicate payment می‌شود. retry می‌تواند failure را تکثیر کند.»

## اسلاید ۴۷ — Circuit Breaker

«Breaker تماس‌های احتمالاً ناموفق را موقتاً متوقف می‌کند تا failure آبشاری نشود. Closed، Open و Half-open حالت‌های کنترل call هستند. Breaker data repair انجام نمی‌دهد و پیام گم‌شده را برنمی‌گرداند.»

## اسلاید ۴۸ — Service Discovery

«در local، Docker DNS نام سرویس را resolve می‌کند. در production ممکن است Kubernetes Service یا Consul این کار را انجام دهد. کد نباید IP ثابت بداند. این کارگاه registry مستقل پیاده نمی‌کند، چون مسئله‌ی اصلی ما routing و failure semantics است.»

## اسلاید ۴۹ — سؤال پل

«Payment و intent انتشار را می‌خواهیم با هم durable کنیم، اما SQL و broker یک transaction محلی مشترک ندارند. بدون 2PC چه چیزی را می‌توانیم در همان database transaction ذخیره کنیم؟»

**پایان جلسه:** پاسخ Outbox را فقط نام ببرید؛ ساختار را به جلسه‌ی بعد بسپارید.

---

# جلسه‌ی سوم — Reliability و Migration

## اسلاید ۵۰ — کاور جلسه سوم

«امروز دو کار می‌کنیم: پیام‌رسانی را در برابر loss و duplicate مقاوم می‌کنیم، بعد Payments را بدون rewrite یک‌باره استخراج می‌کنیم. Reliability و migration یک وجه مشترک دارند: failure و rollback را قبل از اجرا طراحی می‌کنیم.»

## اسلاید ۵۱ — سه Failure Window

«A: commit بدون publish، یعنی loss. B: publish شده اما outbox علامت نخورده، یعنی duplicate. C: consumer commit کرده اما ack نرسیده، یعنی redelivery. طراحی خوب loss را به retry durable و duplicate را به effect قابل کنترل تبدیل می‌کند.»

## اسلاید ۵۲ — Dual Write

«Database و broker دو resource مستقل‌اند. نمی‌توانیم با یک transaction محلی هر دو را اتمیک کنیم. به جای وانمودکردن به اتمیک‌بودن، state و intent را در یک مرز durable می‌کنیم و انتشار را به worker می‌سپاریم.»

## اسلاید ۵۳ — Transactional Outbox

«Payment و OutboxMessage در یک local transaction ذخیره می‌شوند. اگر commit شود، intent انتشار دیگر گم نمی‌شود. Publisher پیام pending را می‌خواند، publish می‌کند و processed را ثبت می‌کند. Outbox delivery را exactly once نمی‌کند.»

**جمله‌ی کلیدی:** «Outbox intent انتشار را durable می‌کند و duplicate را به مسئله‌ای قابل مدیریت تبدیل می‌کند.»

## اسلاید ۵۴ — Producer Atomicity

«فقط Payment و OutboxMessage اتمیک‌اند. Poll، publish و mark processed اتمیک نیستند. اگر بعد از publish و قبل از mark crash کنیم، همان event دوباره publish می‌شود. پس duplicate نتیجه‌ی طبیعی طراحی است.»

## اسلاید ۵۵ — Redelivery

«Consumer business state را commit کرده اما قبل از ack crash می‌کند. broker دوباره پیام را تحویل می‌دهد. این رفتار bug نیست؛ نتیجه‌ی at-least-once است. سؤال ما باید این باشد: effect دوم چگونه صفر شود؟»

## اسلاید ۵۶ — Inbox و Idempotency

«EventId در Inbox ثبت می‌شود و اثر business—LoyaltyTransaction و Balance—در همان local transaction انجام می‌شود. اگر EventId قبلاً دیده شده باشد، handler با success بدون effect جدید تمام می‌شود.»

## اسلاید ۵۷ — تمرین Transaction Boundary

**دستور اجرا:** «دو خط ضخیم بکشید. Producer: Payment + Outbox. Consumer: Inbox + Loyalty effect. crash pointها را بیرون یا بین مرزها علامت بزنید. transaction مشترک SQL و RabbitMQ پاسخ معتبر نیست.»

## اسلاید ۵۸ — Demo Checkpoint 03

**ترتیب اجرا**

1. broker پایین
2. Payment و Outbox commit
3. نمایش pending backlog
4. broker بالا
5. publish و processed
6. replay همان EventId
7. balance فقط یک بار

**جمله‌ی پایان:** «تحویل ممکن است چند بار باشد؛ اثر business یک بار است.»

## اسلاید ۵۹ — محدودیت Outbox

«Outbox ordering، poison message، retention، locking چند worker و schema evolution را خودکار حل نمی‌کند. این‌ها Mention Only هستند؛ فقط نشان می‌دهیم pattern کجا تمام می‌شود تا از آن یک راه‌حل جادویی نسازیم.»

## اسلاید ۶۰ — مسئله‌ی استخراج Payments

«Payments هنوز Module داخل Monolith است و برای اطلاعات Order به contract درون‌پردازشی و shared database تکیه دارد. هدف جداسازی همه‌چیز نیست؛ هدف حذف یک coupling مشخص با کمترین blast radius است.»

## اسلاید ۶۱ — تمرین Seam

**دستور اجرا:** «اولین route، contract داخلی، data owner، rollback و cutover signal را مشخص کنید. طرحی که rollback ندارد کامل نیست.»

**پاسخ مرجع کوتاه:** `/api/payments/**`، `OrderPaymentInfo`، مالکیت Payment، rollback route و signalهای DB/trace.

## اسلاید ۶۲ — Strangler Fig

«Gateway seam ایجاد می‌کند. Client URL عوض نمی‌شود. یک route به service جدید می‌رود و بقیه در Monolith می‌مانند. Strangler اسم دیگری برای rewrite نیست؛ replacement مرحله‌ای با rollback کوچک است.»

## اسلاید ۶۳ — Route Map

«Catalog و Orders فعلاً به Monolith می‌روند. Payments به سرویس جدید و Loyalty به سرویس خودش. route map باید version-controlled باشد. rollback یعنی route را برگردانیم، اما فقط وقتی data ownership و write policy اجازه می‌دهد.»

## اسلاید ۶۴ — Sync Contract داخلی

«Payments برای تصمیم Charge، اطلاعات Order را همین حالا لازم دارد؛ پس یک query داخلی sync داریم. `GetOrderPaymentInfo` read-only و idempotent است و timeout و resilience محدود دارد. حذف همه‌ی sync callها هدف نیست.»

## اسلاید ۶۵ — Event Fan-out

«پس از موفقیت Payment، یک fact منتشر می‌شود. Ordering وضعیت خودش را Paid می‌کند و Loyalty امتیاز می‌دهد. Payments مستقیماً state هیچ‌کدام را تغییر نمی‌دهد و هیچ consumer دیتابیس دیگری را update نمی‌کند.»

## اسلاید ۶۶ — Demo Checkpoint 04 و 05

**قبل از اجرا:** «یک URL عمومی و سه backend داریم. باید route destination، مالکیت Payment و update نهایی Order/Loyalty را ببینیم.»

**ترتیب:** request از Gateway؛ trace به Payments؛ Payment DB؛ event؛ Ordering و Loyalty state؛ rollback route.

## اسلاید ۶۷ — Data Cutover

«Backfill، freeze مالکیت، cutover، verify و rollback window پنج مرحله‌ی مستقل‌اند. dual ownership طولانی ممنوع است. برای هر مرحله باید owner، signal و شرط برگشت مشخص باشد.»

## اسلاید ۶۸ — Test Strategy

«Unit قانون امتیاز را اثبات می‌کند. Architecture مرز dependency را. Integration wiring واقعی DB و broker را. Contract سازگاری HTTP و event را. E2E فقط journey حیاتی را. افزایش تعداد سرویس‌ها نباید ما را به اتکای بیشتر به E2E سوق دهد.»

## اسلاید ۶۹ — Observability Minimum

«یک request باید یک داستان قابل دنبال‌کردن باشد: CorrelationId، log ساخت‌یافته، backlog outbox، failure consumer و trace. اگر failure را نتوانیم توضیح دهیم، سیستم را هنوز طراحی نکرده‌ایم.»

## اسلاید ۷۰ — Liveness و Readiness

«Liveness می‌پرسد process زنده است؟ outage dependency نباید آن را وارد restart-loop کند. Readiness می‌پرسد برای traffic آماده‌ایم؟ dependency حیاتی می‌تواند readiness را false کند و instance از routing خارج شود.»

## اسلاید ۷۱ — Trade-off Ledger

**Interaction:** «سه Gain و سه Cost از کلاس بگیرید. سپس موارد جامانده را اضافه کنید.»

**متن پیشنهادی:** «استقلال release، failure، داده و change cadence را گرفتیم. در عوض network، broker، observability، migration و on-call اضافه شد. اگر فقط ستون چپ را نشان دهیم، معماری را تبلیغ کرده‌ایم نه تحلیل.»

## اسلاید ۷۲ — مدل ذهنی پایانی

**متن پایانی پیشنهادی**

«مرز و ownership را پیدا کنید. بعد failure را طراحی کنید. فقط در مرحله‌ی سوم deployment را جدا کنید. Microservices نقطه‌ی شروع نیست؛ نتیجه‌ی یک تصمیم درباره‌ی استقلال است. هر جا constraint استقلال حذف شد، شجاعت برگشتن به Module را داشته باشید.»

**پرسش آخر:** «اگر Availability و Change Independence این سناریو حذف شوند، کدام بخش معماری را پس می‌گیرید؟»

---

# جملات ممنوع و جایگزین

| نگویید | بگویید |
|---|---|
| Async همیشه بهتر است | Async نوع coupling و failure را تغییر می‌دهد |
| RabbitMQ سرویس‌ها را decouple می‌کند | broker coupling زمانی را کم و coupling قراردادی/عملیاتی را اضافه می‌کند |
| Outbox یعنی exactly once | Outbox intent را durable می‌کند؛ duplicate هنوز ممکن است |
| Database-per-Service یعنی server جدا | یعنی owner و write boundary مستقل |
| Retry availability را حل می‌کند | retry فقط بعضی transient failureها را با پیش‌شرط کنترل می‌کند |
| Strangler یعنی rewrite تدریجی | Strangler replacement route-by-route با rollback محدود است |

# اگر زمان کم آمد

به این ترتیب کوتاه کنید:

1. Service Discovery را به دو دقیقه کاهش دهید.
2. Circuit Breaker را فقط با state diagram توضیح دهید.
3. ارائه‌ی گروه‌ها را به یک گروه موافق و یک گروه مخالف محدود کنید.
4. OpenTelemetry و poison message را فقط نام ببرید.

این بخش‌ها را حذف نکنید:

- تمرین Module یا Microservice
- Failure Injection نسخه‌ی انتشار مستقیم
- Transaction Boundary تمرین Outbox/Inbox
- Strangler Route Map
- Trade-off Ledger نهایی

# اگر Demo اجرا نشد

- Goal و Expected Signal را روی اسلاید نگه دارید.
- log ذخیره‌شده یا diagram را نشان دهید.
- دقیقاً بگویید کدام فرض runtime اثبات نشد.
- تصمیم معماری را ادامه دهید؛ debugging را به بعد کلاس منتقل کنید.
- هرگز خروجی نامرتبط را به‌عنوان موفقیت Demo تفسیر نکنید.
