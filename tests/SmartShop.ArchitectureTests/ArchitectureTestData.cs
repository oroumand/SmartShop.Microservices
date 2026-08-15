using System.Xml.Linq;

namespace SmartShop.ArchitectureTests;

internal static class ArchitectureTestData
{
    public static readonly string[] Modules =
    [
        "Catalog",
        "Ordering",
        "Payments",
        "AiSearch"
    ];

    public static readonly string[] ModuleProjectPrefixes =
    [
        "SmartShop.Catalog",
        "SmartShop.Ordering",
        "SmartShop.Payments",
        "SmartShop.AiSearch"
    ];

    public static IEnumerable<object[]> DomainProjects()
    {
        foreach (var module in Modules)
        {
            yield return [LoadProject($"SmartShop.{module}.Core.Domain")];
        }
    }

    public static IEnumerable<object[]> ApplicationProjects()
    {
        foreach (var module in Modules)
        {
            yield return [LoadProject($"SmartShop.{module}.Core.Application")];
        }
    }

    public static IEnumerable<object[]> EndpointProjects()
    {
        foreach (var module in Modules)
        {
            yield return [LoadProject($"SmartShop.{module}.Endpoints")];
        }
    }

    public static IEnumerable<object[]> ModuleProjects()
    {
        foreach (var projectName in ModuleProjectNames())
        {
            yield return [LoadProject(projectName), ModuleNameFrom(projectName)];
        }
    }

    public static ProjectReferences LoadProject(string projectName)
    {
        var projectFile = Directory
            .GetFiles(RepositoryRoot(), projectName + ".csproj", SearchOption.AllDirectories)
            .Single();

        var document = XDocument.Load(projectFile);

        return new ProjectReferences(
            projectName,
            ProjectReferencesFrom(document),
            PackageReferencesFrom(document),
            FrameworkReferencesFrom(document));
    }

    public static void AssertDoesNotReference(ProjectReferences project, params string[] forbiddenReferencePrefixes)
    {
        var violations = project
            .AllReferences()
            .Where(reference => forbiddenReferencePrefixes.Any(forbidden =>
                reference.Equals(forbidden, StringComparison.Ordinal) ||
                reference.StartsWith(forbidden + ".", StringComparison.Ordinal)))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"{project.Name} must not reference: {string.Join(", ", violations)}");
    }

    public static string[] LayerProjectPrefixes(string layerName)
    {
        return Modules
            .Select(module => $"SmartShop.{module}.{layerName}")
            .ToArray();
    }

    public static string[] OtherModulePrefixes(string currentModule)
    {
        return Modules
            .Where(module => !module.Equals(currentModule, StringComparison.Ordinal))
            .Select(module => $"SmartShop.{module}")
            .ToArray();
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "SmartShop.sln")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Could not find the repository root.");
        }

        return directory.FullName;
    }

    private static string[] ProjectReferencesFrom(XDocument document)
    {
        return document
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => Path.GetFileNameWithoutExtension(include!))
            .ToArray();
    }

    private static string[] PackageReferencesFrom(XDocument document)
    {
        return document
            .Descendants("PackageReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => include!)
            .ToArray();
    }

    private static string[] FrameworkReferencesFrom(XDocument document)
    {
        return document
            .Descendants("FrameworkReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => include!)
            .ToArray();
    }

    private static IEnumerable<string> ModuleProjectNames()
    {
        foreach (var module in Modules)
        {
            yield return $"SmartShop.{module}.Core.Domain";
            yield return $"SmartShop.{module}.Core.Application";
            yield return $"SmartShop.{module}.Endpoints";

            if (module == "AiSearch")
            {
                yield return "SmartShop.AiSearch.Infra.Data";
                yield return "SmartShop.AiSearch.Infra.OpenAI";
                yield return "SmartShop.AiSearch.Infra.Qdrant";
            }
            else
            {
                yield return $"SmartShop.{module}.Infra.Data";
            }
        }
    }

    private static string ModuleNameFrom(string projectName)
    {
        return projectName.Split('.')[1];
    }
}

public sealed record ProjectReferences(
    string Name,
    string[] ProjectReferenceNames,
    string[] PackageReferenceNames,
    string[] FrameworkReferenceNames)
{
    public string[] AllReferences()
    {
        return ProjectReferenceNames
            .Concat(PackageReferenceNames)
            .Concat(FrameworkReferenceNames)
            .ToArray();
    }
}
