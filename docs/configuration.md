# راهنمای تنظیمات SmartShop

این سند کلیدهای configuration استفاده‌شده در پروژه SmartShop را توضیح می‌دهد. مقدارهای واقعی و حساس باید از طریق environment variable یا user secrets تامین شوند و داخل ریپازیتوری commit نشوند.

## کلیدهای اصلی

| کلید configuration | Environment variable | توضیح |
| --- | --- | --- |
| `ConnectionStrings:SmartShopDb` | `ConnectionStrings__SmartShopDb` | دیتابیس ماژول‌های باقی‌مانده در `SmartShop.Api` |
| `ConnectionStrings:LoyaltyDb` | `ConnectionStrings__LoyaltyDb` | دیتابیس تحت مالکیت Loyalty |
| `ConnectionStrings:PaymentsDb` | `ConnectionStrings__PaymentsDb` | دیتابیس تحت مالکیت Payments |
| `Services:Ordering:BaseUrl` | `Services__Ordering__BaseUrl` | آدرس داخلی Ordering برای query موردنیاز Payments |
| `RabbitMq:HostName` | `RabbitMq__HostName` | hostname سرویس RabbitMQ |
| `RabbitMq:UserName` | `RabbitMq__UserName` | نام کاربری RabbitMQ |
| `RabbitMq:Password` | `RabbitMq__Password` | رمز RabbitMQ؛ فقط از secret store تأمین شود |
| `AiSearch:OpenAI:ApiKey` | `AiSearch__OpenAI__ApiKey` | کلید OpenAI برای تولید embedding |
| `AiSearch:OpenAI:BaseUrl` | `AiSearch__OpenAI__BaseUrl` | آدرس پایه OpenAI API |
| `AiSearch:OpenAI:Model` | `AiSearch__OpenAI__Model` | مدل embedding |
| `AiSearch:OpenAI:Dimensions` | `AiSearch__OpenAI__Dimensions` | تعداد dimensionهای embedding |
| `AiSearch:Qdrant:BaseUrl` | `AiSearch__Qdrant__BaseUrl` | آدرس Qdrant |
| `AiSearch:Qdrant:CollectionName` | `AiSearch__Qdrant__CollectionName` | نام collection محصولات در Qdrant |
| `AiSearch:Qdrant:VectorSize` | `AiSearch__Qdrant__VectorSize` | اندازه vector در Qdrant |
| `AiSearch:Qdrant:Distance` | `AiSearch__Qdrant__Distance` | معیار فاصله، مثلا `Cosine` |
| `AiSearch:Qdrant:ApiKey` | `AiSearch__Qdrant__ApiKey` | کلید Qdrant در صورت استفاده از Qdrant امن‌شده یا hosted |

## نمونه environment variableها

```text
ConnectionStrings__SmartShopDb=Server=localhost;Database=SmartShop;Trusted_Connection=True;TrustServerCertificate=True
ConnectionStrings__LoyaltyDb=Server=localhost;Database=SmartShopLoyalty;Trusted_Connection=True;TrustServerCertificate=True
ConnectionStrings__PaymentsDb=Server=localhost;Database=SmartShopPayments;Trusted_Connection=True;TrustServerCertificate=True
Services__Ordering__BaseUrl=http://localhost:5217
RabbitMq__HostName=localhost
AiSearch__OpenAI__ApiKey=YOUR_OPENAI_API_KEY
AiSearch__OpenAI__BaseUrl=https://api.openai.com
AiSearch__OpenAI__Model=text-embedding-3-small
AiSearch__OpenAI__Dimensions=1536
AiSearch__Qdrant__BaseUrl=http://localhost:6333
AiSearch__Qdrant__CollectionName=smartshop-products
AiSearch__Qdrant__VectorSize=1536
AiSearch__Qdrant__Distance=Cosine
AiSearch__Qdrant__ApiKey=
```

## نکات امنیتی

فایل‌های `appsettings.json` باید فقط defaultهای امن یا placeholder داشته باشند. مقدارهای واقعی مثل OpenAI API key، رمز SQL Server یا کلید سرویس‌های hosted نباید داخل کد commit شوند.

برای توسعه local می‌توانید از environment variableها یا user secrets استفاده کنید. فایل `.env.example` فقط template است و نباید شامل secret واقعی باشد.

هیچ‌وقت OpenAI API key واقعی را commit نکنید.
