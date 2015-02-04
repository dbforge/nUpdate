// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.Core.Localization
{
    public class LocalizationProperties
    {
        public LocalizationProperties()
        {
            CancelButtonText = "Cancel";
            ContinueButtonText = "Continue";
            InstallButtonText = "Install";
            CloseButtonText = "Close";

            UpdateSearchDialogHeader = "Searching for updates...";

            NewUpdateDialogHeader = "New updates available.";
            NewUpdateDialogInfoText = "New updates can be downloaded for {0}.";
            NewUpdateDialogNewestVersionText = "Newest version: {0}";
            NewUpdateDialogCurrentVersionText = "Current version: {0}";
            NewUpdateDialogSizeText = "Package size: {0}";
            NewUpdateDialogChangelogText = "Changelog:";
            NewUpdateDialogAccessText = "Accesses:";
            NewUpdateDialogFilesAccessText = "File system";
            NewUpdateDialogRegistryAccessText = "Registry";
            NewUpdateDialogProcessesAccessText = "Processes";
            NewUpdateDialogServicesAccessText = "Services";

            NoUpdateDialogHeader = "There are no new updates available.";
            NoUpdateDialogInfoText = "The application is currently up-to-date.";

            UpdateDownloadDialogLoadingHeader = "Downloading updates...";
            UpdateDownloadDialogLoadingInfo = "Please wait while the available updates are\ndownloaded...  ({0}%)";

            // Put the strings into \" because they are handled over as process arguments (Installer)
            
            InstallerExtractingFilesText = "\"Extracting files...\"";
            InstallerCopyingText = "\"Copying {0}...\"";
            InstallerInitializingErrorCaption = "\"Error while iniaitializing the data.\"";
            InstallerUpdatingErrorCaption = "\"Error while updating the application\"";
            InstallerUpdatingErrorText = "\"{0}. Should the updating be cancelled?\"";
            FileDeletingOperationText = "\"Deleting file \"{0}\"...\"";
            FileRenamingOperationText = "\"Renaming file \"{0}\" to \"{1}\"...\"";
            RegistrySubKeyCreateOperationText = "\"Creating registry subkey \"{0}\"...\"";
            RegistrySubKeyDeleteOperationText = "\"Deleting registry subkey \"{0}\"...\"";
            RegistryNameValuePairSetValueOperationText = "\"Setting value of \"{0}\" in the registry to \"{1}\"...\"";
            RegistryNameValuePairDeleteValueOperationText = "\"Deleting name-value-pair \"{0}\"...\"";
            ProcessStartOperationText = "\"Starting process \"{0}\"...\"";
            ProcessStopOperationText = "\"Terminating process \"{1}\"...\"";
            ServiceStartOperationText = "\"Starting service \"{0}\"...\"";
            ServiceStopOperationText = "\"Stopping service \"{0}\"...\"";

            UpdateSearchErrorCaption = "Error while searching for updates.";
            PackageValidityCheckErrorCaption = "Error while checking the package's signature.";
            PackageNotFoundErrorText = "The package file couldn't be found.";
            InvalidSignatureErrorCaption = "Invalid signature data found.";
            SignatureNotMatchingErrorText =
                "The package's signature does not match the application's integrated one and could be dangerous. In order to avoid any security risks, nUpdate will not allow to install the package and delete it unrecoverably.";
            InvalidSignatureErrorText = "The signature of the package is not a valid RSA-signature.";

            SearchProcessRunningExceptionText = "There is already a search process running.";
            DownloadingProcessRunningExceptionText = "There is already a download process running.";
            NetworkConnectionExceptionText = "No network connection available.";
            PackageSizeCalculationExceptionText =
                "The package size couldn't be calculated because an unknown error appeared. Possibly the package file does not exist.";
            StatisticsScriptExceptionText =
                "An error occured while trying to transfer data for the statistics. Please report this problem to the developer of the program in order to make him capable of fixing it. Response from the PHP-script: {0}";
            PackageFileNotFoundExceptionText = "The update package to check could not be found.";
            MainFolderCreationExceptionText = "The application's main folder could not be created. {0}";
        }

        public string CancelButtonText { get; set; }
        public string ContinueButtonText { get; set; }
        public string InstallButtonText { get; set; }
        public string CloseButtonText { get; set; }

        public string UpdateSearchDialogHeader { get; set; }

        public string NewUpdateDialogHeader { get; set; }
        public string NewUpdateDialogInfoText { get; set; }
        public string NewUpdateDialogNewestVersionText { get; set; }
        public string NewUpdateDialogCurrentVersionText { get; set; }
        public string NewUpdateDialogSizeText { get; set; }
        public string NewUpdateDialogChangelogText { get; set; }
        public string NewUpdateDialogAccessText { get; set; }
        public string NewUpdateDialogRegistryAccessText { get; set; }
        public string NewUpdateDialogFilesAccessText { get; set; }
        public string NewUpdateDialogProcessesAccessText { get; set; }
        public string NewUpdateDialogServicesAccessText { get; set; }

        public string NoUpdateDialogHeader { get; set; }
        public string NoUpdateDialogInfoText { get; set; }

        public string UpdateDownloadDialogLoadingHeader { get; set; }
        public string UpdateDownloadDialogLoadingInfo { get; set; }

        public string InstallerExtractingFilesText { get; set; }
        public string InstallerCopyingText { get; set; }
        public string InstallerInitializingErrorCaption { get; set; }
        public string InstallerUpdatingErrorCaption { get; set; }
        public string InstallerUpdatingErrorText { get; set; }
        public string FileRenamingOperationText { get; set; }
        public string FileDeletingOperationText { get; set; }
        public string RegistrySubKeyCreateOperationText { get; set; }
        public string RegistrySubKeyDeleteOperationText { get; set; }
        public string RegistryNameValuePairSetValueOperationText { get; set; }
        public string RegistryNameValuePairDeleteValueOperationText { get; set; }
        public string ProcessStartOperationText { get; set; }
        public string ProcessStopOperationText { get; set; }
        public string ServiceStartOperationText { get; set; }
        public string ServiceStopOperationText { get; set; }

        public string UpdateSearchErrorCaption { get; set; }
        public string PackageValidityCheckErrorCaption { get; set; }
        public string PackageNotFoundErrorText { get; set; }
        public string InvalidSignatureErrorCaption { get; set; }
        public string SignatureNotMatchingErrorText { get; set; }
        public string InvalidSignatureErrorText { get; set; }

        public string SearchProcessRunningExceptionText { get; set; }
        public string DownloadingProcessRunningExceptionText { get; set; }
        public string NetworkConnectionExceptionText { get; set; }
        public string PackageSizeCalculationExceptionText { get; set; }
        public string StatisticsScriptExceptionText { get; set; }
        public string PackageFileNotFoundExceptionText { get; set; }
        public string MainFolderCreationExceptionText { get; set; }
    }
}