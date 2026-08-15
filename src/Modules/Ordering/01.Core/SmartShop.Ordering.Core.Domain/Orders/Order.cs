namespace SmartShop.Ordering.Core.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
    }

    public Order(
        Guid customerId,
        string customerName,
        string customerEmail,
        IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }

        CustomerId = customerId;
        CustomerName = RequireValue(customerName, nameof(customerName));
        CustomerEmail = RequireValue(customerEmail, nameof(customerEmail));

        var orderItems = items.ToList();

        if (orderItems.Count == 0)
        {
            throw new ArgumentException("At least one order item is required.", nameof(items));
        }

        Id = Guid.NewGuid();
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        _items.AddRange(orderItems);
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public string CustomerName { get; private set; }

    public string CustomerEmail { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(item => item.LineTotal);

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be marked as paid.");
        }

        Status = OrderStatus.Paid;
    }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
