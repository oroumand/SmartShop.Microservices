namespace SmartShop.ModuleContracts.Ordering;

public sealed record OrderPaymentInfo(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Status,
    bool IsPayable);
