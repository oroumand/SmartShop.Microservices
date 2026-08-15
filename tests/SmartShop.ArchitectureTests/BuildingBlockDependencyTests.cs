namespace SmartShop.ArchitectureTests;

public sealed class BuildingBlockDependencyTests
{
    [Fact]
    public void SharedKernel_must_not_reference_modules_api_or_framework_infrastructure()
    {
        var sharedKernelProject = ArchitectureTestData.LoadProject("SmartShop.SharedKernel");

        var forbiddenReferences = new[]
        {
            "SmartShop.Api",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore"
        }
        .Concat(ArchitectureTestData.ModuleProjectPrefixes)
        .ToArray();

        ArchitectureTestData.AssertDoesNotReference(sharedKernelProject, forbiddenReferences);
    }

    [Fact]
    public void ModuleContracts_must_not_reference_modules_api_or_framework_infrastructure()
    {
        var moduleContractsProject = ArchitectureTestData.LoadProject("SmartShop.ModuleContracts");

        var forbiddenReferences = new[]
        {
            "SmartShop.Api",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore"
        }
        .Concat(ArchitectureTestData.ModuleProjectPrefixes)
        .ToArray();

        ArchitectureTestData.AssertDoesNotReference(moduleContractsProject, forbiddenReferences);
    }
}
