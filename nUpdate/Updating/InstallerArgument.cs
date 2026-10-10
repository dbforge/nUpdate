using Newtonsoft.Json;

namespace nUpdate.Updating;

/// <summary>When the installer passes an argument to the restarted application.</summary>
public enum ArgumentCondition
{
    /// <summary>Only after a successful update.</summary>
    Succeeded,

    /// <summary>Only after a failed update.</summary>
    Failed,

    /// <summary>After every update.</summary>
    Always,
}

/// <summary>A command line argument the installer passes to the application when it restarts it.</summary>
[method: JsonConstructor]
public sealed class InstallerArgument(string value, ArgumentCondition when = ArgumentCondition.Always)
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    public ArgumentCondition When { get; } = when;
}
