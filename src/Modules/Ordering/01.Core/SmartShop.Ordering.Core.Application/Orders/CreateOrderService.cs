using SmartShop.ModuleContracts.Catalog;
using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Core.Application.Orders;

public sealed class CreateOrderService(
    ICatalogProductLookup catalogProductLookup,
    IOrderRepository orderRepository) : IOrderCommandService
{
    public async Task<OrderDto> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            throw new ArgumentException("Customer email is required.", nameof(request));
        }

        if (request.Items.Count == 0)
        {
            throw new ArgumentException("At least one order item is required.", nameof(request));
        }

        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request), "Quantity must be positive.");
            }

            var product = await catalogProductLookup.GetProductAsync(
                item.ProductId,
                cancellationToken);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product '{item.ProductId}' was not found or is not active.");
            }

            orderItems.Add(new OrderItem(
                product.ProductId,
                product.Name,
                product.Price,
                item.Quantity));
        }

        var order = new Order(
            request.CustomerId,
            request.CustomerName,
            request.CustomerEmail,
            orderItems);

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.CustomerName,
            order.CustomerEmail,
            order.Status.ToString(),
            order.CreatedAtUtc,
            order.TotalAmount,
            order.Items
                .Select(item => new OrderItemDto(
                    item.ProductId,
                    item.ProductName,
                    item.UnitPrice,
                    item.Quantity,
                    item.LineTotal))
                .ToList());
}
