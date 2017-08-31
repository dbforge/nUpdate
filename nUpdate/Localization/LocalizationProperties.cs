// Copyright © Dominic Beger 2017

namespace nUpdate.Localization
{
    internal struct LocalizationProperties
    {
        public string CancelButtonText { get; set; }
        public string ContinueButtonText { get; set; }
        public string InstallButtonText { get; set; }
        public string CloseButtonText { get; set; }

        public string UpdateSearchDialogHeader { get; set; }

        public string NewUpdateDialogMultipleUpdatesHeader { get; set; }
        public string NewUpdateDialogSingleUpdateHeader { get; set; }
        public string NewUpdateDialogInfoText { get; set; }
        public string NewUpdateDialogAvailableVersionsText { get; set; }
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
        public string InstallerFileInUseError { get; set; }
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
        public string UnfulfilledRequirementErrorCaption { get; set; }
        public string UnfulfilledRequirementErrorText { get; set; }
        public string OperatingSystemText { get; set; }
        public string DotNetFrameworkText { get; set; }
        public string RequiredVersionText { get; set; }
        public string PackageValidityCheckErrorCaption { get; set; }
        public string PackageNotFoundErrorText { get; set; }
        public string InvalidSignatureErrorCaption { get; set; }
        public string SignatureNotMatchingErrorText { get; set; }
        public string InvalidSignatureErrorText { get; set; }

        public string SearchProcessRunningExceptionText { get; set; }
        public string DownloadingProcessRunningExceptionText { get; set; }
        public string NetworkConnectionExceptionText { get; set; }
        public string PackageSizeCalculationExceptionText { get; set; }
        public string InvalidJsonExceptionText { get; set; }
        public string StatisticsScriptExceptionText { get; set; }
        public string PackageFileNotFoundExceptionText { get; set; }
        public string MainFolderCreationExceptionText { get; set; }
    }
}