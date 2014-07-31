using System;

namespace nUpdate.Administration.Core.Localization
{
    public class LocalizationProperties
    {
        public LocalizationProperties()
        {
            this.ProductTitle = "nUpdate Administration 1.1.0.0";
            this.CancelButtonText = "Cancel";
            this.ContinueButtonText = "Continue";
            this.SaveButtonText = "Save";
            this.DoneButtonText = "Done";
            this.CreatePackageButtonText = "Create package";
            this.EditPackageButtonText = "Edit package";
            this.SendButtonText = "Send";
            this.SaveLanguageButtonText = "Save language";

            this.InvalidArgumentErrorCaption = "Invalid argument found.";
            this.InvalidArgumentErrorText = "The entry for {0} can't be parsed to {1}.";

            /*
             * CredentialsDialog
             * */
            this.CredentialsDialogTitle = "Enter your credentials";
            this.CredentialsDialogHeaderText = "Credentials";
            this.CredentialsDialogUsernameLabelText = "Username:";
            this.CredentialsDialogPasswordLabelext = "Password:";

            /*
             * DirectorySearchDialog
             * */
            this.DirectorySearchDialogTitle = "Set the directory - {0} - {1}";
            this.DirectorySearchDialogInfoLabelText = "Select the directory that should be used for the update files.";
            this.DirectorySearchDialogDirectoryLabelText = "Directory:";
            this.DirectorySearchDialogServerNodeText = "Server";

            /*
             * FeedbackDialog
             * */
            this.FeedbackDialogTitle = "Feedback - {0}";
            this.FeedbackDialogHeaderLabelText = "Feedback";
            this.FeedbackDialogEmailAdressWatermarkText = "Your e-mail address";
            this.FeedbackDialogNameWatermarkText = "Name";
            this.FeedBackDialogPrivacyTermsLinkLabelText = "How will my data be used?";

            /*
             * HistoryDialog
             * */
            this.HistoryDialogTitle = "History - {0} - {1}";
            this.HistoryDialogClearButtonText = "Clear history";
            this.HistoryDialogSaveToFileButtonText = "Save to file.";
            this.HistoryDialogOrderDescendingEntryText = "Order descending";
            this.HistoryDialogOrderAscendingEntryText = "Order ascending";
            this.HistoryDialogNoHistoryLabelText = "No history available.";
            this.HistoryDialogCreatedPackageEntryText = "Created package";
            this.HistoryDialogEditedPackageEntryText = "Edited package";
            this.HistoryDialogUploadedPackageEntryText = "Uploaded package";
            this.HistoryDialogDeletedPackageEntryText = "Deleted package";

            /*
             * InfoDialog
             * */
            this.InfoDialogTitle = "Information - {0}";
            this.InfoDialogLicenseInfoLabelText = "nUpdate's solution contains 7 projects which are all licensed under the CC-ND-license.";
            this.InfoDialogIconsLabelText = "Icons:";
            this.InfoDialogIconPackageByLabelText = "Fugue Icon Package by";
            this.InfoDialogSpecialThanksToLabelText = "Special thanks to ";
            this.InfoDialogAsRsaContributorLabelText = "as a contributor to the RsaSignature-class and";
            this.InfoDialogENBLabelText = "for tips and the great \"ExplorerNavigationButton\"-control.";
            this.InfoDialogFollowProjectLabelText = "Follow this project on ";
            this.InfoDialogOrHaveALookLabelText = "or have a look at";

            /*
             * JSONEditorDialog
             * */
            this.JsonEditorDialogTitle = "Edit language file - {0} - {1}";

            /*
             * MainDialog
             * */
            this.MainDialogInfoText = "Welcome, here you can create and manage your updates for nUpdate.";
            this.MainDialogProjectsGroupText = "Projects";
            this.MainDialogPreferencesGroupText = "Preferences";
            this.MainDialogInformationGroupText = "Information";
            this.MainDialogNewProjectText = "New project";
            this.MainDialogNewProjectDescriptionText = "Creates a new project.";
            this.MainDialogOpenProjectText = "Open project";
            this.MainDialogOpenProjectDescriptionText = "Opens an existing project.";
            this.MainDialogInformationText = "Information";
            this.MainDialogInformationDescriptionText = "Shows the information.";
            this.MainDialogPreferencesText = "Preferences";
            this.MainDialogProxyText = "Proxy";
            this.MainDialogProxyDescriptionText = "Configurate your proxies here.";
            this.MainDialogFeedbackText = "Feedback";
            this.MainDialogFeedbackDescriptionText = "Send your feedback here.";
            this.OperatingSystemNotSupportedWarn = "Your system is not supported. Windows Vista or higher necessary...";
            this.MissingRightsWarnCaption = "Missing rights.";
            this.MissingRightsWarnText = "You do not own the admin rights to create the extension's registry entry.";

            /*
             * NewProjectDialog
             * */
            this.NewProjectDialogTitle = "New project - {0}";
            this.PanelSignatureHeader = "Key pair generation";
            this.PanelSignatureInfoText = "nUpdate uses a 4096-bit RSA-Signature to serve for the security of your update packages.\nThe generation of these keys may take a different amount of time,\ndepending on your PC. Please be patient.";
            this.PanelSignatureWaitText = "Please wait while the key pair is being generated...";
            this.PanelGeneralHeader = "General";
            this.PanelGeneralNameText = "Name:";
            this.PanelGeneralNameWatermarkText = "The name of the project";
            this.PanelGeneralLanguageText = "Language:";
            this.PanelFtpHeader = "FTP-Data";
            this.PanelFtpServerText = "Adress:";
            this.PanelFtpUserText = "User:";
            this.PanelFtpUserWatermarkText = "The username";
            this.PanelFtpPasswordText = "Password:";
            this.PanelFtpDirectoryText = "Directory:";
            this.PanelFtpPortText = "Port:";
            this.PanelFtpPortWatermarkText = "The port";
            this.PanelFtpPassiveConnectionText = "Passive (rated)";
            this.PanelFtpActiveConnectionText = "Active";
            this.AlreadyExistingWarnText = "The project {0} already exists.";

            /*
             * ProjectDialog
             * */
            this.ProjectDialogTitle = "{0} - {1}";
            this.ProjectDialogOverviewTabText = "Overview";
            this.ProjectDialogNameLabelText = "Name:";
            this.ProjectDialogUpdateUrlLabelText = "Update-URL:";
            this.ProjectDialogFtpHostLabelText = "FTP-Host:";
            this.ProjectDialogFtpDirectoryLabelText = "FTP-directory:";
            this.ProjectDialogPackagesAmountLabelText = "Amount of packages released:";
            this.ProjectDialogNewestPackageLabelText = "Newest package released:";
            this.ProjectDialogInfoFileloadingLabelText = "Status of the update-info file:";
            this.ProjectDialogCheckInfoFileStatusLinkLabelText = "Check status";
            this.ProjectDialogProjectDataText = "Project-data";
            this.ProjectDialogPublicKeyLabelText = "Public key:";
            this.ProjectDialogProjectIdLabelText = "Project-ID:";
            this.ProjectDialogPackagesTabText = "Packages";
            this.ProjectDialogOverviewText = "Project-overview";
            this.ProjectDialogAddButtonText = "Add";
            this.ProjectDialogEditButtonText = "Edit";
            this.ProjectDialogDeleteButtonText = "Delete";
            this.ProjectDialogUploadButtonText = "Upload";
            this.ProjectDialogHistoryButtonText = "History";
            this.ProjectDialogVersionText = "Version";
            this.ProjectDialogReleasedText = "Released";
            this.ProjectDialogSizeText = "Size";
            this.ProjectDialogDescriptionText = "Description";
            this.ProjectDialogSearchText = "Search...";

            /*
             * ProjectEditDialog
             * */
            this.ProjectEditDialogNewNameText = "New name:";
            this.ProjectEditDialogLanguageText = "Language:";
            this.ProjectEditDialogRenameText = "The new name of the project";
            this.ProjectEditDialogTitle = "Edit project";

            #region "PackageAddDialog

            this.PackageAddDialogTitle = "Add new package - {0} - {1}";
            this.PackageAddDialogGeneralItemText = "General";
            this.PackageAddDialogChangelogItemText = "Changelog";
            this.PackageAddDialogFilesItemText = "Files";
            this.PackageAddDialogAvailabilityItemText = "Availability";
            this.PackageAddDialogDevelopmentalStageLabelText = "Developmental stage:";
            this.PackageAddDialogVersionLabelText = "Version:";
            this.PackageAddDialogDescriptionLabelText = "Description:";
            this.PackageAddDialogPublishCheckBoxText = "Publish this update";
            this.PackageAddDialogPublishInfoLabelText = "Sets if the package should be uploaded yet. You can upload it later, if you disable this" + Environment.NewLine +
                                             "option. The update package will be saved locally on your PC then.";
            this.PackageAddDialogEnvironmentLabelText = "Architecture settings:";
            this.PackageAddDialogEnvironmentInfoLabelText = "Sets if the update package should only run on special architectures. To set any type" + Environment.NewLine +
                                                       "of architecture, choose \"AnyCPU\" as entry.";

            this.PackageAddDialogLoadButtonText = "Load from file...";
            this.PackageAddDialogClearButtonText = "Clear";
            this.PackageAddDialogAddFileButtonText = "Add files...";
            this.PackageAddDialogRemoveFileButtonText = "Remove files...";
            this.PackageAddDialogNameHeaderText = "Name";
            this.PackageAddDialogSizeHeaderText = "Size";
            this.PackageAddDialogAvailableForAllRadioButtonText = "Available for all older versions";
            this.PackageAddDialogAvailableForAllInfoText = "This package is available and can be downloaded for all older versions.";
            this.PackageAddDialogAvailableForSomeRadioButtonText = "Not available for some older versions";
            this.PackageAddDialogAvailableForSomeInfoText = "This package is not available for the following versions.";
            this.PackageAddDialogAddButtonText = "Add";
            this.PackageAddDialogRemoveButtonText = "Remove";

            this.PackageAddDialogArchiveInitializerInfoText = "Initializing archive...";
            this.PackageAddDialogPrepareInfoText = "Preparing update...";
            this.PackageAddDialogSigningInfoText = "Signing package...";
            this.PackageAddDialogConfigInitializerInfoText = "Initializing config...";
            this.PackageAddDialogUploadingPackageInfoText = "Uploading package - {0}";
            this.PackageAddDialogUploadingConfigInfoText = "Uploading configuration...";

            this.PackageAddDialogNoInternetWarningText = "nUpdate Administration could not verify a network connection. Some functions are disabled for now and you can only save the package on your PC. An upload is possible as soon as a network connections is given.";
            this.PackageAddDialogNoInternetWarningCaption = "No network connection available.";
            this.PackageAddDialogNoFilesSpecifiedWarningText = "There were no files specified for the update package. Please make sure you added an archive or some files to pack automatically.";
            this.PackageAddDialogNoFilesSpecifiedWarningCaption = "No files for the package set.";
            this.PackageAddDialogUnsupportedArchiveWarningText = "You added an unsupported archive type to the list. nUpdate is only able to unpack \".ZIP\"-files at the moment.";
            this.PackageAddDialogUnsupportedArchiveWarningCaption = "Unsupported archive type.";
            this.PackageAddDialogVersionInvalidWarningText = "The given version is invalid. You cannot use \"0.0.x.x\" as product-version. Please make sure to select a minimum Minor-version of \"1\".";
            this.PackageAddDialogVersionInvalidWarningCaption = "Invalid package version.";
            this.PackageAddDialogVersionExistingWarningText = "Version \"{0}\" is already existing.";
            this.PackageAddDialogNoChangelogWarningText = "There was no changelog set for the update package.";
            this.PackageAddDialogNoChangelogWarningCaption = "No changelog set.";
            this.PackageAddDialogAlreadyImportedWarningText = "The file \" {0} \" is already imported. Should it be replaced by the new one?";
            this.PackageAddDialogAlreadyImportedWarningCaption = "File already imported";

            this.PackageAddDialogPackageDataCreationErrorCaption = "Creating package data failed.";
            this.PackageAddDialogProjectDataLoadingErrorCaption = "Failed to load project data.";
            this.PackageAddDialogGettingUrlErrorCaption = "Error while getting url.";
            this.PackageAddDialogReadingPackageBytesErrorCaption = "Reading package bytes failed.";
            this.PackageAddDialogInvalidServerDirectoryErrorCaption = "Invalid server directory.";
            this.PackageAddDialogInvalidServerDirectoryErrorText = "The directory for the update files on the server is not valid. Please edit it.";
            this.PackageAddDialogLoadingFtpDataErrorCaption = "Failed to load FTP-data.";
            this.PackageAddDialogConfigurationDownloadErrorCaption = "Configuration download failed.";
            this.PackageAddDialogSerializingDataErrorCaption = "Error on serializing data.";
            this.PackageAddDialogRelativeUriErrorText = "The server-directory can't be set as a relative uri.";
            this.PackageAddDialogPackageInformationSavingErrorCaption = "Saving package information failed.";
            this.PackageAddDialogUploadFailedErrorCaption = "Upload failed.";

            #endregion

            /*
             * Errors
             * */
            this.ProjectReadingErrorCaption = "Error while reading the project.";
            this.ListingServerDataErrorCaption = "Error while listing the server-data.";
        }

        // The title in the forms
        public string ProductTitle { get; set; }

        // The "Cancel"-button text
        public string CancelButtonText { get; set; }

        // The "Continue"-button text
        public string ContinueButtonText { get; set; }

        // The "Save"-button text
        public string SaveButtonText { get; set; }

        // The "Done"-button text
        public string DoneButtonText { get; set; }

        // The "Create package"-button text
        public string CreatePackageButtonText { get; set; }

        // The "Edit package"-button text
        public string EditPackageButtonText { get; set; }

        // The "Send"-button text
        public string SendButtonText { get; set; }

        // The "Save language"-button text
        public string SaveLanguageButtonText { get; set; }

        // The caption of an "Invalid argument"-error
        public string InvalidArgumentErrorCaption { get; set; }

        // The text of an "Invalid argument"-error
        public string InvalidArgumentErrorText { get; set; }

        #region "CredentialsDialog"

        // The title of the credentials dialog
        public string CredentialsDialogTitle { get; set; }

        // The text of the header 
        public string CredentialsDialogHeaderText { get; set; }

        // The text of the username-label
        public string CredentialsDialogUsernameLabelText { get; set; }

        // The text of the password-label
        public string CredentialsDialogPasswordLabelext { get; set; }

        #endregion

        #region "DirectorySearchDialog"

        // The title of the DirectorySearchDialog
        public string DirectorySearchDialogTitle { get; set; }

        // The text of the directory-label
        public string DirectorySearchDialogDirectoryLabelText { get; set; }

        // The text of the info-label
        public string DirectorySearchDialogInfoLabelText { get; set; }

        // The text of the server-node
        public string DirectorySearchDialogServerNodeText { get; set; }

        #endregion

        #region "FeedbackDialog"

        // The title of the FeedbackDialog
        public string FeedbackDialogTitle { get; set; }

        // The text of the header-label
        public string FeedbackDialogHeaderLabelText { get; set; }

        // The watermark of the email-address-textbox
        public string FeedbackDialogEmailAdressWatermarkText { get; set; }

        // The watermark of the name-textbox
        public string FeedbackDialogNameWatermarkText { get; set; }

        // The text of the privacy-terms-linklabel
        public string FeedBackDialogPrivacyTermsLinkLabelText { get; set; }

        #endregion

        #region "HistoryDialog"

        // The title of the HistoryDialog
        public string HistoryDialogTitle { get; set; }

        // The text of the "Clear"-button
        public string HistoryDialogClearButtonText { get; set; }

        // The text of the "Order Descending"-entry
        public string HistoryDialogOrderDescendingEntryText { get; set; }

        // The text of the "Order ascending"-entry
        public string HistoryDialogOrderAscendingEntryText { get; set; }

        // The text of the "Save file"-button
        public string HistoryDialogSaveToFileButtonText { get; set; }

        // The text of the "No history"-label
        public string HistoryDialogNoHistoryLabelText { get; set; }

        // The text of the entry for a created package
        public string HistoryDialogCreatedPackageEntryText { get; set; }

        // The text of the entry for a edited package
        public string HistoryDialogEditedPackageEntryText { get; set; }

        // The text of the entry for a uploaded package
        public string HistoryDialogUploadedPackageEntryText { get; set; }

        // The text of the entry for a deleted package
        public string HistoryDialogDeletedPackageEntryText { get; set; }

        #endregion

        #region "InfoDialog"

        // The title of the InfoDialog
        public string InfoDialogTitle { get; set; }

        // The text of the license-label
        public string InfoDialogLicenseInfoLabelText { get; set; }

        // The text of the "Icons:"-label
        public string InfoDialogIconsLabelText { get; set; }

        // The text of the "Fugue Icon Package by:"-text
        public string InfoDialogIconPackageByLabelText { get; set; }

        // The text of the "Special thanks to..."-label
        public string InfoDialogSpecialThanksToLabelText { get; set; }

        // The text of the "as a contributor..."-label
        public string InfoDialogAsRsaContributorLabelText { get; set; }

        // The text of the "for tips and the ExplorerNavigationButton"-label
        public string InfoDialogENBLabelText { get; set; }

        // The text of the "Follow this project on"-label
        public string InfoDialogFollowProjectLabelText { get; set; }

        // The text of the "or have a look at..."-label
        public string InfoDialogOrHaveALookLabelText { get; set; }

        #endregion

        #region "JSONEditorDialog"

        // The title of the JSONEditorDialog
        public string JsonEditorDialogTitle { get; set; }

        #endregion

        #region "MainDialog"

        // The header in the MainDialog
        public string MainDialogHeader { get; set; }

        // The info text in the MainDialog
        public string MainDialogInfoText { get; set; }

        // The text of the "project" group in the ListView
        public string MainDialogProjectsGroupText { get; set; }

        // The text of the "Preferences" group in the ListView
        public string MainDialogPreferencesGroupText { get; set; }

        // The text for the "information" group in the ListView
        public string MainDialogInformationGroupText { get; set; }

        // The text for the "New project"-button
        public string MainDialogNewProjectText { get; set; }

        // The text of the description for the "New project"-button
        public string MainDialogNewProjectDescriptionText { get; set; }

        // The text for the "Open project"-button
        public string MainDialogOpenProjectText { get; set; }

        // The text of the description for the "Open project"-button
        public string MainDialogOpenProjectDescriptionText { get; set; }

        // The text for the "Information"-button
        public string MainDialogInformationText { get; set; }

        // The text of the description for the "Information"-button
        public string MainDialogInformationDescriptionText { get; set; }

        // The text for the "Preferences"-button
        public string MainDialogPreferencesText { get; set; }

        // The text of the description for the "Preferences"-button
        public string MainDialogPreferencesDescriptionText { get; set; }

        // The text for the "Proxy"-button
        public string MainDialogProxyText { get; set; }

        // The text of the description for the "Proxy"-button
        public string MainDialogProxyDescriptionText { get; set; }

        // The text for the "Feedback"-button
        public string MainDialogFeedbackText { get; set; }

        // The text of the description for the "Feedback"-button
        public string MainDialogFeedbackDescriptionText { get; set; }

        #endregion

        #region "NewProjectDialog"

        // The title of the NewProjectDialog
        public string NewProjectDialogTitle { get; set; }

        // The header of the signature-panel
        public string PanelSignatureHeader { get; set; }

        // The info text in the signature-panel
        public string PanelSignatureInfoText { get; set; }

        // The text of the "Loading"-label
        public string PanelSignatureWaitText { get; set; }

        // The header of the general-panel
        public string PanelGeneralHeader { get; set; }

        // The text of the "Name"-label
        public string PanelGeneralNameText { get; set; }

        // The watermark-text of the "Name"-textbox
        public string PanelGeneralNameWatermarkText { get; set; }

        // The text of the "Language"-label
        public string PanelGeneralLanguageText { get; set; }

        // The header of the Ftp-panel
        public string PanelFtpHeader { get; set; }

        // The text of the "Server"-label
        public string PanelFtpServerText { get; set; }

        // The text of the "User"-label
        public string PanelFtpUserText { get; set; }

        // The watermark-text of the "User"-textbox
        public string PanelFtpUserWatermarkText { get; set; }

        // The text of the "Password"-label
        public string PanelFtpPasswordText { get; set; }

        // The text of the "Directory"-label
        public string PanelFtpDirectoryText { get; set; }

        // The text of the "Port"-label
        public string PanelFtpPortText { get; set; }

        // The watermark-text of the "Port"-textbox
        public string PanelFtpPortWatermarkText { get; set; }

        // The text of the "Passive"-combobox item
        public string PanelFtpPassiveConnectionText { get; set; }

        // The text of the "Active"-combobox item
        public string PanelFtpActiveConnectionText { get; set; }

        #endregion

        #region "ProjectDialog"

        // The title of the "Project"-form
        public string ProjectDialogTitle { get; set; }

        // The text of the "Overview"-tabpage
        public string ProjectDialogOverviewTabText { get; set; }

        // The text of the "Packages"-tabpage
        public string ProjectDialogPackagesTabText { get; set; }

        // The text of the "Overview"-header
        public string ProjectDialogOverviewText { get; set; }

        // The text of the "Name"-label
        public string ProjectDialogNameLabelText { get; set; }

        // The text of the "Update-URL"-label
        public string ProjectDialogUpdateUrlLabelText { get; set; }

        // The text of the "FTP-Host"-label
        public string ProjectDialogFtpHostLabelText { get; set; }

        // The text of the "FTP-Directory"-label
        public string ProjectDialogFtpDirectoryLabelText { get; set; }

        // The text of the "Amount of packages released"-label
        public string ProjectDialogPackagesAmountLabelText { get; set; }

        // The text of the "Newest package released"-label
        public string ProjectDialogNewestPackageLabelText { get; set; }

        // The text of the "Status of the info-file"-label
        public string ProjectDialogInfoFileloadingLabelText { get; set; }

        // The text of the "Check status"-label
        public string ProjectDialogCheckInfoFileStatusLinkLabelText { get; set; }

        // The text of the "Project-data"-header
        public string ProjectDialogProjectDataText { get; set; }

        // The text of the "Public key"-label
        public string ProjectDialogPublicKeyLabelText { get; set; }

        // The text of the "Project-ID"-label
        public string ProjectDialogProjectIdLabelText { get; set; }

        // The text of the "Add"-button
        public string ProjectDialogAddButtonText { get; set; }

        // The text of the "Edit"-button
        public string ProjectDialogEditButtonText { get; set; }

        // The text of the "Delete"-button
        public string ProjectDialogDeleteButtonText { get; set; }

        // The text of the "Upload"-button
        public string ProjectDialogUploadButtonText { get; set; }

        // The text of the "History"-button
        public string ProjectDialogHistoryButtonText { get; set; }

        // The text of the "Version"-column
        public string ProjectDialogVersionText { get; set; }

        // The text of the "Released"-coumn
        public string ProjectDialogReleasedText { get; set; }

        // The text of the "Size"-column
        public string ProjectDialogSizeText { get; set; }

        // The text of the "Description"-column
        public string ProjectDialogDescriptionText { get; set; }

        // The watermark-text of the "Search"-textbox
        public string ProjectDialogSearchText { get; set; }

        #endregion

        #region "ProjectEditDialog"

        // The title of the ProjectEditDialog
        public string ProjectEditDialogTitle { get; set; }

        // The watermark-text of the "Rename"-textbox
        public string ProjectEditDialogRenameText { get; set; }

        // The text of the "New name"-label
        public string ProjectEditDialogNewNameText { get; set; }

        // The text of the "Language"-label
        public string ProjectEditDialogLanguageText { get; set; }

        #endregion

        #region "PackageAddDialog

        // The title of the PackageAddDialog
        public string PackageAddDialogTitle { get; set; }

        // The text of the "General"-item
        public string PackageAddDialogGeneralItemText { get; set; }

        // The text of the "Changelog"-item
        public string PackageAddDialogChangelogItemText { get; set; }

        // The text of the "Files"-item
        public string PackageAddDialogFilesItemText { get; set; }

        // The text of the "Availability"-item
        public string PackageAddDialogAvailabilityItemText { get; set; }

        // The text of the "Operations"-item
        public string PackageAddDialogOperationsItemText { get; set; }

        // The text of the "Proxy"-item
        public string PackageAddDialogProxyItemText { get; set; }

        // General-panel

        // The text of the "Developmental stage"-label
        public string PackageAddDialogDevelopmentalStageLabelText { get; set; }

        // The text of the "Version"-label
        public string PackageAddDialogVersionLabelText { get; set; }

        // The text of the "Description"-label
        public string PackageAddDialogDescriptionLabelText { get; set; }

        // The text of the checkbox for publishing
        public string PackageAddDialogPublishCheckBoxText { get; set; }

        // The text of the publishing info-label
        public string PackageAddDialogPublishInfoLabelText { get; set; }

        // The text of the environment-label
        public string PackageAddDialogEnvironmentLabelText { get; set; }

        // The text of the environment-info-label
        public string PackageAddDialogEnvironmentInfoLabelText { get; set; }

        // Changelog-Tab

        // The text of the "Load"-button
        public string PackageAddDialogLoadButtonText { get; set; }

        // The text of the "Clear"-button
        public string PackageAddDialogClearButtonText { get; set; }

        // Files-tab

        // The text of the "Add File"-button
        public string PackageAddDialogAddFileButtonText { get; set; }

        // The text of the "Remove File"-button
        public string PackageAddDialogRemoveFileButtonText { get; set; }

        // The text of the "Name"-header in the listview
        public string PackageAddDialogNameHeaderText { get; set; }

        // The text of the "Size"-header in the listview
        public string PackageAddDialogSizeHeaderText { get; set; }

        // Availability

        // The text of the radio button for all available versions
        public string PackageAddDialogAvailableForAllRadioButtonText { get; set; }

        // The text of the info for the radio button for all available versions
        public string PackageAddDialogAvailableForAllInfoText { get; set; }

        // The text of the radio button for some available versions
        public string PackageAddDialogAvailableForSomeRadioButtonText { get; set; }

        // The text of the info for the radio button for some available versions
        public string PackageAddDialogAvailableForSomeInfoText { get; set; }

        // The text of the "Add"-button to add the versions
        public string PackageAddDialogAddButtonText { get; set; }

        // The text of the "Remove"-button to remove the versions
        public string PackageAddDialogRemoveButtonText { get; set; }

        // Proxy

        // The text of the CheckBox, if a proxy should be used
        public string PackageAddDialogUseProxyCheckBoxText { get; set; }

        // The text of the "Host"-label
        public string PackageAddDialogProxyHostLabelText { get; set; }

        // The text of the "Port"-label
        public string PackageAddDialogProxyPortLabelText { get; set; }

        // The text of the "Username"-label
        public string PackageAddDialogProxyUsernameLabelText { get; set; }

        // The text of the "Password"-label
        public string PackageAddDialogProxyPasswordLabelText { get; set; }

        // The text of the proxy info-label
        public string PackageAddDialogProxyInfoLabelText { get; set; }

        // Operations

        // The text of the "Use operations"-checkbox.
        public string PackageAddDialogUseOperationsCheckBoxText { get; set; }

        // The text of the "Add operation"-button.
        public string PackageAddDialogOperationAddButtonText { get; set; }

        // Information while creating

        public string PackageAddDialogArchiveInitializerInfoText { get; set; }

        public string PackageAddDialogPrepareInfoText { get; set; }

        public string PackageAddDialogSigningInfoText { get; set; }

        public string PackageAddDialogConfigInitializerInfoText { get; set; }

        public string PackageAddDialogUploadingPackageInfoText { get; set; }

        public string PackageAddDialogUploadingConfigInfoText { get; set; }

        // Warnings and errors

        // The caption of the error if the data could not be created
        public string PackageAddDialogPackageDataCreationErrorCaption { get; set; }

        // The caption of the error if the data of the project could not be loaded
        public string PackageAddDialogProjectDataLoadingErrorCaption { get; set; }

        // The caption of the error if the url for the package could not be defined
        public string PackageAddDialogGettingUrlErrorCaption { get; set; }

        // The caption of the error if the package could not be read while signing
        public string PackageAddDialogReadingPackageBytesErrorCaption { get; set; }

        // The caption of the error if the server directory is invalid
        public string PackageAddDialogInvalidServerDirectoryErrorCaption { get; set; }

        // The text of the error if the server directory is invalid
        public string PackageAddDialogInvalidServerDirectoryErrorText { get; set; }

        // The caption of the error if the FTP-data could not be loaded
        public string PackageAddDialogLoadingFtpDataErrorCaption { get; set; }

        // The caption of the error if the configuration download failed
        public string PackageAddDialogConfigurationDownloadErrorCaption { get; set; }

        // The caption of the error if the serializing of the data failed
        public string PackageAddDialogSerializingDataErrorCaption { get; set; }

        // The text of the error if the directory can't be set as a relative uri
        public string PackageAddDialogRelativeUriErrorText { get; set; }

        // The caption of the error if the saving of the package information/data fails
        public string PackageAddDialogPackageInformationSavingErrorCaption { get; set; }

        // The caption of the error if the upload fails
        public string PackageAddDialogUploadFailedErrorCaption { get; set; }

        // The text of the warning if there is no network available
        public string PackageAddDialogNoInternetWarningText { get; set; }

        // The caption of the warning if there is no network available
        public string PackageAddDialogNoInternetWarningCaption { get; set; }

        // The text of the warning when no files are specified for the package
        public string PackageAddDialogNoFilesSpecifiedWarningText { get; set; }

        // The caption of the warning when no files are specified for the package
        public string PackageAddDialogNoFilesSpecifiedWarningCaption { get; set; }

        // The text of the warning when there is an unsupported archive type added to the list
        public string PackageAddDialogUnsupportedArchiveWarningText { get; set; }

        // The caption of the warning when there is an unsupported archive type added to the list
        public string PackageAddDialogUnsupportedArchiveWarningCaption { get; set; }

        // The text of the warning when the given version is invalid
        public string PackageAddDialogVersionInvalidWarningText { get; set; }

        // The text of the warning when the given version is already existing
        public string PackageAddDialogVersionExistingWarningText { get; set; }

        // The caption of the warning when the given version is invalid
        public string PackageAddDialogVersionInvalidWarningCaption { get; set; }

        // The text of the warning when thre is no changelog specified
        public string PackageAddDialogNoChangelogWarningText { get; set; }

        // The caption of the warning when thre is no changelog specified
        public string PackageAddDialogNoChangelogWarningCaption { get; set; }

        // The caption of the warning when the file is already imported
        public string PackageAddDialogAlreadyImportedWarningText { get; set; }

        // The text of the warning when the file is already imported
        public string PackageAddDialogAlreadyImportedWarningCaption { get; set; }

        #endregion

        #region "Errors"

        /*
         * General errors
         * */

        // The caption of the error when not all fields wre filled out
        public string InputsMissingErrorCaption { get; set; }

        // The text of the error when not all fields wre filled out
        public string InputsMissingErrorText { get; set; }

        // Sets the caption of the error when the reading of the project fails
        public string ProjectReadingErrorCaption { get; set; }

        // The text of the "Not supported-warning"
        public string OperatingSystemNotSupportedWarn { get; set; }

        // The caption of the "Missing rights-warning"
        public string MissingRightsWarnCaption { get; set; }

        // The text of the "Missing rights-warning"
        public string MissingRightsWarnText { get; set; }

        // The text of the warning when the project already exists
        public string AlreadyExistingWarnText { get; set; }

        /*
         * Server-errors
         * */

        // The caption of the error when the listing of the server data fails
        public string ListingServerDataErrorCaption { get; set; }

        #endregion
    }
}
