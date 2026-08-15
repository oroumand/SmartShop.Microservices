using Microsoft.EntityFrameworkCore;
using SmartShop.Loyalty.Core.Application.Accounts;
using SmartShop.Loyalty.Core.Domain.Accounts;
using SmartShop.Loyalty.Infra.Data.Inbox;

namespace SmartShop.Loyalty.Infra.Data.Accounts;

public sealed class EfEarnLoyaltyPointsService(LoyaltyDbContext dbContext)
    : IEarnLoyaltyPointsService
{
    public async Task EarnForPaymentAsync(
        EarnPointsForPayment request,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.ProcessedMessages.AnyAsync(
                message => message.Id == request.MessageId,
                cancellationToken))
        {
            return;
        }

        var paymentWasAlreadyApplied = await dbContext.Transactions.AnyAsync(
            transaction => transaction.SourcePaymentId == request.PaymentId,
            cancellationToken);

        if (paymentWasAlreadyApplied)
        {
            dbContext.ProcessedMessages.Add(
                new ProcessedMessage(request.MessageId, nameof(EarnPointsForPayment)));
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var account = await dbContext.Accounts
            .SingleOrDefaultAsync(
                account => account.CustomerId == request.CustomerId,
                cancellationToken);

        if (account is null)
        {
            account = new LoyaltyAccount(request.CustomerId);
            await dbContext.Accounts.AddAsync(account, cancellationToken);
        }

        account.EarnForPayment(
            request.PaymentId,
            request.PaidAmount,
            request.OccurredAtUtc);

        dbContext.ProcessedMessages.Add(
            new ProcessedMessage(request.MessageId, nameof(EarnPointsForPayment)));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
