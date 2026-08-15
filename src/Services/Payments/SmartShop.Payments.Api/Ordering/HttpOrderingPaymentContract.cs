using System.Net;
using System.Net.Http.Json;
using SmartShop.ModuleContracts.Ordering;

namespace SmartShop.Payments.Api.Ordering;

public sealed class HttpOrderingPaymentContract(HttpClient httpClient)
    : IOrderingPaymentContract
{
    public async Task<OrderPaymentInfo?> GetOrderForPaymentAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"/internal/orders/{orderId}/payment-info",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderPaymentInfo>(
            cancellationToken);
    }
}
