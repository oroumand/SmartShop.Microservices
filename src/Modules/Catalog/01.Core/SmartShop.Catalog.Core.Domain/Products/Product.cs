namespace SmartShop.Catalog.Core.Domain.Products;

public sealed class Product
{
    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        Category = string.Empty;
    }

    public Product(
        Guid id,
        string name,
        string description,
        string category,
        decimal price,
        bool isActive = true)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product id is required.", nameof(id));
        }

        Id = id;
        Name = RequireValue(name, nameof(name));
        Description = RequireValue(description, nameof(description));
        Category = RequireValue(category, nameof(category));

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");
        }

        Price = price;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string Category { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public static Product Create(
        string name,
        string description,
        string category,
        decimal price) =>
        new(Guid.NewGuid(), name, description, category, price);

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
