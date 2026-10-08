using nUpdate.Installer;

namespace nUpdate.Localization;

/// <summary>Builds the installer text table from the client's texts.</summary>
internal static class InstallerTextMapper
{
    public static Dictionary<string, string> ToInstallerTexts(UpdateTexts texts)
    {
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));

        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(InstallerText.ExtractingFiles)] = texts.InstallerExtracting,
            [nameof(InstallerText.Copying)] = texts.InstallerCopying,
            [nameof(InstallerText.FileDeleting)] = texts.DeletingFile,
            [nameof(InstallerText.FileRenaming)] = texts.RenamingFile,
            [nameof(InstallerText.RegistrySubKeyCreate)] = texts.CreatingRegistryKey,
            [nameof(InstallerText.RegistrySubKeyDelete)] = texts.DeletingRegistryKey,
            [nameof(InstallerText.RegistryValueDelete)] = texts.DeletingRegistryValue,
            [nameof(InstallerText.RegistryValueSet)] = texts.SettingRegistryValue,
            [nameof(InstallerText.ProcessStart)] = texts.StartingProcess,
            [nameof(InstallerText.ProcessStop)] = texts.TerminatingProcess,
            [nameof(InstallerText.ServiceStart)] = texts.StartingService,
            [nameof(InstallerText.ServiceStop)] = texts.StoppingService,
            [nameof(InstallerText.ProcessWaiting)] = texts.WaitingForProcess,
            [nameof(InstallerText.ProcessExitCodeError)] = texts.ProcessExitCode,
            [nameof(InstallerText.WindowTitle)] = texts.InstallerTitle,
            [nameof(InstallerText.WaitingForApplication)] = texts.InstallerWaiting,
            [nameof(InstallerText.UpdatingErrorCaption)] = texts.InstallerUpdatingError,
            [nameof(InstallerText.InitializingErrorCaption)] = texts.InstallerInitializingError,
            [nameof(InstallerText.FileInUseError)] = texts.InstallerFileInUse,
            [nameof(InstallerText.RetryButton)] = texts.InstallerRetry,
            [nameof(InstallerText.SkipButton)] = texts.InstallerSkip,
            [nameof(InstallerText.AbortButton)] = texts.InstallerAbort,
            [nameof(InstallerText.CloseButton)] = texts.Close,
            [nameof(InstallerText.LogFileHint)] = texts.InstallerLogFile,
        };
    }
}
