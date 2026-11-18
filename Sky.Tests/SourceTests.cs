using System.Text.RegularExpressions;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The library's and the tests' sources, held to the width the repository's prose is wrapped at.
/// </summary>
public sealed partial class SourceTests
{
    private const int Width = 100;

    private static string Root
    {
        get
        {
            // The repository's root is the folder above the tests that holds the solution.
            var at = new DirectoryInfo(AppContext.BaseDirectory);
            while (at is not null && !File.Exists(Path.Combine(at.FullName, "Sky.slnx")))
                at = at.Parent;
            return at?.FullName
                ?? throw new DirectoryNotFoundException("No Sky.slnx above the tests.");
        }
    }

    // A comment line that holds an address and nothing else, which no wrapping shortens.
    [GeneratedRegex(@"^\s*///?\s*\S+://\S+\s*$")]
    private static partial Regex AddressAlone();

    [Fact]
    public void Every_Comment_Line_Holds_To_A_Hundred_Columns()
    {
        var root = Root;
        var over = new List<string>();
        foreach (var project in new[] { "Sky", "Sky.Tests" })
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, project), "*.cs", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(root, file);
                var parts = relative.Split(Path.DirectorySeparatorChar);
                if (parts.Contains("bin") || parts.Contains("obj"))
                    continue;
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.TrimStart().StartsWith("//", StringComparison.Ordinal) && line.Length > Width && !AddressAlone().IsMatch(line))
                        over.Add($"{relative}:{i + 1} ({line.Length})");
                }
            }
        }

        Assert.True(over.Count == 0, $"{over.Count} comment lines past {Width} columns:\n" + string.Join("\n", over));
    }

    // The engines the module was written beside, and the framework the second stands on, which
    // the library's sources and its public members' summaries name nowhere, so a stranger's
    // engine reads no other engine's in them.
    [GeneratedRegex(@"3DEngine|BevyCSharp|\bBevy\b", RegexOptions.IgnoreCase)]
    private static partial Regex EngineName();

    [Fact]
    public void The_Librarys_Sources_Name_No_Engine()
    {
        var root = Root;
        var named = new List<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "Sky"), "*.cs", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(root, file);
            if (relative.Split(Path.DirectorySeparatorChar).Any(p => p is "bin" or "obj"))
                continue;
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
                if (EngineName().IsMatch(lines[i]))
                    named.Add($"{relative}:{i + 1}");
        }

        Assert.True(named.Count == 0, $"{named.Count} lines name an engine:\n" + string.Join("\n", named));
    }
}
