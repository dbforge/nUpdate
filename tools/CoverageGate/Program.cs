// Fails the build when an assembly's line or branch coverage in a Cobertura report is below its threshold.
// Usage: dotnet run --project tools/CoverageGate -- <report.cobertura.xml>... --require <assembly>=<line%>[,<branch%>] ... [--show-uncovered]
//
// Several reports (or one report with the same assembly several times) are merged per line: hits are added, branch
// coverage takes the best value seen for a line. For exact branch merging across test projects feed the gate one report
// that ReportGenerator merged (CI does this).

using System.Globalization;
using System.Xml.Linq;

var reports = new List<string>();
var requirements = new Dictionary<string, (double Line, double Branch)>(StringComparer.OrdinalIgnoreCase);
var showUncovered = false;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--require" when i + 1 < args.Length:
            var spec = args[++i];
            var eq = spec.IndexOf('=', StringComparison.Ordinal);
            if (eq <= 0)
                return Fail($"Invalid requirement \"{spec}\". Expected <assembly>=<line%>[,<branch%>].");
            var parts = spec[(eq + 1)..].Split(',');
            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var line)
                || parts.Length > 2 || (parts.Length == 2 && !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _)))
                return Fail($"Invalid requirement \"{spec}\". Expected <assembly>=<line%>[,<branch%>] with numeric percentages.");
            var branch = parts.Length > 1 ? double.Parse(parts[1], CultureInfo.InvariantCulture) : line;
            requirements[spec[..eq]] = (line, branch);
            break;
        case "--show-uncovered":
            showUncovered = true;
            break;
        default:
            reports.Add(args[i]);
            break;
    }
}

if (reports.Count == 0)
    return Fail("No coverage report given.");

// file -> line number -> (hits, branch covered, branch total)
var assemblies = new Dictionary<string, Dictionary<(string File, int Line), (int Hits, int BranchCovered, int BranchTotal)>>(StringComparer.OrdinalIgnoreCase);

foreach (var report in reports)
{
    if (!File.Exists(report))
        return Fail($"Report not found: {report}");

    foreach (var package in XDocument.Load(report).Descendants("package"))
    {
        var name = package.Attribute("name")?.Value ?? "?";
        if (!assemblies.TryGetValue(name, out var lines))
            assemblies[name] = lines = [];

        foreach (var cls in package.Descendants("class"))
        {
            var file = cls.Attribute("filename")?.Value ?? "?";
            foreach (var lineElement in cls.Element("lines")?.Elements("line") ?? [])
            {
                var number = int.Parse(lineElement.Attribute("number")!.Value, CultureInfo.InvariantCulture);
                var hits = int.Parse(lineElement.Attribute("hits")!.Value, CultureInfo.InvariantCulture);
                var covered = 0;
                var total = 0;
                if (string.Equals(lineElement.Attribute("branch")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                {
                    // condition-coverage="50% (1/2)"
                    var text = lineElement.Attribute("condition-coverage")?.Value ?? "";
                    var open = text.IndexOf('(', StringComparison.Ordinal);
                    var slash = text.IndexOf('/', StringComparison.Ordinal);
                    var close = text.IndexOf(')', StringComparison.Ordinal);
                    if (open >= 0 && slash > open && close > slash)
                    {
                        covered = int.Parse(text[(open + 1)..slash], CultureInfo.InvariantCulture);
                        total = int.Parse(text[(slash + 1)..close], CultureInfo.InvariantCulture);
                    }
                }

                var key = (file, number);
                if (lines.TryGetValue(key, out var existing))
                    lines[key] = (existing.Hits + hits, Math.Max(existing.BranchCovered, covered), Math.Max(existing.BranchTotal, total));
                else
                    lines[key] = (hits, covered, total);
            }
        }
    }
}

var failed = false;
foreach (var (assembly, lines) in assemblies.OrderBy(pair => pair.Key, StringComparer.Ordinal))
{
    var lineTotal = lines.Count;
    var lineCovered = lines.Count(pair => pair.Value.Hits > 0);
    var branchTotal = lines.Sum(pair => pair.Value.BranchTotal);
    var branchCovered = lines.Sum(pair => pair.Value.BranchCovered);
    var linePercent = lineTotal == 0 ? 100 : 100.0 * lineCovered / lineTotal;
    var branchPercent = branchTotal == 0 ? 100 : 100.0 * branchCovered / branchTotal;

    var required = requirements.TryGetValue(assembly, out var r) ? r : ((double, double)?)null;
    var ok = required is null || (linePercent >= required.Value.Item1 - 1e-9 && branchPercent >= required.Value.Item2 - 1e-9);
    failed |= !ok;

    Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {assembly}: line {linePercent:F2}% ({lineCovered}/{lineTotal}), branch {branchPercent:F2}% ({branchCovered}/{branchTotal})"
        + (required is null ? " (no threshold)" : $" required {required.Value.Item1}% / {required.Value.Item2}%"));

    if (showUncovered || !ok)
    {
        foreach (var ((file, number), (hits, covered, total)) in lines.OrderBy(p => p.Key.File, StringComparer.Ordinal).ThenBy(p => p.Key.Line))
        {
            if (hits == 0)
                Console.WriteLine($"    uncovered line   {file}:{number}");
            else if (total > 0 && covered < total)
                Console.WriteLine($"    partial branch   {file}:{number} ({covered}/{total})");
        }
    }
}

foreach (var missing in requirements.Keys.Where(k => !assemblies.ContainsKey(k)))
{
    Console.WriteLine($"FAIL {missing}: not present in any report");
    failed = true;
}

return failed ? 1 : 0;

static int Fail(string message)
{
    Console.Error.WriteLine(message);
    return 2;
}
