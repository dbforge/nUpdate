// Fails when the API page and the public API of the packages differ: every public type and member needs an entry on
// the page, and every entry on the page needs a type or member that still exists.
// Usage: dotnet run --project tools/ApiDocsCheck -- <api.html> <assembly.dll>... [--list]
//
// The page names what it documents in data-api attributes, with the IDs this tool prints: the documentation comment ID
// of the type or member, followed by ~ and its type for members that have one (return, property, field or event
// type), so that a changed signature shows up as one entry missing and one stale. An element may list several IDs,
// separated by spaces, such as all overloads of a method or the same member in the three UpdaterUI packages.
// --list prints the public API instead of comparing it.

using System.Text.RegularExpressions;
using ApiDocsCheck;

var list = args.Contains("--list");
var paths = args.Where(a => a != "--list").ToList();
if (paths.Count < 2)
    return Fail("Usage: ApiDocsCheck <api.html> <assembly.dll>... [--list]");

var page = paths[0];
var assemblies = paths.Skip(1).ToList();
var notFound = paths.FirstOrDefault(p => !File.Exists(p));
if (notFound is not null)
    return Fail($"Not found: {notFound}");

var api = new SortedSet<string>(StringComparer.Ordinal);
foreach (var assembly in assemblies)
    api.UnionWith(ApiSurface.Read(assembly));

if (list)
{
    foreach (var id in api)
        Console.WriteLine(id);
    return 0;
}

var documented = new SortedSet<string>(StringComparer.Ordinal);
foreach (Match match in Regex.Matches(File.ReadAllText(page), "data-api=\"([^\"]*)\""))
    documented.UnionWith(match.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(id => System.Net.WebUtility.HtmlDecode(id)!));

var missing = api.Except(documented).ToList();
var stale = documented.Except(api).ToList();
Console.WriteLine($"{api.Count} public types and members in {assemblies.Count} assemblies, {documented.Count} IDs on {page}.");
if (missing.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine($"Missing from the page ({missing.Count}): add an entry with these IDs in data-api.");
    foreach (var id in missing)
        Console.WriteLine("  " + id);
}

if (stale.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine($"On the page but not in the API ({stale.Count}): update or remove these entries.");
    foreach (var id in stale)
        Console.WriteLine("  " + id);
}

if (missing.Count == 0 && stale.Count == 0)
{
    Console.WriteLine("The page documents the whole public API.");
    return 0;
}

return 1;

static int Fail(string message)
{
    Console.Error.WriteLine(message);
    return 2;
}
