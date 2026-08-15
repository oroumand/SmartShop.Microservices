namespace SmartShop.ArchitectureTests;

public sealed class DomainDependencyTests
{
    [Theory]
    [MemberData(nameof(ArchitectureTestData.DomainProjects), MemberType = typeof(ArchitectureTestData))]
    public void Domain_projects_must_be_independent(ProjectReferences domainProject)
    {
        var forbiddenReferences = new[]
        {
            "SmartShop.Api",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore"
        }
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Core.Application"))
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Infra"))
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Endpoints"))
        .ToArray();

        ArchitectureTestData.AssertDoesNotReference(domainProject, forbiddenReferences);
    }
}
