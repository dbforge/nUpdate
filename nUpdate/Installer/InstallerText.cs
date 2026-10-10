using nUpdate.Localization;

namespace nUpdate.Installer;

/// <summary>Keys of the localized texts the installer shows.</summary>
public enum InstallerText
{
    ExtractingFiles,
    Copying,
    FileDeleting,
    FileRenaming,
    RegistrySubKeyCreate,
    RegistrySubKeyDelete,
    RegistryValueDelete,
    RegistryValueSet,
    ProcessStart,
    ProcessStop,
    ServiceStart,
    ServiceStop,
    ProcessWaiting,
    ProcessExitCodeError,
    WindowTitle,
    WaitingForApplication,
    UpdatingErrorCaption,
    InitializingErrorCaption,
    FileInUseError,
    RetryButton,
    SkipButton,
    AbortButton,
    CloseButton,
    LogFileHint,
}

/// <summary>English fallback texts for <see cref="InstallerText" />, the defaults of <see cref="UpdateTexts" />.</summary>
public static class InstallerTexts
{
    private static readonly Dictionary<string, string>
        English = InstallerTextMapper.ToInstallerTexts(new UpdateTexts());

    public static string Default(InstallerText key) =>
        English.TryGetValue(key.ToString(), out var text) ? text : throw new ArgumentOutOfRangeException(nameof(key));
}
