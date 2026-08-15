namespace SmartShop.ArchitectureTests;

public sealed class EndpointDependencyTests
{
    [Theory]
    [MemberData(nameof(ArchitectureTestData.EndpointProjects), MemberType = typeof(ArchitectureTestData))]
    public void Endpoints_projects_must_not_depend_on_infrastructure(ProjectReferences endpointProject)
    {
        var forbiddenReferences = new[]
        {
            "Microsoft.EntityFrameworkCore"
        }
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Infra"))
        .ToArray();

        ArchitectureTestData.AssertDoesNotReference(endpointProject, forbiddenReferences);
    }
}
