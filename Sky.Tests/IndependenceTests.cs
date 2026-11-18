using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The library stands on .NET alone, so any engine takes it. Beside <see cref="ReferenceTests"/>,
/// which refuses the names of engines, windows and graphics APIs, these allow .NET's own assemblies
/// alone, no call into native code and no reference in the project file.
/// </summary>
public class IndependenceTests
{
    private static readonly Assembly Library = typeof(SkyClock).Assembly;

    [Fact]
    public void The_Library_References_Only_Dotnets_Own_Assemblies()
    {
        var foreign = Library.GetReferencedAssemblies()
            .Select(a => a.Name ?? "")
            .Where(n => !n.StartsWith("System", StringComparison.Ordinal) && n is not "netstandard" and not "mscorlib")
            .ToList();
        Assert.True(foreign.Count == 0, $"the library references {string.Join(", ", foreign)}");
    }

    [Fact]
    public void The_Library_Calls_No_Native_Code()
    {
        // A window and a graphics API are reached from .NET through a call into a native library,
        // which a method marked to be imported from one makes.
        const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var native = Library.GetTypes()
            .SelectMany(t => t.GetMethods(all))
            .Where(m => m.Attributes.HasFlag(MethodAttributes.PinvokeImpl))
            .Select(m => $"{m.DeclaringType?.Name}.{m.Name}")
            .ToList();
        Assert.True(native.Count == 0, $"the library calls native code from {string.Join(", ", native)}");
    }

    [Fact]
    public void The_Librarys_Project_Takes_No_Package_No_Project_And_No_Assembly()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Sky.slnx")))
            root = root.Parent;
        Assert.NotNull(root);
        foreach (var file in new[] { Path.Combine("Sky", "Sky.csproj"), "Directory.Build.props" })
        {
            var taken = XDocument.Load(Path.Combine(root.FullName, file)).Descendants()
                .Where(e => e.Name.LocalName is "PackageReference" or "ProjectReference" or "Reference" or "FrameworkReference")
                .Select(e => $"{e.Name.LocalName} {e.Attribute("Include")?.Value}")
                .ToList();
            Assert.True(taken.Count == 0, $"{file} takes {string.Join(", ", taken)}");
        }
    }
}
