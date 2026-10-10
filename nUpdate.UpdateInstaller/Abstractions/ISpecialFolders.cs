namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>The well-known folders packages can target.</summary>
internal interface ISpecialFolders
{
    string ApplicationData { get; }

    string Temp { get; }

    string Desktop { get; }
}
