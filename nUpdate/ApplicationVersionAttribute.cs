namespace nUpdate;

/// <summary>Declares the current version of the application on its entry assembly, for example <c>[assembly: ApplicationVersion("2.1.0")]</c>.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ApplicationVersionAttribute : Attribute
{
    public ApplicationVersionAttribute(string version)
    {
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }

    public string Version { get; }
}
