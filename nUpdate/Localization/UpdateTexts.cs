namespace nUpdate.Localization;

/// <summary>
///     The texts of the nUpdate client library and its built-in user interfaces. Every property has an English
///     default, so a partial custom text file is valid.
/// </summary>
public sealed class UpdateTexts
{
    public string Cancel { get; set; } = "Cancel";
    public string Close { get; set; } = "Close";
    public string Install { get; set; } = "Install";
    public string Searching { get; set; } = "Searching for updates...";
    public string NewUpdatesTitle { get; set; } = "{0} new updates available.";
    public string NewUpdateTitle { get; set; } = "{0} new update available.";
    public string NewUpdateInfo { get; set; } = "New updates can be downloaded for {0}.";
    public string AvailableVersions { get; set; } = "Available versions: {0}";
    public string CurrentVersion { get; set; } = "Current version: {0}";
    public string TotalSize { get; set; } = "Total package size: {0}";
    public string Changelog { get; set; } = "Changelog:";
    public string Touches { get; set; } = "Accesses:";
    public string TouchesRegistry { get; set; } = "Registry";
    public string TouchesFiles { get; set; } = "File system";
    public string TouchesProcesses { get; set; } = "Processes";
    public string TouchesServices { get; set; } = "Services";
    public string StaysClosedAfterUpdate { get; set; } = "{0} stays closed after the update.";
    public string NoUpdatesTitle { get; set; } = "There are no new updates available.";
    public string NoUpdatesInfo { get; set; } = "The application is currently up-to-date.";
    public string Downloading { get; set; } = "Downloading updates...";
    public string DownloadingInfo { get; set; } = "Please wait while the available updates are\ndownloaded...  ({0}%)";
    public string DownloadError { get; set; } = "Error while downloading the update packages.";
    public string InstallerExtracting { get; set; } = "Extracting files...";
    public string InstallerCopying { get; set; } = "Copying {0}...";
    public string InstallerInitializingError { get; set; } = "Error while initializing the installer.";
    public string InstallerUpdatingError { get; set; } = "Error while updating the application.";

    public string InstallerFileInUse { get; set; } =
        "The installer cannot overwrite the file '{0}' because it is being used by another process. Close the applications that block it and try again.";

    public string RenamingFile { get; set; } = "Renaming file \"{0}\" to \"{1}\"...";
    public string DeletingFile { get; set; } = "Deleting file \"{0}\"...";
    public string CreatingRegistryKey { get; set; } = "Creating registry subkey \"{0}\"...";
    public string DeletingRegistryKey { get; set; } = "Deleting registry subkey \"{0}\"...";
    public string SettingRegistryValue { get; set; } = "Setting value of \"{0}\" in the registry to \"{1}\"...";
    public string DeletingRegistryValue { get; set; } = "Deleting name-value-pair \"{0}\"...";
    public string StartingProcess { get; set; } = "Starting process \"{0}\"...";
    public string TerminatingProcess { get; set; } = "Terminating process \"{0}\"...";
    public string StartingService { get; set; } = "Starting service \"{0}\"...";
    public string StoppingService { get; set; } = "Stopping service \"{0}\"...";
    public string WaitingForProcess { get; set; } = "Waiting for \"{0}\" to exit...";
    public string ProcessExitCode { get; set; } = "\"{0}\" exited with code {1}.";
    public string InstallerTitle { get; set; } = "Updating {0}";
    public string InstallerWaiting { get; set; } = "Waiting for {0} to close...";
    public string InstallerRetry { get; set; } = "Retry";
    public string InstallerSkip { get; set; } = "Skip";
    public string InstallerAbort { get; set; } = "Abort";
    public string InstallerLogFile { get; set; } = "The log file contains the details: {0}";
    public string SearchError { get; set; } = "Error while searching for updates.";
    public string VerificationError { get; set; } = "Error while checking the package's signature.";
    public string PackageNotFound { get; set; } = "The package file couldn't be found.";
    public string InvalidSignatureTitle { get; set; } = "Invalid signature data found.";

    public string InvalidSignatureInfo { get; set; } =
        "nUpdate will cancel the installation of the update packages and delete them unrecoverably.";

    public string InvalidSignatureData { get; set; } =
        "The signature of the update package is not a valid RSA-signature.";

    public string PackageFileNotFound { get; set; } = "The update package of version \"{0}\" could not be found.";
    public string NotEnoughDiskSpaceTitle { get; set; } = "Not enough disk space.";

    public string NotEnoughDiskSpaceInfo { get; set; } =
        "You don't have enough disk space left on your drive and nUpdate is not able to download and install the available updates ({0}). Please free a minimum of {1} to make sure the updates can be downloaded and installed without any problems.";

    public string InstallerNotFound { get; set; } =
        "The update installer was not found at \"{0}\". Reference the nUpdate.UpdateInstaller.UI.Avalonia package or set InstallerPath to your own installer.";

    public string NoWriteAccess { get; set; } =
        "{0} cannot be updated because this user may not change the files in \"{1}\". Ask an administrator to install the update or to give you write access to the folder.";
}
