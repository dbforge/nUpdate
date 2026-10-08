using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reflection;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>Creates the built-in FTP/FTPS/SFTP providers or loads a plugin's factory.</summary>
public sealed class TransferProviderFactory : ITransferProviderFactory
{
    private readonly IFileSystem _fileSystem;
    private readonly Func<string, Assembly> _assemblyLoader;

    public TransferProviderFactory(IFileSystem fileSystem)
        : this(fileSystem, LoadAssembly)
    {
    }

    public TransferProviderFactory(IFileSystem fileSystem, Func<string, Assembly> assemblyLoader)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _assemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
    }

    public IReadOnlyCollection<TransferProtocol> SupportedProtocols { get; } =
        [TransferProtocol.Ftp, TransferProtocol.FtpsExplicit, TransferProtocol.FtpsImplicit, TransferProtocol.Sftp, TransferProtocol.Plugin];

    public ITransferProvider Create(TransferSettings settings, TransferCredentials credentials)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(credentials);
        if (string.IsNullOrWhiteSpace(settings.Host) && settings.Protocol != TransferProtocol.Plugin)
            throw new ArgumentException("The transfer settings have no host.", nameof(settings));

        return settings.Protocol switch
        {
            TransferProtocol.Ftp or TransferProtocol.FtpsExplicit or TransferProtocol.FtpsImplicit => new FtpTransferProvider(settings, credentials, _fileSystem),
            TransferProtocol.Sftp => new SftpTransferProvider(settings, credentials, _fileSystem),
            TransferProtocol.Plugin => LoadPluginFactory(settings).Create(settings, credentials),
            _ => throw new NotSupportedException($"The transfer protocol {settings.Protocol} is not supported."),
        };
    }

    /// <summary>Resolves the factory a plugin assembly exports through <see cref="ServiceProviderAttribute" />.</summary>
    public ITransferProviderFactory LoadPluginFactory(TransferSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (string.IsNullOrWhiteSpace(settings.PluginAssemblyPath))
            throw new InvalidOperationException("The project uses a transfer plugin but names no plugin assembly.");
        if (!_fileSystem.File.Exists(settings.PluginAssemblyPath))
            throw new FileNotFoundException($"The transfer plugin \"{settings.PluginAssemblyPath}\" does not exist.", settings.PluginAssemblyPath);
        return FromAssembly(_assemblyLoader(settings.PluginAssemblyPath));
    }

    public static ITransferProviderFactory FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var attribute = assembly.GetCustomAttribute<ServiceProviderAttribute>()
                        ?? throw new InvalidOperationException($"The assembly \"{assembly.GetName().Name}\" has no {nameof(ServiceProviderAttribute)}.");
        return FromProvider((IServiceProvider)Activator.CreateInstance(attribute.ServiceType)!);
    }

    public static ITransferProviderFactory FromProvider(IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        return provider.GetService(typeof(ITransferProviderFactory)) as ITransferProviderFactory
               ?? throw new InvalidOperationException("The plugin's service provider does not offer an ITransferProviderFactory.");
    }

    [ExcludeFromCodeCoverage] // Loads an arbitrary assembly from disk.
    private static Assembly LoadAssembly(string path) => Assembly.LoadFrom(path);
}
