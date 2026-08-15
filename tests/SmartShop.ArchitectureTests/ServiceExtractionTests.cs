namespace SmartShop.ArchitectureTests;

public sealed class ServiceExtractionTests
{
    [Fact]
    public void Modular_monolith_must_not_reference_extracted_payments_projects()
    {
        var modularMonolith = ArchitectureTestData.LoadProject("SmartShop.Api");

        ArchitectureTestData.AssertDoesNotReference(
            modularMonolith,
            "SmartShop.Payments");
    }

    [Fact]
    public void Gateway_must_not_reference_business_implementation_projects()
    {
        var gateway = ArchitectureTestData.LoadProject("SmartShop.Gateway");

        ArchitectureTestData.AssertDoesNotReference(
            gateway,
            "SmartShop.Catalog",
            "SmartShop.Ordering",
            "SmartShop.Payments",
            "SmartShop.Loyalty",
            "SmartShop.AiSearch");
    }
}
