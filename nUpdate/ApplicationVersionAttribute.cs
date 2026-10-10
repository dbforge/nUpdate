namespace nUpdate;

/// <summary>Declares the current version of the application on its entry assembly, for example <c>[assembly: ApplicationVersion("2.1.0")]</c>.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ApplicationVersionAttribute(string version) : Attribute
{
    public string Version { get; } = version ?? throw new ArgumentNullException(nameof(version));
}
