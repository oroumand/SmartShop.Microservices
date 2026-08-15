namespace SmartShop.Loyalty.Core.Application.Accounts;

public sealed record LoyaltyAccountDto(
    Guid CustomerId,
    int Balance,
    DateTime? MemberSinceUtc);
