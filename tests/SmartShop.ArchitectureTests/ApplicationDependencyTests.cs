namespace SmartShop.ArchitectureTests;

public sealed class ApplicationDependencyTests
{
    [Theory]
    [MemberData(nameof(ArchitectureTestData.ApplicationProjects), MemberType = typeof(ArchitectureTestData))]
    public void Application_projects_must_not_depend_on_infrastructure_or_endpoints(ProjectReferences applicationProject)
    {
        var forbiddenReferences = new[]
        {
            "SmartShop.Api",
            "Microsoft.AspNetCore",
            "Microsoft.Data.SqlClient",
            "Microsoft.EntityFrameworkCore",
            "OpenAI",
            "Qdrant",
            "Qdrant.Client",
            "System.Data.SqlClient"
        }
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Infra"))
        .Concat(ArchitectureTestData.LayerProjectPrefixes("Endpoints"))
        .ToArray();

        ArchitectureTestData.AssertDoesNotReference(applicationProject, forbiddenReferences);
    }
}
