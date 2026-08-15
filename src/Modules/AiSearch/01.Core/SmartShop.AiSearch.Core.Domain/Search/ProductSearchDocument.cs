namespace SmartShop.AiSearch.Core.Domain.Search;

public sealed class ProductSearchDocument
{
    public ProductSearchDocument(
        Guid productId,
        string name,
        string description,
        string category,
        decimal price)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Category = category;
        Price = price;
        TextForEmbedding = BuildTextForEmbedding();
    }

    public Guid ProductId { get; }

    public string Name { get; }

    public string Description { get; }

    public string Category { get; }

    public decimal Price { get; }

    public string TextForEmbedding { get; }

    private string BuildTextForEmbedding() =>
        $"Name: {Name}\nCategory: {Category}\nDescription: {Description}\nPrice: {Price}";
}
