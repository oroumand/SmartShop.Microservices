namespace SmartShop.Loyalty.Core.Application.Accounts;

public interface ILoyaltyAccountQueryService
{
    Task<LoyaltyAccountDto> GetAccountAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LoyaltyTransactionDto>> GetTransactionsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}
