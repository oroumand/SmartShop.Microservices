using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.UnitTests;

public sealed class OrderPaymentTests
{
    [Fact]
    public void Applying_the_same_payment_twice_has_one_business_effect()
    {
        var order = CreatePendingOrder();
        var paymentId = Guid.NewGuid();

        var firstDeliveryChangedState = order.ApplySuccessfulPayment(paymentId);
        var duplicateDeliveryChangedState = order.ApplySuccessfulPayment(paymentId);

        Assert.True(firstDeliveryChangedState);
        Assert.False(duplicateDeliveryChangedState);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(paymentId, order.PaymentId);
    }

    [Fact]
    public void A_different_payment_cannot_pay_an_already_paid_order()
    {
        var order = CreatePendingOrder();
        order.ApplySuccessfulPayment(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() =>
            order.ApplySuccessfulPayment(Guid.NewGuid()));
    }

    private static Order CreatePendingOrder() =>
        new(
            Guid.NewGuid(),
            "Test Customer",
            "test@smartshop.local",
            [new OrderItem(Guid.NewGuid(), "Test Product", 1_250m, 1)]);
}
