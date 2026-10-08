using Newtonsoft.Json;

namespace nUpdate.Operations;

/// <summary>
///     An action the installer performs before or after replacing the application files. Each kind of operation is
///     its own class; in JSON the <c>type</c> property names the kind.
/// </summary>
public abstract class Operation
{
    /// <summary>The JSON discriminator, for example <c>deleteFiles</c>.</summary>
    [JsonProperty(Order = -2)]
    public abstract string Type { get; }

    /// <summary>The part of the system the operation touches.</summary>
    [JsonIgnore]
    public abstract OperationArea Area { get; }

    /// <summary>When <c>true</c> the operation runs before the package files are copied, otherwise after.</summary>
    public bool RunBeforeFileReplacement { get; set; }

    /// <summary>Registry and service operations exist only on Windows; packages for other platforms must not contain them.</summary>
    [JsonIgnore]
    public bool RequiresWindows => IsWindowsOnly(Area);

    /// <summary>Whether operations of the area exist only on Windows: the registry and services.</summary>
    public static bool IsWindowsOnly(OperationArea area) => area is OperationArea.Registry or OperationArea.Services;

    /// <summary>The discriminator of every operation class, so readers and editors can enumerate them.</summary>
    public static IReadOnlyDictionary<string, Type> Types { get; } = new Dictionary<string, Type>(StringComparer.Ordinal)
    {
        [DeleteFilesOperation.TypeName] = typeof(DeleteFilesOperation),
        [RenameFileOperation.TypeName] = typeof(RenameFileOperation),
        [CreateRegistryKeysOperation.TypeName] = typeof(CreateRegistryKeysOperation),
        [DeleteRegistryKeysOperation.TypeName] = typeof(DeleteRegistryKeysOperation),
        [SetRegistryValuesOperation.TypeName] = typeof(SetRegistryValuesOperation),
        [DeleteRegistryValuesOperation.TypeName] = typeof(DeleteRegistryValuesOperation),
        [StartProcessOperation.TypeName] = typeof(StartProcessOperation),
        [TerminateProcessOperation.TypeName] = typeof(TerminateProcessOperation),
        [StartServiceOperation.TypeName] = typeof(StartServiceOperation),
        [StopServiceOperation.TypeName] = typeof(StopServiceOperation),
    };
}

/// <summary>Deletes files from a directory of the client. Placeholders such as <c>%program%</c> are expanded by the installer.</summary>
public sealed class DeleteFilesOperation : Operation
{
    public const string TypeName = "deleteFiles";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Files;

    public string Directory { get; set; } = string.Empty;

    public List<string> Files { get; set; } = [];
}

/// <summary>Renames one file.</summary>
public sealed class RenameFileOperation : Operation
{
    public const string TypeName = "renameFile";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Files;

    public string Path { get; set; } = string.Empty;

    public string NewName { get; set; } = string.Empty;
}

/// <summary>Creates sub keys below a registry key.</summary>
public sealed class CreateRegistryKeysOperation : Operation
{
    public const string TypeName = "createRegistryKeys";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Registry;

    public string Key { get; set; } = string.Empty;

    public List<string> SubKeys { get; set; } = [];
}

/// <summary>Deletes sub keys below a registry key.</summary>
public sealed class DeleteRegistryKeysOperation : Operation
{
    public const string TypeName = "deleteRegistryKeys";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Registry;

    public string Key { get; set; } = string.Empty;

    public List<string> SubKeys { get; set; } = [];
}

/// <summary>Sets values of a registry key.</summary>
public sealed class SetRegistryValuesOperation : Operation
{
    public const string TypeName = "setRegistryValues";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Registry;

    public string Key { get; set; } = string.Empty;

    public List<RegistryValue> Values { get; set; } = [];
}

/// <summary>Deletes values of a registry key.</summary>
public sealed class DeleteRegistryValuesOperation : Operation
{
    public const string TypeName = "deleteRegistryValues";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Registry;

    public string Key { get; set; } = string.Empty;

    public List<string> Names { get; set; } = [];
}

/// <summary>Starts a process, optionally waiting for it and failing the update when it reports an error.</summary>
public sealed class StartProcessOperation : Operation
{
    public const string TypeName = "startProcess";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Processes;

    public string Path { get; set; } = string.Empty;

    public string Arguments { get; set; } = string.Empty;

    /// <summary>Whether the installer waits until the process has exited before it continues.</summary>
    public bool WaitForExit { get; set; }

    /// <summary>Whether an exit code other than 0 fails the update. Only applies with <see cref="WaitForExit" />.</summary>
    public bool FailOnError { get; set; }
}

/// <summary>Terminates every process with the given name.</summary>
public sealed class TerminateProcessOperation : Operation
{
    public const string TypeName = "terminateProcess";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Processes;

    public string ProcessName { get; set; } = string.Empty;
}

/// <summary>Starts a Windows service.</summary>
public sealed class StartServiceOperation : Operation
{
    public const string TypeName = "startService";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Services;

    public string ServiceName { get; set; } = string.Empty;

    public List<string> Arguments { get; set; } = [];
}

/// <summary>Stops a Windows service.</summary>
public sealed class StopServiceOperation : Operation
{
    public const string TypeName = "stopService";

    public override string Type => TypeName;

    public override OperationArea Area => OperationArea.Services;

    public string ServiceName { get; set; } = string.Empty;
}
