import fs from "node:fs/promises";
import { Presentation, PresentationFile } from "@oai/artifact-tool";
import JSZip from "jszip";

const OUT = "/workspace/scratch/077b6b390747/smartshop-microservices/materials/slides/SmartShop-Microservices-Workshop-FA.pptx";
const QA = "/workspace/scratch/077b6b390747/work/presentation/qa";

const C = {
  bg: "#F7FAF9", ink: "#102A43", muted: "#52667A", teal: "#0EA5A4",
  green: "#23B26D", coral: "#EF6351", amber: "#F4B942", white: "#FFFFFF",
  line: "#D9E4E8", paleTeal: "#DDF5F3", paleCoral: "#FDE8E4", paleAmber: "#FFF3CE",
  paleNavy: "#E8EEF3", code: "#0B1F33",
};

const FONT = {
  body: "IRANYekan",
  medium: "IRANYekan Medium",
  bold: "IRANYekan",
  display: "IRANYekan ExtraBlack",
  mono: "DejaVu Sans Mono",
};

const rtl = (s) => `\u200F${s}`;
const src = {
  azureMicro: "https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices",
  azureDomain: "https://learn.microsoft.com/en-us/azure/architecture/microservices/model/domain-analysis",
  azureData: "https://learn.microsoft.com/en-us/azure/architecture/microservices/design/data-considerations",
  rabbit: "https://www.rabbitmq.com/client-libraries/dotnet-api-guide",
  awsOutbox: "https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/transactional-outbox.html",
  awsRetry: "https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/retry-backoff.html",
  awsPubSub: "https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/publish-subscribe.html",
  msResilience: "https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience",
  yarp: "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/getting-started?view=aspnetcore-10.0",
  strangler: "https://martinfowler.com/bliki/StranglerFigApplication.html",
  patterns: "https://learn.microsoft.com/en-us/azure/architecture/patterns/",
  domeIntro: "https://dometrain.com/course/getting-started-microservices-architecture/",
  domeDeep: "https://dometrain.com/course/deep-dive-microservices-architecture/",
};

const slides = [
  {type:"cover",section:"کارگاه شش‌ساعته",title:"از مونولیت ماژولار تا مایکروسرویس",sub:"تصمیم، شکست و مهاجرت واقعی",num:"MS",note:"این کارگاه ادامه‌ی مستقیم کارگاه Modular Monolith است. امروز قرار نیست صرفاً چند پروژه‌ی جدا بسازیم؛ قرار است هزینه و دلیل هر جداسازی را ببینیم."},
  {type:"compare",title:"چیزی که امروز نمی‌سازیم",leftTitle:"Distributed Monolith",left:["مرزهای مبهم","وابستگی هم‌زمان","استقرار ظاهراً جدا"],rightTitle:"Intentional Services",right:["مالکیت روشن","Failure مستقل","استقلال قابل‌اندازه‌گیری"],note:"Microservices هدف نیست؛ ابزاری برای استقلال است. اگر مرز و مالکیت روشن نباشد، فقط شبکه را به مشکلات قبلی اضافه کرده‌ایم.",sources:[src.azureMicro]},
  {type:"architecture",title:"در پایان چه خواهیم داشت؟",sub:"یک URL عمومی، سه backend و مالکیت مستقل داده",nodes:["Gateway","SmartShop.Api","Payments","Loyalty","RabbitMQ"],edges:[[0,1],[0,2],[0,3],[2,4],[4,3]],note:"فقط نتیجه را preview کنید. جزئیات مسیر مهاجرت و Reliability را در جلسات بعد باز می‌کنیم.",sources:[src.yarp,src.rabbit]},
  {type:"process",title:"روش کارگاه",steps:["مسئله","تمرین","تئوری","بازطراحی","تصمیم","کد"],note:"پاسخ مدرس قبل از تمرین نمایش داده نمی‌شود. هدف این است که تئوری پاسخ یک درد واقعی باشد، نه فهرست واژه‌ها."},
  {type:"cards",title:"نقشه‌ی سه جلسه",titleSize:38,items:[{h:"01 · Boundary",p:"Loyalty باید Module باشد یا Service؟"},{h:"02 · Communication",p:"وقتی شبکه و failure وارد می‌شوند چه می‌کنیم؟"},{h:"03 · Reliability + Migration",p:"بدون Big Bang چگونه جدا می‌کنیم؟"}],note:"هر جلسه دو ساعت و شامل یک استراحت پنج‌دقیقه‌ای است. هر جلسه با Exit Ticket بسته می‌شود."},

  {type:"cover",section:"جلسه‌ی اول",title:"یک قابلیت کسب‌وکار جدید",sub:"ماژول یا مایکروسرویس؟",num:"01",note:"هدف جلسه: مرز Loyalty را از روی نیاز کسب‌وکار و ویژگی‌های معماری پیدا کنیم."},
  {type:"architecture",title:"SmartShop در نقطه‌ی شروع",sub:"چهار ماژول، یک process، یک deployment",nodes:["Catalog","Ordering","Payments","AI Search"],edges:[[0,1],[1,2],[0,3]],note:"معماری قبلی شکست‌خورده نیست. Modular Monolith نقطه‌ی شروع سالم و قابل‌استخراج ماست."},
  {type:"problem",title:"درخواست جدید محصول",quote:"بعد از پرداخت موفق، مشتری امتیاز بگیرد.",tag:"هنوز راه‌حل فنی نداریم",note:"فعلاً هیچ نام تکنیکی مطرح نکنید. از کلاس بخواهید سؤال‌هایی را پیدا کند که طراحی را عوض می‌کنند."},
  {type:"cards",title:"Product Brief و Constraints",items:[{h:"قاعده",p:"هر ۱۰۰ واحد پرداخت = ۱ امتیاز"},{h:"استقلال تغییر",p:"قواعد Loyalty با cadence جدا تغییر می‌کند"},{h:"Failure",p:"خرابی Loyalty نباید Payment را rollback کند"},{h:"قابلیت توضیح",p:"تاریخچه‌ی Earn باید قابل پیگیری باشد"}],note:"این چهار constraint مبنای تمام تصمیم‌های بعدی‌اند؛ نه علاقه‌ی ما به ابزارها."},
  {type:"exercise",title:"اول سؤال، بعد طراحی",time:"۱۰ دقیقه",deliverable:"۵ سؤال تعیین‌کننده + ۳ فرض صریح",prompts:["Trigger دقیق چیست؟","CustomerId از کجا می‌آید؟","چه Consistency پذیرفتنی است؟"],note:"گروه‌ها هنوز حق انتخاب تکنولوژی ندارند. فقط ابهام‌ها و فرض‌ها را ثبت کنند."},
  {type:"cards",title:"ابهام‌های تعیین‌کننده",items:[{h:"Trigger",p:"authorized یا settled؟"},{h:"Identity",p:"مهمان هم امتیاز می‌گیرد؟"},{h:"Consistency",p:"چند ثانیه تأخیر پذیرفتنی است؟"},{h:"Scope",p:"Earn، Redeem یا هر دو؟"}],note:"پاسخ مرجع: trigger=PaymentSucceeded، فقط مشتری شناخته‌شده، eventual consistency پذیرفتنی، نسخه‌ی اول فقط Earn."},
  {type:"compare",title:"Module با Microservice برابر نیست",leftTitle:"مرز منطقی",left:["زبان و مدل","قواعد و داده","Dependency direction"],rightTitle:"مرز runtime",right:["Process و port","deployment","failure و operation"],footerCallout:"Service = مرز منطقی + مالکیت + runtime مستقل",note:"سه مرز را جدا کنید: logical boundary، runtime boundary و ownership boundary."},
  {type:"statement",title:"Microservice حول Business Capability شکل می‌گیرد",big:"کاری که کسب‌وکار انجام می‌دهد؛ نه تعداد فایل‌ها",accent:"Loyalty = Earn + History + Balance",note:"اندازه‌ی کد معیار نیست. قابلیت کسب‌وکار و تغییرات هم‌جهت، سرنخ قوی‌تری هستند.",sources:[src.azureDomain]},
  {type:"balance",title:"استقلال رایگان نیست",left:"Independence",right:"Distributed-system Cost",leftItems:["release مستقل","failure containment","team autonomy"],rightItems:["network","observability","security","on-call","test complexity"],note:"برای هر استقلالی که می‌خریم، هزینه‌ی عملیاتی پرداخت می‌کنیم. این تراز در پایان دوباره سنجیده می‌شود.",sources:[src.azureMicro]},
  {type:"bars",title:"Architecture Characteristics برای Loyalty",items:[{n:"Change Independence",v:92},{n:"Availability",v:78},{n:"Explainable Consistency",v:72},{n:"Operability",v:65}],note:"این اعداد اندازه‌گیری علمی نیستند؛ فقط ترتیب اولویت برای بحث‌اند. از کلاس بخواهید یک ویژگی حذف‌شده را پیشنهاد کند."},
  {type:"exercise",title:"Module یا Microservice؟",time:"۱۰ دقیقه",deliverable:"یک تصمیم + سه شاهد",prompts:["Module","Microservice","Module با Extraction Point"],note:"به کلاس اجازه دهید گزینه‌ی سوم را انتخاب کند. معیار ارزیابی، کیفیت شواهد است نه یک پاسخ از پیش‌تعیین‌شده."},
  {type:"cards",title:"چه شواهدی تصمیم را معتبر می‌کنند؟",items:[{h:"قوی",p:"Rate of Change · Availability · Data Ownership"},{h:"متوسط",p:"Team Ownership · Scale Profile · Reuse Channel"},{h:"ضعیف",p:"تعداد کلاس · مُد بازار · علاقه به Broker"}],note:"تصمیم مرزی باید به evidence متصل شود. تغییر مستقل و نیاز availability مهم‌ترین شواهد سناریوی ما هستند."},
  {type:"cards",title:"چهار نوع Coupling",items:[{h:"Temporal",p:"هم‌زمان باید بالا باشند"},{h:"Data",p:"یک schema یا مدل مشترک"},{h:"Contract",p:"تغییر پیام، مصرف‌کننده را می‌شکند"},{h:"Organizational",p:"هر تغییر نیازمند هماهنگی تیمی"}],note:"Async فقط coupling زمانی را کاهش می‌دهد؛ coupling قراردادی و سازمانی همچنان باقی است."},
  {type:"heatmap",title:"سرنخ‌های Boundary",cols:["Ordering","Payments","Loyalty"],rows:[{n:"قواعد قیمت/سفارش",v:[3,1,0]},{n:"درگاه و تسویه",v:[1,3,0]},{n:"کمپین و امتیاز",v:[0,1,3]}],note:"چیزهایی را کنار هم بگذار که با هم تغییر می‌کنند. heatmap صرفاً ابزار گفت‌وگو است."},
  {type:"matrix",title:"چه کسی مالک کدام داده است؟",cols:["Ordering","Payments","Loyalty"],rows:[{n:"Order",v:["مالک","Reference","Reference"]},{n:"Payment",v:["Reference","مالک","Reference"]},{n:"LoyaltyAccount",v:["—","—","مالک"]},{n:"CustomerId",v:["شناسه","شناسه","شناسه"]}],note:"Ownership با read access فرق دارد. سرویس دیگر ممکن است یک reference داشته باشد، اما حق mutation یا join مستقیم ندارد.",sources:[src.azureData]},
  {type:"compare",title:"Database-per-Service دقیقاً یعنی چه؟",leftTitle:"یعنی",left:["مالک schema و migration مستقل","دسترسی از طریق contract","transaction محلی"],rightTitle:"الزاماً یعنی نیست",right:["server فیزیکی جدا","vendor متفاوت","هزینه‌ی زیرساخت سه‌برابر"],footerCallout:"Shared server ممکن است؛ Shared ownership نه",note:"cross-service join و transaction مستقیم ممنوع است. استقلال منطقی داده اصل است، نه topology فیزیکی.",sources:[src.azureData]},
  {type:"exercise",title:"Context Canvas را کامل کنید",time:"۱۱ دقیقه",deliverable:"۸ خانه برای مرز Loyalty",prompts:["Responsibility + Data","Inbound + Outbound + Dependency","Consistency + Failure + Non-goal"],note:"قالب چاپی یا فایل Miro را بدهید. خروجی باید یک boundary قابل‌نقد باشد، نه diagram زیبا."},
  {type:"canvas",title:"Context Canvas مرجع Loyalty",items:[{h:"داخل مرز",p:"Earn · Balance · History"},{h:"داده‌ی مالک",p:"Account · Transaction · Inbox"},{h:"ورودی",p:"PaymentSucceededV1"},{h:"خروجی",p:"Balance / History API"},{h:"وابستگی",p:"Broker + DB"},{h:"Consistency",p:"Eventual؛ چند ثانیه"},{h:"Failure",p:"توقف مصرف، نه rollback payment"},{h:"Non-goal",p:"Redeem و campaign engine"}],note:"این پاسخ مرجع است. اختلاف‌های کلاس را با constraintها بسنجید، نه با سلیقه."},
  {type:"decision",title:"تصمیم را ثبت می‌کنیم",context:"قواعد Loyalty مستقل تغییر می‌کند و outage آن نباید Payment را برگرداند.",decision:"Loyalty به‌صورت سرویس و مالک داده‌ی خودش ساخته می‌شود.",consequences:["+ deployment و failure مستقل","− شبکه، broker و operation جدید","− consistency فوری نداریم"],note:"ADR-0013 مرز Loyalty و ADR-0014 مدل ارتباط را ثبت می‌کنند. ADR سند حقیقت ابدی نیست؛ رد تصمیم و هزینه‌هاست."},
  {type:"contract",title:"Public Contract نسخه‌ی اول",code:"GET /api/loyalty/{customerId}/balance\n→ { customerId, points }\n\nGET /api/loyalty/{customerId}/history\n→ [{ transactionId, points, occurredAtUtc }]",callout:"Endpoint عمومی Earn نداریم",note:"کسب امتیاز consequence یک واقعیت Payment است، نه command عمومی مشتری."},
  {type:"demo",title:"مرز اجرای نسخه‌ی اول را ببین",titleSize:38,goal:"Loyalty یک process واقعی و مستقل است",action:"health و balance مشتری جدید را صدا بزن",expected:"/health = Healthy · balance = 0",reset:"database مستقل را پاک و سرویس را restart کن",tag:"workshop-01",note:"در diff فقط مرزهای مهم را نشان دهید: project، port، image، config، health و database."},
  {type:"exit",title:"Exit Ticket · جلسه‌ی اول",question:"کدام نیاز، هزینه‌ی سرویس مستقل را توجیه کرد؟",sub:"و دقیقاً چه هزینه‌ای به سیستم اضافه شد؟",note:"دو پاسخ کوتاه از کلاس بگیرید. پاسخ کامل باید هم gain و هم cost را نام ببرد."},

  {type:"cover",section:"جلسه‌ی دوم",title:"وقتی شبکه وارد معماری می‌شود",sub:"Communication، Coupling و Failure",num:"02",note:"هدف جلسه: ارتباط را بر اساس intent و failure semantics انتخاب کنیم، نه بر اساس محبوبیت ابزار."},
  {type:"problem",title:"مسئله‌ی امروز",quote:"اگر Payment موفق شود و Loyalty خاموش باشد چه باید شود؟",tag:"پاسخ محصول ≠ پاسخ زیرساخت",note:"از کلاس سه پاسخ بخواهید: برای کاربر، برای داده، و برای operation چه اتفاقی می‌افتد؟"},
  {type:"exercise",title:"چهار راه ارتباط",time:"۱۰ دقیقه",deliverable:"Coupling · Failure · Consistency · Latency",prompts:["HTTP مستقیم","دیتابیس مشترک","Integration Event","منطق داخل Payments"],note:"گروه‌ها یک گزینه را defend کنند و یک failure mode آن را صریح بنویسند."},
  {type:"matrix",title:"هیچ گزینه‌ای بدون هزینه نیست",cols:["HTTP","Shared DB","Event","Inside Payment"],rows:[{n:"Availability",v:["وابسته","وابسته","بهتر","بهتر"]},{n:"Coupling",v:["زمانی","داده‌ای","قراردادی","دامنه‌ای"]},{n:"Consistency",v:["فوری","فوری","نهایی","فوری"]},{n:"Complexity",v:["متوسط","پنهان","بالا","تعویق‌شده"]}],note:"Shared DB کم‌هزینه به نظر می‌رسد اما ownership و migration را می‌شکند. گزینه‌ی ما event است چون payment نباید منتظر loyalty بماند."},
  {type:"cards",title:"Query، Command و Event",items:[{h:"Query",p:"اطلاعات بده · GetOrderPaymentInfo"},{h:"Command",p:"این کار را انجام بده · ChargeOrder"},{h:"Event",p:"این اتفاق افتاد · PaymentSucceeded"}],note:"event در زمان گذشته است و مالک تصمیم را producer می‌داند. consumer تصمیم می‌گیرد چگونه واکنش نشان دهد."},
  {type:"sequence",title:"ارتباط Sync چه زمانی درست است؟",actors:["Caller","Service","Dependency"],messages:["درخواست","برای ادامه پاسخ لازم است","پاسخ در timeout budget"],note:"وقتی caller بدون پاسخ نمی‌تواند تصمیمش را بگیرد، sync طبیعی است؛ اما availability و latency به هم گره می‌خورند."},
  {type:"architecture",title:"ارتباط Async چه زمانی درست است؟",sub:"Producer واقعیتی را اعلام می‌کند",nodes:["Payments","RabbitMQ","Ordering","Loyalty"],edges:[[0,1],[1,2],[1,3]],note:"producer منتظر نتیجه‌ی business consumer نمی‌ماند. این یعنی کاهش temporal coupling، نه حذف تمام coupling.",sources:[src.awsPubSub]},
  {type:"timelineCompare",title:"Temporal Coupling را ببین",topTitle:"Sync",top:["A آماده","B باید آماده","پاسخ"],bottomTitle:"Async",bottom:["A publish","Queue buffer","B هر زمان آماده شد"],note:"در Async، broker ظرفیت و failure mode جدید می‌آورد. buffer بودن به معنی بی‌نهایت‌بودن یا بدون‌خطر بودن نیست."},
  {type:"exercise",title:"Event را طراحی کنید",time:"۱۰ دقیقه",deliverable:"Name · Version · Fields · Semantics",prompts:["past tense باشد","مصرف‌کننده DB producer را query نکند","داده‌ی لازم برای واکنش را حمل کند"],note:"هر گروه قراردادش را روی یک کارت بنویسد. قبل از نمایش پاسخ مرجع، version و معنای event را نقد کنید."},
  {type:"contract",title:"PaymentSucceededV1",code:"{\n  \"eventId\": \"uuid\",\n  \"occurredAtUtc\": \"2026-...Z\",\n  \"paymentId\": \"uuid\",\n  \"orderId\": \"uuid\",\n  \"customerId\": \"uuid\",\n  \"amount\": 12500\n}",callout:"Immutable fact · Explicit version",note:"شناسه‌ی رخداد برای idempotency لازم است. مقدار payment برای محاسبه‌ی امتیاز در consumer حمل می‌شود."},
  {type:"radial",title:"قرارداد Event خوب",center:"PaymentSucceededV1",items:["Past tense","Immutable fact","Consumer data","Explicit version","No shared domain model"],note:"contract را حول نیاز consumer طراحی کنید اما semantics آن باید fact متعلق به producer باقی بماند."},
  {type:"architecture",title:"Checkpoint 02 · انتشار مستقیم",sub:"نقطه‌ی قرمز، محل مشکل است",nodes:["Payment DB commit","Publish","RabbitMQ","Loyalty consume"],edges:[[0,1],[1,2],[2,3]],dangerEdge:0,note:"نسخه‌ی ساده را عمداً می‌سازیم تا failure window دیده شود. فعلاً publish بلافاصله بعد از commit انجام می‌شود."},
  {type:"demo",title:"Demo · Happy Path",goal:"Payment تا Loyalty balance",action:"Payment موفق برای مشتری ثابت بساز",expected:"201 → event → افزایش balance",reset:"customer ثابت + database clean",tag:"workshop-02",note:"سه signal را جدا نشان دهید: response پرداخت، پیام broker و query موجودی."},
  {type:"process",title:"Eventual Consistency یک Timeline است",steps:["T0 · Payment","T1 · Response","T2 · Event","T3 · Balance"],note:"پنجره‌ی inconsistency را صریح و قابل‌مشاهده کنید. سؤال محصول: چند ثانیه تأخیر پذیرفتنی است؟"},
  {type:"compare3",title:"Delivery با Business Effect فرق دارد",items:[{h:"At-most-once",p:"ممکن است از دست برود"},{h:"At-least-once",p:"ممکن است تکرار شود"},{h:"Exactly-once effect",p:"با idempotency در دامنه"}],note:"قول exactly-once delivery ندهید. هدف عملی، at-least-once delivery با اثر کسب‌وکار idempotent است.",sources:[src.rabbit]},
  {type:"demo",title:"Demo · Failure Injection",goal:"Lost message را واقعی ببین",action:"RabbitMQ خاموش → Payment ثبت",expected:"Payment ذخیره؛ publish fail؛ replay نداریم",reset:"broker را بالا بیاور؛ داده را بررسی کن",tag:"workshop-02",danger:true,note:"این failure را repair نکنید؛ اجازه دهید کلاس مشکل را مشاهده کند. سپس سؤال transaction را مطرح کنید."},
  {type:"sequence",title:"پنجره‌ی Lost Message",actors:["Payments","Database","Broker"],messages:["COMMIT ✓","CRASH ×","PUBLISH انجام نشد"],danger:true,note:"Database transaction تمام شده اما intent انتشار durable نشده است. transaction واقعی فقط داخل DB بوده."},
  {type:"budget",title:"Timeout · بودجه‌ی انتظار",total:"800ms",parts:[{n:"Gateway",v:100},{n:"Service",v:250},{n:"Dependency",v:350},{n:"Margin",v:100}],note:"timeout از latency budget می‌آید، نه حدس. timeoutهای لایه‌ها باید با deadline کلی سازگار باشند.",sources:[src.msResilience]},
  {type:"checklist",title:"Retry می‌تواند خطا را تکثیر کند",items:["فقط خطای transient","تعداد محدود","backoff + jitter","عملیات idempotent"],warning:"روی Charge بدون idempotency، retry می‌تواند پرداخت تکراری بسازد",note:"retry درمان همه خطاها نیست. عدم idempotency، outage طولانی و overload سه ضدشرط مهم‌اند.",sources:[src.awsRetry,src.msResilience]},
  {type:"state",title:"Circuit Breaker جلوی شکست آبشاری را می‌گیرد",states:["Closed","Open","Half-open"],note:"breaker تماس‌های بی‌فایده را موقتاً متوقف می‌کند و فرصت recovery می‌دهد؛ داده‌ی از دست‌رفته را repair نمی‌کند.",sources:[src.patterns]},
  {type:"compare",title:"Service Discovery در این کارگاه",leftTitle:"Local",left:["Docker DNS","نام سرویس در compose","port داخلی ثابت"],rightTitle:"Production Context",right:["Kubernetes Service","Consul یا platform registry","health-aware routing"],footerCallout:"کد نباید IP ثابت بداند",note:"در کارگاه Docker DNS کافی است. مفاهیم registry/discovery را توضیح دهید ولی ابزار جدا اضافه نکنید."},
  {type:"problem",title:"سؤال پل به جلسه‌ی سوم",quote:"چگونه state و intent انتشار را اتمیک کنیم، بدون 2PC؟",tag:"Payment + Outbox",note:"جواب را هنوز کامل نشان ندهید. کلاس باید transaction boundary را حدس بزند."},

  {type:"cover",section:"جلسه‌ی سوم",title:"قابلیت اطمینان و مهاجرت",sub:"بدون مهاجرت یک‌باره",num:"03",note:"هدف جلسه: failure windowها را با outbox/inbox کنترل کنیم و Payments را route-by-route استخراج کنیم."},
  {type:"threeFailures",title:"سه Failure Window",items:[{h:"A",p:"commit بدون publish"},{h:"B",p:"publish بدون mark outbox"},{h:"C",p:"consumer commit بدون ack"}],note:"هر پنجره یک پیامد متفاوت دارد: loss یا duplicate. شرکت‌کننده باید بداند کدام‌یک طبیعی و کدام‌یک repairable است."},
  {type:"problem",title:"مسئله‌ی Dual Write",quote:"دو resource مستقل، یک transaction محلی ندارند.",tag:"Database ≠ Broker",note:"2PC را وارد پیاده‌سازی کارگاه نمی‌کنیم. به جای آن، intent را در همان DB تراکنش business ذخیره می‌کنیم.",sources:[src.awsOutbox]},
  {type:"architecture",title:"Transactional Outbox",sub:"State و publish intent در یک transaction",nodes:["Payment","OutboxMessage","Local DB","Publisher","Broker"],edges:[[0,2],[1,2],[2,3],[3,4]],note:"Payment و OutboxMessage اتمیک‌اند. Publisher بعداً پیام‌های pending را منتشر می‌کند.",sources:[src.awsOutbox]},
  {type:"process",title:"سمت Producer دقیقاً چه اتمیک است؟",steps:["Local TX","Poll pending","Publish","Mark processed"],note:"فقط مرحله‌ی اول اتمیک است. crash بعد از publish و قبل از mark باعث duplicate می‌شود؛ طراحی باید آن را بپذیرد."},
  {type:"sequence",title:"چرا Consumer دوباره پیام را می‌بیند؟",actors:["Consumer","Database","Broker"],messages:["Business commit ✓","CRASH ×","ACK نرسید → Redelivery"],danger:true,note:"redelivery رفتار طبیعی at-least-once است، نه bug broker.",sources:[src.rabbit]},
  {type:"architecture",title:"Idempotent Consumer و Inbox",sub:"اثر business و ProcessedMessage در یک transaction",nodes:["EventId","Inbox","LoyaltyTransaction","Balance"],edges:[[0,1],[1,2],[2,3]],note:"اگر EventId قبلاً در Inbox وجود دارد، effect دوباره اعمال نمی‌شود. Inbox و business state باید در یک DB transaction باشند."},
  {type:"exercise",title:"Transaction Boundary را رسم کنید",time:"۱۰ دقیقه",deliverable:"دو boundary مستقل Producer و Consumer",prompts:["Payment + Outbox","Inbox + Loyalty effect","نه transaction مشترک SQL + RabbitMQ"],note:"از گروه‌ها بخواهید مرز اتمیک را با خط ضخیم بکشند و crash pointها را علامت بزنند."},
  {type:"demo",title:"Demo · Checkpoint 03",goal:"Recovery و duplicate-safe effect",action:"broker down → payment → broker up → replay",expected:"outbox pending سپس processed؛ balance فقط یک بار",reset:"اسکریپت reset و customer ثابت",tag:"workshop-03",note:"ابتدا backlog را نشان دهید، سپس recovery و در پایان replay عمدی همان EventId را."},
  {type:"compare",title:"Outbox چه چیزی را حل نمی‌کند؟",leftTitle:"حل می‌کند",left:["commit بدون intent","retry durable","event loss window"],rightTitle:"هنوز با شماست",right:["ordering","poison message","retention و locking","schema evolution"],note:"Outbox یک الگوی reliability محدود است، نه message platform کامل.",sources:[src.awsOutbox]},
  {type:"architecture",title:"حالا Payments را استخراج کنیم",sub:"قبل از جداسازی، coupling map را ببین",nodes:["Ordering","Payments Module","Shared DB","Loyalty","Broker"],edges:[[0,1],[1,2],[1,4],[4,3]],dangerEdge:1,note:"وابستگی اصلی: Payments برای تصمیم charge به اطلاعات Order نیاز دارد و هنوز از shared DB استفاده می‌کند."},
  {type:"exercise",title:"اولین Seam کجاست؟",time:"۱۰ دقیقه",deliverable:"Route · Contract · Data owner · Rollback · Signal",prompts:["کدام route؟","چه داده‌ای باید منتقل شود؟","چطور برمی‌گردیم؟"],note:"هدف، کوچک‌ترین cutover قابل برگشت است؛ نه استخراج کامل در یک حرکت."},
  {type:"architecture",title:"Strangler Fig",sub:"Route-by-route replacement",nodes:["Client","Gateway","Monolith","New Payments"],edges:[[0,1],[1,2],[1,3]],note:"Gateway seam می‌سازد؛ بخشی از traffic به سرویس جدید می‌رود و rollback با تغییر route ممکن است.",sources:[src.strangler,src.yarp]},
  {type:"routeMap",title:"Route Map قبل و بعد",rows:[{r:"/api/catalog/**",b:"Monolith",a:"Monolith"},{r:"/api/orders/**",b:"Monolith",a:"Monolith"},{r:"/api/payments/**",b:"Monolith",a:"Payments"},{r:"/api/loyalty/**",b:"—",a:"Loyalty"}],note:"route map باید دقیق، version-controlled و قابل rollback باشد. یک URL عمومی برای client حفظ می‌شود.",sources:[src.yarp]},
  {type:"architecture",title:"یک Sync Contract هنوز لازم است",sub:"برای تصمیم charge، پاسخ همین حالا لازم است",nodes:["Payments","Ordering Internal API","Order DB"],edges:[[0,1],[1,2]],note:"GetOrderPaymentInfo یک query داخلی و read-only است. timeout و resilience محدود داریم؛ retry برای query idempotent امن‌تر است.",sources:[src.msResilience]},
  {type:"architecture",title:"نتیجه‌ی Payment با Event پخش می‌شود",sub:"یک fact، چند واکنش مستقل",nodes:["Payments","RabbitMQ","Ordering","Loyalty"],edges:[[0,1],[1,2],[1,3]],note:"Ordering وضعیت خودش را update می‌کند و Loyalty امتیاز می‌دهد. هیچ consumer مالک state دیگری نیست.",sources:[src.awsPubSub]},
  {type:"demo",title:"Demo · Checkpoint 04 و 05",goal:"یک URL عمومی، سه backend",action:"Payment را از Gateway ثبت کن",expected:"route→Payments؛ Order و Loyalty نهایی update",reset:"route Payments را به Monolith برگردان",tag:"workshop-04 / workshop-05",note:"ابتدا route destination را نشان دهید، سپس DB مالک Payment و بعد اثر eventual روی Ordering و Loyalty."},
  {type:"process",title:"Data Cutover و Rollback",steps:["Backfill","Ownership freeze","Cutover","Verify","Rollback window"],note:"dual ownership طولانی ممنوع است. برای هر مرحله owner، signal و شرط rollback تعریف کنید."},
  {type:"layers",title:"Test Strategy",items:[{h:"Unit",p:"قاعده‌ی امتیاز"},{h:"Architecture",p:"مرز dependency"},{h:"Integration",p:"DB + broker"},{h:"Contract",p:"HTTP/Event compatibility"},{h:"E2E",p:"مسیر حیاتی"}],note:"هر تست سؤال متفاوتی را جواب می‌دهد. E2E کم و حیاتی؛ integration و contract برای مرزها مهم‌تر می‌شوند."},
  {type:"trace",title:"Observability Minimum",items:["CorrelationId","Structured logs","Outbox backlog","Consumer failures","Distributed trace"],note:"بدون correlation، queue depth و trace نمی‌توان failure را توضیح داد. observability بخشی از معماری است، نه افزودنی آخر کار."},
  {type:"compare",title:"Liveness با Readiness یکی نیست",leftTitle:"Liveness",left:["process زنده است؟","dependency outage نباید restart-loop بسازد","failure داخلی را نشان می‌دهد"],rightTitle:"Readiness",right:["آماده‌ی traffic است؟","dependency حیاتی را بررسی می‌کند","از routing خارج می‌شود"],note:"health check باید هدف عملیاتی داشته باشد. dependency outage را بی‌دلیل به liveness وصل نکنید."},
  {type:"ledger",title:"Trade-off Ledger",leftTitle:"Independence Gained",left:["release","failure","data ownership","change cadence"],rightTitle:"Complexity Added",right:["network","broker","observability","migration","on-call"],note:"از کلاس سه مورد دیگر بگیرید. اگر ستون راست دیده نشود، تصمیم microservice احتمالاً تبلیغاتی است."},
  {type:"final",title:"مدل ذهنی پایانی",steps:["مرز و Ownership را پیدا کن","Failure را طراحی کن","بعد Deployment را جدا کن"],footerCallout:"Repository: github.com/oroumand/SmartShop.Microservices · tags: workshop-00 … workshop-06",note:"این سه مرحله را به‌عنوان معیار بازبینی هر استخراج تکرار کنید. پایان با Q&A و مسیر مطالعه.",sources:[src.domeIntro,src.domeDeep]},
];

function rect(slide, x, y, w, h, fill=C.white, line=C.line, radius=true) {
  return slide.shapes.add({geometry: radius ? "roundRect" : "rect", position:{left:x,top:y,width:w,height:h}, fill, line:{style:"solid",fill:line,width:1}});
}

function textBox(slide, text, x, y, w, h, opts={}) {
  const s = slide.shapes.add({geometry:"textbox",position:{left:x,top:y,width:w,height:h},fill:"none",line:{style:"solid",fill:"none",width:0}});
  s.text = opts.ltr ? text : rtl(text);
  s.text.style = {
    fontSize: opts.size ?? 24, color: opts.color ?? C.ink, bold: opts.bold ?? false,
    typeface: opts.font ?? (opts.bold ? FONT.bold : FONT.body), alignment: opts.align ?? (opts.ltr ? "left" : "right"),
    verticalAlignment: opts.valign ?? "middle", autoFit:"shrinkText", wrap:opts.wrap ?? "square",
    insets:{left:opts.inset ?? 8,right:opts.inset ?? 8,top:opts.inset ?? 4,bottom:opts.inset ?? 4},
  };
  return s;
}

function base(slide, n, title, section, titleSize=47) {
  slide.background.fill = C.bg;
  slide.shapes.add({geometry:"rect",position:{left:0,top:0,width:14,height:720},fill:C.teal,line:{style:"solid",fill:C.teal,width:0}});
  if (title) textBox(slide,title,72,62,1100,64,{size:titleSize,bold:true,font:FONT.bold,wrap:"none",valign:"top"});
  textBox(slide,section || (n<28?"جلسه ۱ · Boundary":n<50?"جلسه ۲ · Communication":"جلسه ۳ · Reliability + Migration"),72,18,520,24,{size:14,color:C.teal,bold:true});
  textBox(slide,String(n).padStart(2,"0"),1160,665,50,28,{size:13,color:C.muted,align:"center",ltr:true});
  slide.shapes.add({geometry:"rect",position:{left:72,top:678,width:1060,height:2},fill:C.line,line:{style:"solid",fill:C.line,width:0}});
}

function pill(slide,label,x,y,w,color=C.teal,fill=C.paleTeal,ltr=false) {
  const p=rect(slide,x,y,w,34,fill,fill,true);
  textBox(slide,label,x+2,y+1,w-4,30,{size:15,bold:true,color,align:"center",ltr});
  return p;
}

function node(slide,label,x,y,w,h,color=C.teal,sub="") {
  const s=rect(slide,x,y,w,h,C.white,color,true);
  slide.shapes.add({geometry:"rect",position:{left:x,top:y,width:8,height:h},fill:color,line:{style:"solid",fill:color,width:0}});
  textBox(slide,label,x+16,y+10,w-26,sub?30:h-20,{size:20,bold:true,align:"center",ltr:/^[A-Za-z0-9_./* -]+$/.test(label)});
  if(sub) textBox(slide,sub,x+16,y+42,w-26,h-48,{size:14,color:C.muted,align:"center"});
  return s;
}

function arrow(slide,a,b,danger=false,from="right",to="left") {
  return slide.shapes.connect(a,b,{kind:"straight",fromSide:from,toSide:to,line:{style:danger?"dashed":"solid",fill:danger?C.coral:C.teal,width:3},tail:{type:"arrow",width:"med",length:"med"}});
}

function card(slide,h,p,x,y,w,hgt,accent=C.teal,idx=null) {
  rect(slide,x,y,w,hgt,C.white,C.line,true);
  slide.shapes.add({geometry:"rect",position:{left:x,top:y,width:w,height:6},fill:accent,line:{style:"solid",fill:accent,width:0}});
  if(idx!==null) pill(slide,String(idx).padStart(2,"0"),x+w-62,y+14,44,accent,accent===C.amber?C.paleAmber:C.paleTeal,true);
  textBox(slide,h,x+18,y+18,w-36,hgt>150?48:42,{size:30,bold:true,color:C.ink});
  textBox(slide,p,x+18,y+(hgt>150?72:64),w-36,hgt-(hgt>150?88:76),{size:22,color:C.muted});
}

function renderSlide(pres,d,n) {
  const slide=pres.slides.add();
  const t=d.type;
  if(t==="cover") {
    slide.background.fill=C.ink;
    slide.shapes.add({geometry:"ellipse",position:{left:-100,top:-150,width:480,height:480},fill:C.teal,line:{style:"solid",fill:C.teal,width:0},transparency:22});
    slide.shapes.add({geometry:"ellipse",position:{left:1040,top:510,width:320,height:320},fill:C.amber,line:{style:"solid",fill:C.amber,width:0},transparency:16});
    textBox(slide,d.section,720,92,450,36,{size:18,color:C.amber,bold:true});
    textBox(slide,d.title,430,166,740,180,{size:n===1?68:64,color:C.white,bold:true,font:FONT.display});
    textBox(slide,d.sub,580,358,590,78,{size:32,color:C.paleTeal,font:FONT.medium});
    textBox(slide,d.num,80,360,350,230,{size:d.num.length>2?118:178,color:C.white,bold:true,font:FONT.display,align:"center",ltr:true});
    textBox(slide,"SmartShop · Architecture Workshop",780,620,390,30,{size:16,color:C.line,align:"right",ltr:true});
  } else {
    base(slide,n,d.title,d.section,d.titleSize);
    const X=72,Y=138,W=1136,H=500;
    if(t==="problem") {
      slide.shapes.add({geometry:"ellipse",position:{left:94,top:190,width:170,height:170},fill:C.paleCoral,line:{style:"solid",fill:C.paleCoral,width:0}});
      textBox(slide,"؟",104,185,150,170,{size:118,bold:true,color:C.coral,align:"center"});
      textBox(slide,d.quote,330,188,790,205,{size:49,bold:true,font:FONT.display});
      pill(slide,d.tag,760,450,360,C.coral,C.paleCoral);
    } else if(t==="statement") {
      textBox(slide,d.big,155,188,970,190,{size:50,bold:true,font:FONT.display,align:"center"});
      slide.shapes.add({geometry:"rect",position:{left:340,top:410,width:600,height:4},fill:C.teal,line:{style:"solid",fill:C.teal,width:0}});
      textBox(slide,d.accent,265,447,750,68,{size:27,bold:true,color:C.teal,align:"center",ltr:/^[A-Za-z]/.test(d.accent)});
    } else if(t==="exercise") {
      rect(slide,X,Y,W,88,C.paleAmber,C.amber,true);
      textBox(slide,"تمرین",X+24,Y+10,160,58,{size:29,bold:true,color:C.ink});
      pill(slide,d.time,X+W-210,Y+26,165,C.ink,C.amber);
      textBox(slide,"خروجی قابل تحویل",X+680,Y+112,420,34,{size:17,color:C.muted,bold:true});
      textBox(slide,d.deliverable,X+420,Y+148,680,64,{size:28,bold:true});
      const pw=(W-48)/d.prompts.length;
      d.prompts.forEach((p,i)=>card(slide,String(i+1),p,X+i*pw,Y+252,pw-16,132,C.amber));
    } else if(t==="compare" || t==="ledger") {
      const lw=542;
      const lAccent=t==="ledger"?C.green:C.coral, rAccent=C.teal;
      card(slide,d.leftTitle,d.left.map(x=>`• ${x}`).join("\n"),X,Y,lw,360,lAccent);
      card(slide,d.rightTitle,d.right.map(x=>`• ${x}`).join("\n"),X+594,Y,lw,360,rAccent);
      if(d.footerCallout) pill(slide,d.footerCallout,285,535,710,C.ink,C.paleNavy);
    } else if(t==="cards" || t==="canvas") {
      const count=d.items.length, cols=count<=4?count:4, rows=Math.ceil(count/cols), gap=18;
      const cw=(W-gap*(cols-1))/cols, ch=(H-gap*(rows-1))/rows;
      d.items.forEach((it,i)=>card(slide,it.h,it.p,X+(i%cols)*(cw+gap),Y+Math.floor(i/cols)*(ch+gap),cw,ch,[C.teal,C.green,C.amber,C.coral][i%4],i+1));
    } else if(t==="balance") {
      card(slide,d.left,d.leftItems.map(x=>`• ${x}`).join("\n"),X,Y,430,350,C.green);
      card(slide,d.right,d.rightItems.map(x=>`• ${x}`).join("\n"),X+706,Y,430,350,C.coral);
      slide.shapes.add({geometry:"rect",position:{left:500,top:415,width:280,height:12},fill:C.ink,line:{style:"solid",fill:C.ink,width:0},rotation:0});
      slide.shapes.add({geometry:"triangle",position:{left:600,top:392,width:80,height:80},fill:C.amber,line:{style:"solid",fill:C.amber,width:0},rotation:180});
      textBox(slide,"Trade-off",515,475,250,45,{size:24,bold:true,color:C.ink,align:"center",ltr:true});
    } else if(t==="bars") {
      d.items.forEach((it,i)=>{const y=Y+i*102;textBox(slide,it.n,X+700,y,390,36,{size:20,bold:true,ltr:true});rect(slide,X+80,y+48,1010,24,C.paleNavy,C.paleNavy,true);rect(slide,X+80+1010*(1-it.v/100),y+48,1010*it.v/100,24,[C.teal,C.green,C.amber,C.coral][i],[C.teal,C.green,C.amber,C.coral][i],true);textBox(slide,`${it.v}`,X,y+38,65,40,{size:18,color:C.muted,align:"center",ltr:true});});
      pill(slide,"اولویت نسبی، نه امتیاز علمی",410,560,460,C.ink,C.paleNavy);
    } else if(t==="heatmap") {
      const cw=250,rh=78; d.cols.forEach((c,i)=>textBox(slide,c,X+350+i*cw,Y,cw-10,54,{size:19,bold:true,align:"center",ltr:true}));
      d.rows.forEach((r,ri)=>{textBox(slide,r.n,X,Y+70+ri*rh,320,58,{size:19,bold:true});r.v.forEach((v,ci)=>{const colors=[C.white,C.paleAmber,C.paleTeal,C.teal];rect(slide,X+350+ci*cw,Y+70+ri*rh,cw-10,58,colors[v],C.line,true);textBox(slide,["کم","کم","متوسط","زیاد"][v],X+350+ci*cw,Y+70+ri*rh,cw-10,58,{size:17,bold:v===3,color:v===3?C.white:C.ink,align:"center"});});});
    } else if(t==="matrix") {
      const cw=(W-300)/d.cols.length; textBox(slide,"معیار",X,Y,280,52,{size:18,bold:true});d.cols.forEach((c,i)=>textBox(slide,c,X+300+i*cw,Y,cw-8,52,{size:17,bold:true,align:"center",ltr:true}));
      d.rows.forEach((r,ri)=>{const y=Y+66+ri*88;rect(slide,X,y,W,70,ri%2?C.white:C.paleNavy,C.line,true);textBox(slide,r.n,X+14,y+8,268,54,{size:18,bold:true});r.v.forEach((v,ci)=>textBox(slide,v,X+300+ci*cw,y+8,cw-8,54,{size:16,align:"center",ltr:/^[A-Za-z—]/.test(v)}));});
    } else if(t==="decision") {
      card(slide,"Context",d.context,X,Y,W,120,C.amber);
      card(slide,"Decision",d.decision,X,Y+145,W,120,C.teal);
      card(slide,"Consequences",d.consequences.join("\n"),X,Y+290,W,185,C.coral);
    } else if(t==="contract") {
      rect(slide,X,Y,820,430,C.code,C.code,true);
      textBox(slide,d.code,X+28,Y+24,764,382,{size:d.code.includes("{")?21:24,color:"#EAF2F8",font:FONT.mono,ltr:true,align:"left",valign:"top"});
      card(slide,"Contract Rule",d.callout,X+850,Y,286,220,C.teal);
      pill(slide,"Versioned · Explicit · Minimal",X+850,Y+255,286,C.ink,C.paleNavy,true);
    } else if(t==="demo") {
      const a=d.danger?C.coral:C.green; const arr=[["Goal",d.goal],["Action",d.action],["Expected Signal",d.expected],["Reset / Recovery",d.reset]];
      arr.forEach((it,i)=>card(slide,it[0],it[1],X+(i%2)*570,Y+Math.floor(i/2)*192,548,170,i===2?a:(i===3?C.amber:C.teal)));
      pill(slide,d.tag,445,545,390,a,d.danger?C.paleCoral:C.paleTeal,true);
    } else if(t==="exit") {
      textBox(slide,"؟",85,165,190,210,{size:160,bold:true,color:C.amber,align:"center"});
      textBox(slide,d.question,300,180,820,150,{size:40,bold:true,font:FONT.display});
      textBox(slide,d.sub,500,370,620,70,{size:25,color:C.muted});
      pill(slide,"پاسخ: یک Gain + یک Cost",660,490,460,C.ink,C.paleAmber);
    } else if(t==="process") {
      const count=d.steps.length,gap=18,cw=(W-gap*(count-1))/count; const shapes=[];
      d.steps.forEach((s,i)=>shapes.push(node(slide,s,X+i*(cw+gap),Y+150,cw,130,[C.teal,C.green,C.amber,C.coral,C.teal,C.green][i%6])));
      for(let i=0;i<shapes.length-1;i++) arrow(slide,shapes[i],shapes[i+1],false,"right","left");
    } else if(t==="architecture") {
      const count=d.nodes.length; const coords=count===3?[[100,190],[470,190],[840,190]]:count===4?[[70,170],[390,170],[710,100],[710,260]]:[[40,180],[260,180],[500,80],[500,280],[820,180]];
      const shapes=[]; d.nodes.forEach((lab,i)=>{const [cx,cy]=coords[i];shapes.push(node(slide,lab,X+cx,Y+cy,220,92,i===0?C.ink:(i===1?C.teal:(i===2?C.green:(i===3?C.amber:C.coral)))));});
      d.edges.forEach((e,i)=>arrow(slide,shapes[e[0]],shapes[e[1]],d.dangerEdge===i));
      if(d.sub) textBox(slide,d.sub,X,Y+5,W,60,{size:22,color:C.muted,align:"center"});
    } else if(t==="sequence") {
      const actors=[]; const gap=120,cw=(W-gap*2)/3; d.actors.forEach((a,i)=>{actors.push(node(slide,a,X+i*(cw+gap),Y,cw,66,i===0?C.ink:(i===1?C.teal:C.coral)));slide.shapes.add({geometry:"rect",position:{left:X+i*(cw+gap)+cw/2,top:Y+74,width:2,height:330},fill:C.line,line:{style:"solid",fill:C.line,width:0}});});
      d.messages.forEach((m,i)=>{const from=i===2?2:0,to=i===2?0:(i===1?2:1);const y=Y+118+i*96;slide.shapes.add({geometry:"rightArrow",position:{left:Math.min(X+from*(cw+gap)+cw/2,X+to*(cw+gap)+cw/2),top:y,width:Math.abs(to-from)*(cw+gap),height:28},fill:(d.danger&&i===1)?C.coral:C.teal,line:{style:"solid",fill:"none",width:0},rotation:to<from?180:0});textBox(slide,m,X+240,y-40,656,34,{size:17,color:(d.danger&&i===1)?C.coral:C.ink,align:"center"});});
    } else if(t==="timelineCompare") {
      [[d.topTitle,d.top,C.coral,Y+40],[d.bottomTitle,d.bottom,C.teal,Y+270]].forEach(([title,arr,color,y])=>{textBox(slide,title,X,y,160,48,{size:23,bold:true,color,ltr:true});const shapes=[];arr.forEach((s,i)=>shapes.push(node(slide,s,X+190+i*300,y,250,76,color)));for(let i=0;i<shapes.length-1;i++)arrow(slide,shapes[i],shapes[i+1]);});
    } else if(t==="compare3" || t==="threeFailures") {
      const cw=(W-36)/3;d.items.forEach((it,i)=>card(slide,it.h,it.p,X+i*(cw+18),Y+70,cw,300,[C.coral,C.amber,C.green][i]));
    } else if(t==="radial") {
      const center=node(slide,d.center,500,300,280,100,C.ink); const pts=[[100,150],[500,100],[900,150],[250,430],[750,430]];d.items.forEach((it,i)=>{const [px,py]=pts[i];const s=node(slide,it,px,py,250,70,[C.teal,C.green,C.amber,C.coral,C.teal][i]);arrow(slide,center,s,false,py<300?"top":"bottom",py<300?"bottom":"top");});
    } else if(t==="budget") {
      textBox(slide,d.total,70,230,250,120,{size:74,bold:true,color:C.ink,align:"center",ltr:true});
      let x=370; const total=d.parts.reduce((a,b)=>a+b.v,0);d.parts.forEach((p,i)=>{const w=740*p.v/total;rect(slide,x,255,w,72,[C.ink,C.teal,C.green,C.amber][i],[C.ink,C.teal,C.green,C.amber][i],false);textBox(slide,p.n,x,345,w,42,{size:16,bold:true,align:"center",ltr:true});textBox(slide,`${p.v}ms`,x,270,w,40,{size:17,bold:true,color:C.white,align:"center",ltr:true});x+=w;});
    } else if(t==="checklist") {
      d.items.forEach((it,i)=>{slide.shapes.add({geometry:"ellipse",position:{left:120,top:160+i*76,width:46,height:46},fill:C.green,line:{style:"solid",fill:C.green,width:0}});textBox(slide,"✓",120,158+i*76,46,46,{size:24,bold:true,color:C.white,align:"center"});textBox(slide,it,190,154+i*76,620,56,{size:22,bold:true});});
      card(slide,"هشدار",d.warning,830,220,300,210,C.coral);
    } else if(t==="state") {
      const shapes=[];d.states.forEach((s,i)=>shapes.push(node(slide,s,120+i*360,260,260,110,[C.green,C.coral,C.amber][i])));arrow(slide,shapes[0],shapes[1]);arrow(slide,shapes[1],shapes[2]);arrow(slide,shapes[2],shapes[0],false,"top","top");
    } else if(t==="routeMap") {
      const cols=["Route","قبل","بعد"],xs=[X,X+520,X+790],ws=[500,250,346];cols.forEach((c,i)=>textBox(slide,c,xs[i],Y,ws[i],54,{size:19,bold:true,align:"center",ltr:true}));d.rows.forEach((r,ri)=>{const y=Y+68+ri*84;rect(slide,X,y,W,66,ri%2?C.white:C.paleNavy,C.line,true);[r.r,r.b,r.a].forEach((v,i)=>textBox(slide,v,xs[i],y+7,ws[i],52,{size:17,bold:i===2,color:i===2?C.teal:C.ink,align:i===0?"left":"center",ltr:true}));});
    } else if(t==="layers") {
      d.items.forEach((it,i)=>{const w=920-i*130,x=X+(W-w)/2,y=Y+20+i*84;rect(slide,x,y,w,68,[C.paleNavy,C.paleTeal,"#E6F5EB",C.paleAmber,C.paleCoral][i],[C.line,C.teal,C.green,C.amber,C.coral][i],true);textBox(slide,`${it.h} · ${it.p}`,x+20,y+5,w-40,56,{size:19,bold:true,align:"center",ltr:true});});
    } else if(t==="trace") {
      const shapes=[];d.items.forEach((it,i)=>shapes.push(node(slide,it,90+i*205,250,180,90,[C.ink,C.teal,C.green,C.amber,C.coral][i])));for(let i=0;i<shapes.length-1;i++)arrow(slide,shapes[i],shapes[i+1]);
      textBox(slide,"یک درخواست؛ یک داستان قابل دنبال‌کردن",300,430,680,60,{size:25,bold:true,color:C.teal,align:"center"});
    } else if(t==="final") {
      const shapes=[];d.steps.forEach((s,i)=>shapes.push(node(slide,s,105+i*375,190,330,155,[C.teal,C.amber,C.green][i])));for(let i=0;i<2;i++)arrow(slide,shapes[i],shapes[i+1]);pill(slide,d.footerCallout,170,500,940,C.ink,C.paleNavy,true);
    }
  }
  const note=[`هدف: ${d.note || "پیام اصلی اسلاید را تثبیت کنید."}`,"","Transition: سؤال یا مسئله‌ی اسلاید بعد را قبل از reveal مطرح کنید."];
  if(d.sources?.length){note.push("","[Sources]",...d.sources.map(x=>`- ${x}`),"[/Sources]");}
  slide.speakerNotes.textFrame.setText(note.join("\n"));
  slide.speakerNotes.setVisible(true);
  return slide;
}

async function writeBlob(path,blob){await fs.writeFile(path,new Uint8Array(await blob.arrayBuffer()));}

async function fixRtlPptx(path) {
  const zip = await JSZip.loadAsync(await fs.readFile(path));
  const slideNames = Object.keys(zip.files).filter(n => /^ppt\/slides\/slide\d+\.xml$/.test(n));
  const hasPersian = /[\u0600-\u06FF]/;
  for (const name of slideNames) {
    let xml = await zip.file(name).async("string");
    xml = xml.replace(/<a:p(?:\s[^>]*)?>[\s\S]*?<\/a:p>/g, block => {
      if (!hasPersian.test(block)) return block;
      if (/<a:pPr\b/.test(block)) {
        return block.replace(/<a:pPr\b([^>]*)>/, (m, attrs) => /\brtl=/.test(attrs) ? m : `<a:pPr${attrs} rtl="1">`);
      }
      return block.replace(/<a:p([^>]*)>/, `<a:p$1><a:pPr rtl="1" algn="r"/>`);
    });
    zip.file(name, xml);
  }
  await fs.writeFile(path, await zip.generateAsync({type:"nodebuffer",compression:"DEFLATE",compressionOptions:{level:6}}));
}

async function main(){
  if(slides.length!==72) throw new Error(`Expected 72 slides, got ${slides.length}`);
  await fs.mkdir(QA,{recursive:true});
  await fs.mkdir(new URL(".",`file://${OUT}`).pathname,{recursive:true});
  const p=Presentation.create({slideSize:{width:1280,height:720}});
  slides.forEach((d,i)=>renderSlide(p,d,i+1));
  if(process.env.FAST!=="1") {
    for(const [i,s] of p.slides.items.entries()){
      const stem=`slide-${String(i+1).padStart(2,"0")}`;
      await writeBlob(`${QA}/${stem}.png`,await p.export({slide:s,format:"png",scale:1}));
      const layout=await s.export({format:"layout"});
      await fs.writeFile(`${QA}/${stem}.layout.json`,await layout.text());
    }
    await writeBlob(`${QA}/deck-montage.webp`,await p.export({format:"webp",montage:true,scale:1}));
  }
  const pptx=await PresentationFile.exportPptx(p);
  await pptx.save(OUT);
  await fixRtlPptx(OUT);
  console.log(JSON.stringify({slides:slides.length,output:OUT,qa:QA},null,2));
}

main().catch(e=>{console.error(e);process.exitCode=1;});
