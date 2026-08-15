namespace SmartShop.ArchitectureTests;

public sealed class CrossModuleDependencyTests
{
    [Theory]
    [MemberData(nameof(ArchitectureTestData.ModuleProjects), MemberType = typeof(ArchitectureTestData))]
    public void Modules_must_not_reference_other_modules_internal_projects(
        ProjectReferences moduleProject,
        string moduleName)
    {
        var forbiddenReferences = ArchitectureTestData.OtherModulePrefixes(moduleName);

        ArchitectureTestData.AssertDoesNotReference(moduleProject, forbiddenReferences);
    }
}
