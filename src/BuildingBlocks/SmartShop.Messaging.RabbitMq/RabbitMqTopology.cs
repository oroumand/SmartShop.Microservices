namespace SmartShop.Messaging.RabbitMq;

public static class RabbitMqTopology
{
    public const string PaymentSucceededRoutingKey = "payments.payment-succeeded.v1";

    public const string LoyaltyPaymentSucceededQueue = "loyalty.payment-succeeded.v1";

    public const string OrderingPaymentSucceededQueue = "ordering.payment-succeeded.v1";
}
