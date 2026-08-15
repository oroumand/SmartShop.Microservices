using Microsoft.EntityFrameworkCore;
using SmartShop.Catalog.Core.Domain.Products;

namespace SmartShop.Catalog.Infra.Data.Seed;

public static class CatalogSeedData
{
    public static async Task SeedAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Products.AddRange(
            Product.Create("Dell XPS 13 Laptop", "Compact productivity laptop with a bright 13-inch display.", "Laptop", 1299.00m),
            Product.Create("Lenovo ThinkPad X1 Carbon", "Lightweight business laptop with a durable keyboard.", "Laptop", 1599.00m),
            Product.Create("Apple iPhone 16", "Modern smartphone with excellent camera performance.", "Phone", 899.00m),
            Product.Create("Samsung Galaxy S25", "Android phone with a vivid OLED display.", "Phone", 849.00m),
            Product.Create("Sony WH-1000XM5", "Wireless noise-cancelling over-ear headphones.", "Headphone", 349.00m),
            Product.Create("Dell UltraSharp 27 Monitor", "27-inch 4K monitor for clear office and creative work.", "Monitor", 549.00m),
            Product.Create("Keychron K8 Keyboard", "Wireless mechanical keyboard with tactile switches.", "Keyboard", 109.00m),
            Product.Create("Logitech MX Master 3S", "Ergonomic wireless mouse for productive workflows.", "Mouse", 99.00m),
            Product.Create("Canon EOS R50 Camera", "Mirrorless camera suited to travel and content creation.", "Camera", 679.00m),
            Product.Create("LG UltraGear 32 Monitor", "High refresh rate monitor designed for gaming.", "Monitor", 429.00m));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
