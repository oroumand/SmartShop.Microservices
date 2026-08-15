using SmartShop.ModuleContracts.Ordering;
using SmartShop.IntegrationEvents;
using SmartShop.IntegrationEvents.Payments;
using SmartShop.Payments.Core.Domain.Payments;

namespace SmartShop.Payments.Core.Application.Payments;

public sealed class PayOrderService(
    IOrderingPaymentContract orderingPaymentContract,
    IPaymentRepository paymentRepository,
    IIntegrationEventOutbox outbox) : IPaymentCommandService
{
    public async Task<PaymentDto> PayOrderAsync(
        PayOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.OrderId == Guid.Empty)
        {
            throw new ArgumentException("Order id is required.", nameof(request));
        }

        if (!Enum.TryParse<PaymentMethod>(request.Method, ignoreCase: true, out var method))
        {
            throw new ArgumentException("Payment method is invalid.", nameof(request));
        }

        var order = await orderingPaymentContract.GetOrderForPaymentAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            throw new InvalidOperationException($"Order '{request.OrderId}' was not found.");
        }

        if (!order.IsPayable)
        {
            throw new InvalidOperationException(
                $"Order '{request.OrderId}' is not payable. Current status is '{order.Status}'.");
        }

        var payment = Payment.CreateRequest(
            order.OrderId,
            order.TotalAmount,
            method);

        payment.MarkAsSucceeded();

        await paymentRepository.AddAsync(payment, cancellationToken);
        await orderingPaymentContract.MarkOrderAsPaidAsync(order.OrderId, payment.Id, cancellationToken);
        outbox.Enqueue(
            RabbitMqEventNames.PaymentSucceededV1,
            new PaymentSucceededV1(
                Guid.NewGuid(),
                payment.Id,
                order.OrderId,
                order.CustomerId,
                payment.Amount,
                payment.PaidAtUtc!.Value));
        await paymentRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.CreatedAtUtc,
            payment.PaidAtUtc);
}

internal static class RabbitMqEventNames
{
    public const string PaymentSucceededV1 = "payments.payment-succeeded.v1";
}
