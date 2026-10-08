using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Operations;

namespace nUpdate.Administration.ViewModels;

/// <summary>
///     The kinds of operations the editor offers, one per operation type of the manifest. <see cref="Icon" /> is the key
///     of the icon geometry in App.axaml.
/// </summary>
public sealed record OperationKind(string Type, string DisplayName, OperationArea Area, string Icon)
{
    /// <summary>Registry and service operations exist only on Windows.</summary>
    public bool RequiresWindows => Operation.IsWindowsOnly(Area);

    /// <summary>The area as the palette names it; registry and service operations say that they are Windows only.</summary>
    public string AreaTitle => Area switch
    {
        OperationArea.Files => "Files",
        OperationArea.Processes => "Processes",
        OperationArea.Registry => "Registry · Windows",
        _ => "Services · Windows",
    };

    public static IReadOnlyList<OperationKind> All { get; } =
    [
        new(DeleteFilesOperation.TypeName, "Delete files", OperationArea.Files, "IconDelete"),
        new(RenameFileOperation.TypeName, "Rename a file", OperationArea.Files, "IconRename"),
        new(StartProcessOperation.TypeName, "Start a process", OperationArea.Processes, "IconStart"),
        new(TerminateProcessOperation.TypeName, "Terminate a process", OperationArea.Processes, "IconStop"),
        new(CreateRegistryKeysOperation.TypeName, "Create registry sub keys", OperationArea.Registry, "IconRegistry"),
        new(DeleteRegistryKeysOperation.TypeName, "Delete registry sub keys", OperationArea.Registry, "IconRegistry"),
        new(SetRegistryValuesOperation.TypeName, "Set registry values", OperationArea.Registry, "IconRegistry"),
        new(DeleteRegistryValuesOperation.TypeName, "Delete registry values", OperationArea.Registry, "IconRegistry"),
        new(StartServiceOperation.TypeName, "Start a service", OperationArea.Services, "IconService"),
        new(StopServiceOperation.TypeName, "Stop a service", OperationArea.Services, "IconService"),
    ];

    public static OperationKind FromType(string type) =>
        All.FirstOrDefault(k => k.Type == type) ??
        throw new ArgumentException($"Unknown operation \"{type}\".", nameof(type));

    public static OperationKind FromOperation(Operation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return FromType(operation.Type);
    }
}

/// <summary>One registry value in a "set registry values" operation.</summary>
public partial class RegistryValueEditorViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = string.Empty;

    /// <summary>The value as text: a number for DWORD and QWORD, one entry per line for multi-strings, comma-separated bytes or Base64 for binary data.</summary>
    [ObservableProperty] private string _value = string.Empty;

    [ObservableProperty] private RegistryValueKind _kind = RegistryValueKind.String;

    public static IReadOnlyList<RegistryValueKind> Kinds { get; } =
    [
        RegistryValueKind.String, RegistryValueKind.ExpandString, RegistryValueKind.DWord, RegistryValueKind.QWord,
        RegistryValueKind.MultiString, RegistryValueKind.Binary
    ];

    /// <summary>Fills the editor from a value of an existing operation.</summary>
    public static RegistryValueEditorViewModel FromValue(RegistryValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new RegistryValueEditorViewModel
        {
            Name = value.Name,
            Kind = value.Kind,
            Value = value.Value switch
            {
                null => string.Empty,
                long number => number.ToString(CultureInfo.InvariantCulture),
                string[] strings => string.Join(Environment.NewLine, strings),
                byte[] bytes => string.Join(",", bytes),
                _ => (string)value.Value,
            },
        };
    }

    /// <summary>A validation message, or <c>null</c> when the value parses for its kind.</summary>
    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "every registry value needs a name.";
        try
        {
            ToRegistryValue();
            return null;
        }
        catch (FormatException)
        {
            return $"the value of \"{Name.Trim()}\" is not valid for {Kind}.";
        }
        catch (OverflowException)
        {
            return $"the value of \"{Name.Trim()}\" is out of range for {Kind}.";
        }
    }

    /// <exception cref="FormatException">The text does not parse for the kind.</exception>
    public RegistryValue ToRegistryValue()
    {
        var name = Name.Trim();
        var text = Value.Trim();
        return Kind switch
        {
            RegistryValueKind.DWord => RegistryValue.DWord(name,
                checked((int)long.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture))),
            RegistryValueKind.QWord => RegistryValue.QWord(name,
                long.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture)),
            RegistryValueKind.MultiString => RegistryValue.MultiString(name,
                Value.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)),
            RegistryValueKind.Binary => RegistryValue.Binary(name, ParseBytes(text)),
            RegistryValueKind.ExpandString => RegistryValue.ExpandString(name, Value),
            _ => RegistryValue.String(name, Value),
        };
    }

    private static byte[] ParseBytes(string text)
    {
        if (text.Length == 0)
            return [];
        var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.All(p => byte.TryParse(p, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            return parts.Select(p => byte.Parse(p, CultureInfo.InvariantCulture)).ToArray();
        return Convert.FromBase64String(text);
    }
}

/// <summary>Edits one operation. Which fields apply depends on the kind.</summary>
public partial class OperationEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    private string _value = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    private string _secondValue = string.Empty;

    [ObservableProperty] private string _listText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Phase))]
    private bool _runBeforeFileReplacement;

    /// <summary>For "start a process": wait until it has exited before the installer continues.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    private bool _waitForExit;

    /// <summary>For "start a process": an exit code other than 0 fails the update (only while waiting).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    private bool _failOnError;

    public OperationEditorViewModel(OperationKind kind)
    {
        Kind = kind ?? throw new ArgumentNullException(nameof(kind));
        RegistryValues.CollectionChanged += OnRegistryValuesChanged;
    }

    public OperationKind Kind { get; }

    public string DisplayName => Kind.DisplayName;

    public ObservableCollection<RegistryValueEditorViewModel> RegistryValues { get; } = [];

    public string ValueLabel => Kind.Type switch
    {
        DeleteFilesOperation.TypeName => "Directory (for example %program%/plugins)",
        RenameFileOperation.TypeName => "File path (for example %program%/old.dll)",
        CreateRegistryKeysOperation.TypeName or DeleteRegistryKeysOperation.TypeName
            or SetRegistryValuesOperation.TypeName
            or DeleteRegistryValuesOperation.TypeName =>
            "Registry key (for example HKEY_CURRENT_USER\\Software\\Vendor)",
        StartProcessOperation.TypeName => "Executable path (for example %program%/tool.exe)",
        TerminateProcessOperation.TypeName => "Process name (without .exe)",
        _ => "Service name",
    };

    public string? SecondValueLabel => Kind.Type switch
    {
        RenameFileOperation.TypeName => "New file name",
        StartProcessOperation.TypeName => "Arguments",
        _ => null,
    };

    public string? ListLabel => Kind.Type switch
    {
        DeleteFilesOperation.TypeName => "File names, one per line",
        CreateRegistryKeysOperation.TypeName or DeleteRegistryKeysOperation.TypeName => "Sub key names, one per line",
        DeleteRegistryValuesOperation.TypeName => "Value names, one per line",
        StartServiceOperation.TypeName => "Start arguments, one per line (optional)",
        _ => null,
    };

    public bool HasSecondValue => SecondValueLabel is not null;

    public bool HasList => ListLabel is not null;

    public bool HasRegistryValues => Kind.Type == SetRegistryValuesOperation.TypeName;

    public bool IsStartProcess => Kind.Type == StartProcessOperation.TypeName;

    /// <summary>Whether the first value is a path, which may start with a placeholder such as <c>%program%</c>.</summary>
    public bool UsesPaths => Kind.Area == OperationArea.Files || IsStartProcess;

    /// <summary>One line about what the operation does, for its card in the list.</summary>
    public string Summary
    {
        get
        {
            var text = string.IsNullOrWhiteSpace(SecondValue) ? Value.Trim() : $"{Value.Trim()} {SecondValue.Trim()}";
            if (text.Length == 0)
                return "Not filled in yet";
            if (!IsStartProcess || !WaitForExit)
                return text;
            return FailOnError ? text + " · waits, fails on error" : text + " · waits";
        }
    }

    /// <summary>When the operation runs, relative to replacing the files.</summary>
    public string Phase => RunBeforeFileReplacement ? "Before files" : "After files";

    /// <summary>Everything the user entered, to tell whether an existing package's operations changed.</summary>
    public string Signature => string.Join("\u001f", new[]
    {
        Kind.Type, Value, SecondValue, ListText, RunBeforeFileReplacement.ToString(), WaitForExit.ToString(),
        FailOnError.ToString(),
    }.Concat(RegistryValues.Select(v => $"{v.Name}={v.Kind}:{v.Value}")));

    /// <summary>Puts a placeholder in front of the path, replacing the one it starts with.</summary>
    public void InsertPlaceholder(string placeholder)
    {
        ArgumentNullException.ThrowIfNull(placeholder);
        var path = Value.Trim();
        if (path.StartsWith('%') && path.IndexOf('%', 1) is var end and > 0)
            path = path[(end + 1)..];
        path = path.TrimStart('/', '\\');
        Value = path.Length == 0 ? placeholder + "/" : $"{placeholder}/{path}";
    }

    /// <summary>Fills the editor from an existing operation.</summary>
    public static OperationEditorViewModel FromOperation(Operation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var editor = new OperationEditorViewModel(OperationKind.FromOperation(operation))
        { RunBeforeFileReplacement = operation.RunBeforeFileReplacement };
        switch (operation)
        {
            case DeleteFilesOperation delete:
                editor.Value = delete.Directory;
                editor.ListText = Lines(delete.Files);
                break;
            case RenameFileOperation rename:
                editor.Value = rename.Path;
                editor.SecondValue = rename.NewName;
                break;
            case CreateRegistryKeysOperation create:
                editor.Value = create.Key;
                editor.ListText = Lines(create.SubKeys);
                break;
            case DeleteRegistryKeysOperation deleteKeys:
                editor.Value = deleteKeys.Key;
                editor.ListText = Lines(deleteKeys.SubKeys);
                break;
            case SetRegistryValuesOperation set:
                editor.Value = set.Key;
                foreach (var value in set.Values)
                    editor.RegistryValues.Add(RegistryValueEditorViewModel.FromValue(value));
                break;
            case DeleteRegistryValuesOperation deleteValues:
                editor.Value = deleteValues.Key;
                editor.ListText = Lines(deleteValues.Names);
                break;
            case StartProcessOperation start:
                editor.Value = start.Path;
                editor.SecondValue = start.Arguments;
                editor.WaitForExit = start.WaitForExit;
                editor.FailOnError = start.FailOnError;
                break;
            case TerminateProcessOperation terminate:
                editor.Value = terminate.ProcessName;
                break;
            case StartServiceOperation startService:
                editor.Value = startService.ServiceName;
                editor.ListText = Lines(startService.Arguments);
                break;
            default:
                editor.Value = ((StopServiceOperation)operation).ServiceName;
                break;
        }

        return editor;
    }

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return $"{DisplayName}: enter the {ValueLabel.ToLowerInvariant()}.";
        if (Kind.Type == RenameFileOperation.TypeName && string.IsNullOrWhiteSpace(SecondValue))
            return $"{DisplayName}: enter the new file name.";
        if (HasList && Kind.Type != StartServiceOperation.TypeName && Lines().Length == 0)
            return $"{DisplayName}: enter at least one entry.";
        if (HasRegistryValues)
        {
            if (RegistryValues.Count == 0)
                return $"{DisplayName}: add at least one value.";
            var problem = RegistryValues.Select(v => v.Validate()).FirstOrDefault(p => p is not null);
            if (problem is not null)
                return $"{DisplayName}: {problem}";
        }

        return null;
    }

    public Operation ToOperation()
    {
        var value = Value.Trim();
        Operation operation = Kind.Type switch
        {
            DeleteFilesOperation.TypeName => new DeleteFilesOperation { Directory = value, Files = Lines().ToList() },
            RenameFileOperation.TypeName => new RenameFileOperation { Path = value, NewName = SecondValue.Trim() },
            CreateRegistryKeysOperation.TypeName => new CreateRegistryKeysOperation
            { Key = value, SubKeys = Lines().ToList() },
            DeleteRegistryKeysOperation.TypeName => new DeleteRegistryKeysOperation
            { Key = value, SubKeys = Lines().ToList() },
            SetRegistryValuesOperation.TypeName => new SetRegistryValuesOperation
            { Key = value, Values = RegistryValues.Select(v => v.ToRegistryValue()).ToList() },
            DeleteRegistryValuesOperation.TypeName => new DeleteRegistryValuesOperation
            { Key = value, Names = Lines().ToList() },
            StartProcessOperation.TypeName => new StartProcessOperation
            {
                Path = value,
                Arguments = SecondValue,
                WaitForExit = WaitForExit,
                FailOnError = WaitForExit && FailOnError
            },
            TerminateProcessOperation.TypeName => new TerminateProcessOperation { ProcessName = value },
            StartServiceOperation.TypeName => new StartServiceOperation
            { ServiceName = value, Arguments = Lines().ToList() },
            _ => new StopServiceOperation { ServiceName = value },
        };
        operation.RunBeforeFileReplacement = RunBeforeFileReplacement;
        return operation;
    }

    /// <summary>Registry values are part of the signature, so changing one changes the operation.</summary>
    private void OnRegistryValuesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var value in e.NewItems?.OfType<RegistryValueEditorViewModel>() ?? [])
            value.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Signature));
        OnPropertyChanged(nameof(Signature));
    }

    [RelayCommand]
    private void AddRegistryValue() => RegistryValues.Add(new RegistryValueEditorViewModel());

    [RelayCommand]
    private void RemoveRegistryValue(RegistryValueEditorViewModel? value)
    {
        if (value is not null)
            RegistryValues.Remove(value);
    }

    private static string Lines(IEnumerable<string> entries) => string.Join(Environment.NewLine, entries);

    private string[] Lines() =>
        ListText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
