using Microsoft.EntityFrameworkCore;
using SmartShop.Loyalty.Core.Application.Accounts;

namespace SmartShop.Loyalty.Infra.Data.Accounts;

public sealed class EfLoyaltyAccountQueryService(LoyaltyDbContext dbContext)
    : ILoyaltyAccountQueryService
{
    public async Task<LoyaltyAccountDto> GetAccountAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        ValidateCustomerId(customerId);

        var account = await dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(
                account => account.CustomerId == customerId,
                cancellationToken);

        return account is null
            ? new LoyaltyAccountDto(customerId, 0, null)
            : new LoyaltyAccountDto(
                account.CustomerId,
                account.Balance,
                account.CreatedAtUtc);
    }

    public async Task<IReadOnlyList<LoyaltyTransactionDto>> GetTransactionsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        ValidateCustomerId(customerId);

        return await dbContext.Transactions
            .AsNoTracking()
            .Where(transaction =>
                dbContext.Accounts.Any(account =>
                    account.Id == transaction.LoyaltyAccountId &&
                    account.CustomerId == customerId))
            .OrderByDescending(transaction => transaction.OccurredAtUtc)
            .Select(transaction => new LoyaltyTransactionDto(
                transaction.Id,
                transaction.SourcePaymentId,
                transaction.Points,
                transaction.OccurredAtUtc,
                transaction.Description))
            .ToListAsync(cancellationToken);
    }

    private static void ValidateCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }
    }
}
