// Copyright © Dominic Beger 2018

using System;

namespace nUpdate.Administration.Core.Localization
{
    public class LocalizationProperties
    {
        public LocalizationProperties()
        {
            ProductTitle = "nUpdate Administration 1.1.0.0";
            CancelButtonText = "Cancel";
            ContinueButtonText = "Continue";
            SaveButtonText = "Save";
            DoneButtonText = "Done";
            CreatePackageButtonText = "Create package";
            EditPackageButtonText = "Edit package";
            SendButtonText = "Send";
            SaveLanguageButtonText = "Save language";

            InvalidArgumentErrorCaption = "Invalid argument found.";
            InvalidArgumentErrorText = "The entry for {0} can't be parsed to {1}.";

            /*
             * CredentialsDialog
             * */
            CredentialsDialogTitle = "Enter your credentials";
            CredentialsDialogHeaderText = "Credentials";
            CredentialsDialogUsernameLabelText = "Username:";
            CredentialsDialogPasswordLabelext = "Password:";

            /*
             * DirectorySearchDialog
             * */
            DirectorySearchDialogTitle = "Set the directory - {0} - {1}";
            DirectorySearchDialogInfoLabelText = "Select the directory that should be used for the update files.";
            DirectorySearchDialogDirectoryLabelText = "Directory:";
            DirectorySearchDialogServerNodeText = "Server";

            /*
             * FeedbackDialog
             * */
            FeedbackDialogTitle = "Feedback - {0}";
            FeedbackDialogHeaderLabelText = "Feedback";
            FeedbackDialogEmailAdressWatermarkText = "Your e-mail address";
            FeedbackDialogNameWatermarkText = "Name";
            FeedBackDialogPrivacyTermsLinkLabelText = "How will my data be used?";

            /*
             * HistoryDialog
             * */
            HistoryDialogTitle = "History - {0} - {1}";
            HistoryDialogClearButtonText = "Clear history";
            HistoryDialogSaveToFileButtonText = "Save to file.";
            HistoryDialogOrderDescendingEntryText = "Order descending";
            HistoryDialogOrderAscendingEntryText = "Order ascending";
            HistoryDialogNoHistoryLabelText = "No history available.";
            HistoryDialogCreatedPackageEntryText = "Created package";
            HistoryDialogEditedPackageEntryText = "Edited package";
            HistoryDialogUploadedPackageEntryText = "Uploaded package";
            HistoryDialogDeletedPackageEntryText = "Deleted package";

            /*
             * InfoDialog
             * */
            InfoDialogTitle = "Information - {0}";
            InfoDialogLicenseInfoLabelText =
                "nUpdate's solution contains 7 projects which are all licensed under the CC-ND-license.";
            InfoDialogIconsLabelText = "Icons:";
            InfoDialogIconPackageByLabelText = "Fugue Icon Package by";
            InfoDialogSpecialThanksToLabelText = "Special thanks to ";
            InfoDialogAsRsaContributorLabelText = "as a contributor to the RsaSignature-class and";
            InfoDialogEnbLabelText = "for tips and the great \"ExplorerNavigationButton\"-control.";
            InfoDialogFollowProjectLabelText = "Follow this project on ";
            InfoDialogOrHaveALookLabelText = "or have a look at";

            /*
             * JSONEditorDialog
             * */
            JsonEditorDialogTitle = "Edit language file - {0} - {1}";

            /*
             * MainDialog
             * */
            MainDialogInfoText = "Welcome, here you can create and manage your updates for nUpdate.";
            MainDialogProjectsGroupText = "Projects";
            MainDialogPreferencesGroupText = "Preferences";
            MainDialogInformationGroupText = "Information";
            MainDialogNewProjectText = "New project";
            MainDialogNewProjectDescriptionText = "Creates a new project.";
            MainDialogOpenProjectText = "Open project";
            MainDialogOpenProjectDescriptionText = "Opens an existing project.";
            MainDialogInformationText = "Information";
            MainDialogInformationDescriptionText = "Shows the information.";
            MainDialogPreferencesText = "Preferences";
            MainDialogProxyText = "Proxy";
            MainDialogProxyDescriptionText = "Configurate your proxies here.";
            MainDialogFeedbackText = "Feedback";
            MainDialogFeedbackDescriptionText = "Send your feedback here.";
            OperatingSystemNotSupportedWarn = "Your system is not supported. Windows Vista or higher necessary...";
            MissingRightsWarnCaption = "Missing rights.";
            MissingRightsWarnText = "You do not own the admin rights to create the extension's registry entry.";

            /*
             * NewProjectDialog
             * */
            NewProjectDialogTitle = "New project - {0}";
            PanelSignatureHeader = "Key pair generation";
            PanelSignatureInfoText =
                "nUpdate uses a 4096-bit RSA-Signature to serve for the security of your update packages.\nThe generation of these keys may take a different amount of time,\ndepending on your PC. Please be patient.";
            PanelSignatureWaitText = "Please wait while the key pair is being generated...";
            PanelGeneralHeader = "General";
            PanelGeneralNameText = "Name:";
            PanelGeneralNameWatermarkText = "The name of the project";
            PanelGeneralLanguageText = "Language:";
            PanelFtpHeader = "FTP-Data";
            PanelFtpServerText = "Adress:";
            PanelFtpUserText = "User:";
            PanelFtpUserWatermarkText = "The username";
            PanelFtpPasswordText = "Password:";
            PanelFtpDirectoryText = "Directory:";
            PanelFtpPortText = "Port:";
            PanelFtpPortWatermarkText = "The port";
            PanelFtpPassiveConnectionText = "Passive (rated)";
            PanelFtpActiveConnectionText = "Active";
            AlreadyExistingWarnText = "The project {0} already exists.";

            /*
             * ProjectDialog
             * */
            ProjectDialogTitle = "{0} - {1}";
            ProjectDialogOverviewTabText = "Overview";
            ProjectDialogNameLabelText = "Name:";
            ProjectDialogUpdateUrlLabelText = "Update-URL:";
            ProjectDialogFtpHostLabelText = "FTP-Host:";
            ProjectDialogFtpDirectoryLabelText = "FTP-directory:";
            ProjectDialogPackagesAmountLabelText = "Amount of packages released:";
            ProjectDialogNewestPackageLabelText = "Newest package released:";
            ProjectDialogInfoFileloadingLabelText = "Status of the update-info file:";
            ProjectDialogCheckInfoFileStatusLinkLabelText = "Check status";
            ProjectDialogProjectDataText = "Project-data";
            ProjectDialogPublicKeyLabelText = "Public key:";
            ProjectDialogProjectIdLabelText = "Project-ID:";
            ProjectDialogPackagesTabText = "Packages";
            ProjectDialogOverviewText = "Project-overview";
            ProjectDialogAddButtonText = "Add";
            ProjectDialogEditButtonText = "Edit";
            ProjectDialogDeleteButtonText = "Delete";
            ProjectDialogUploadButtonText = "Upload";
            ProjectDialogHistoryButtonText = "History";
            ProjectDialogVersionText = "Version";
            ProjectDialogReleasedText = "Released";
            ProjectDialogSizeText = "Size";
            ProjectDialogDescriptionText = "Description";
            ProjectDialogSearchText = "Search...";

            /*
             * ProjectEditDialog
             * */
            ProjectEditDialogNewNameText = "New name:";
            ProjectEditDialogLanguageText = "Language:";
            ProjectEditDialogRenameText = "The new name of the project";
            ProjectEditDialogTitle = "Edit project";

            #region "PackageAddDialog

            PackageAddDialogTitle = "Add new package - {0} - {1}";
            PackageAddDialogGeneralItemText = "General";
            PackageAddDialogChangelogItemText = "Changelog";
            PackageAddDialogFilesItemText = "Files";
            PackageAddDialogAvailabilityItemText = "Availability";
            PackageAddDialogDevelopmentalStageLabelText = "Developmental stage:";
            PackageAddDialogVersionLabelText = "Version:";
            PackageAddDialogDescriptionLabelText = "Description:";
            PackageAddDialogPublishCheckBoxText = "Publish this update";
            PackageAddDialogPublishInfoLabelText =
                "Sets if the package should be uploaded yet. You can upload it later, if you disable this" +
                Environment.NewLine +
                "option. The update package will be saved locally on your PC then.";
            PackageAddDialogEnvironmentLabelText = "Architecture settings:";
            PackageAddDialogEnvironmentInfoLabelText =
                "Sets if the update package should only run on special architectures. To set any type" +
                Environment.NewLine +
                "of architecture, choose \"AnyCPU\" as entry.";

            PackageAddDialogLoadButtonText = "Load from file...";
            PackageAddDialogClearButtonText = "Clear";
            PackageAddDialogAddFileButtonText = "Add files...";
            PackageAddDialogRemoveFileButtonText = "Remove files...";
            PackageAddDialogNameHeaderText = "Name";
            PackageAddDialogSizeHeaderText = "Size";
            PackageAddDialogAvailableForAllRadioButtonText = "Available for all older versions";
            PackageAddDialogAvailableForAllInfoText =
                "This package is available and can be downloaded for all older versions.";
            PackageAddDialogAvailableForSomeRadioButtonText = "Not available for some older versions";
            PackageAddDialogAvailableForSomeInfoText = "This package is not available for the following versions.";
            PackageAddDialogAddButtonText = "Add";
            PackageAddDialogRemoveButtonText = "Remove";

            PackageAddDialogArchiveInitializerInfoText = "Initializing archive...";
            PackageAddDialogPrepareInfoText = "Preparing update...";
            PackageAddDialogSigningInfoText = "Signing package...";
            PackageAddDialogConfigInitializerInfoText = "Initializing config...";
            PackageAddDialogUploadingPackageInfoText = "Uploading package - {0}";
            PackageAddDialogUploadingConfigInfoText = "Uploading configuration...";

            PackageAddDialogNoInternetWarningText =
                "nUpdate Administration could not verify a network connection. Some functions are disabled for now and you can only save the package on your PC. An upload is possible as soon as a network connections is given.";
            PackageAddDialogNoInternetWarningCaption = "No network connection available.";
            PackageAddDialogNoFilesSpecifiedWarningText =
                "There were no files specified for the update package. Please make sure you added an archive or some files to pack automatically.";
            PackageAddDialogNoFilesSpecifiedWarningCaption = "No files for the package set.";
            PackageAddDialogUnsupportedArchiveWarningText =
                "You added an unsupported archive type to the list. nUpdate is only able to unpack \".ZIP\"-files at the moment.";
            PackageAddDialogUnsupportedArchiveWarningCaption = "Unsupported archive type.";
            PackageAddDialogVersionInvalidWarningText =
                "The given version is invalid. You cannot use \"0.0.x.x\" as product-version. Please make sure to select a minimum Minor-version of \"1\".";
            PackageAddDialogVersionInvalidWarningCaption = "Invalid package version.";
            PackageAddDialogVersionExistingWarningText = "Version \"{0}\" is already existing.";
            PackageAddDialogNoChangelogWarningText = "There was no changelog set for the update package.";
            PackageAddDialogNoChangelogWarningCaption = "No changelog set.";
            PackageAddDialogAlreadyImportedWarningText =
                "The file \" {0} \" is already imported. Should it be replaced by the new one?";
            PackageAddDialogAlreadyImportedWarningCaption = "File already imported";

            PackageAddDialogPackageDataCreationErrorCaption = "Creating package data failed.";
            PackageAddDialogProjectDataLoadingErrorCaption = "Failed to load project data.";
            PackageAddDialogGettingUrlErrorCaption = "Error while getting url.";
            PackageAddDialogReadingPackageBytesErrorCaption = "Reading package bytes failed.";
            PackageAddDialogInvalidServerDirectoryErrorCaption = "Invalid server directory.";
            PackageAddDialogInvalidServerDirectoryErrorText =
                "The directory for the update files on the server is not valid. Please edit it.";
            PackageAddDialogLoadingFtpDataErrorCaption = "Failed to load FTP-data.";
            PackageAddDialogConfigurationDownloadErrorCaption = "Configuration download failed.";
            PackageAddDialogSerializingDataErrorCaption = "Error on serializing data.";
            PackageAddDialogRelativeUriErrorText = "The server-directory can't be set as a relative uri.";
            PackageAddDialogPackageInformationSavingErrorCaption = "Saving package information failed.";
            PackageAddDialogUploadFailedErrorCaption = "Upload failed.";

            #endregion

            /*
             * Errors
             * */
            ProjectReadingErrorCaption = "Error while reading the project.";
            ListingServerDataErrorCaption = "Error while listing the server-data.";
        }

        // The "Cancel"-button text
        public string CancelButtonText { get; set; }

        // The "Continue"-button text
        public string ContinueButtonText { get; set; }

        // The "Create package"-button text
        public string CreatePackageButtonText { get; set; }

        // The "Done"-button text
        public string DoneButtonText { get; set; }

        // The "Edit package"-button text
        public string EditPackageButtonText { get; set; }

        // The caption of an "Invalid argument"-error
        public string InvalidArgumentErrorCaption { get; set; }

        // The text of an "Invalid argument"-error
        public string InvalidArgumentErrorText { get; set; }

        #region "JSONEditorDialog"

        // The title of the JSONEditorDialog
        public string JsonEditorDialogTitle { get; set; }

        #endregion

        // The title in the forms
        public string ProductTitle { get; set; }

        // The "Save"-button text
        public string SaveButtonText { get; set; }

        // The "Save language"-button text
        public string SaveLanguageButtonText { get; set; }

        // The "Send"-button text
        public string SendButtonText { get; set; }

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
        public string InfoDialogEnbLabelText { get; set; }

        // The text of the "Follow this project on"-label
        public string InfoDialogFollowProjectLabelText { get; set; }

        // The text of the "or have a look at..."-label
        public string InfoDialogOrHaveALookLabelText { get; set; }

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

        #region "Async operation's messages"

        #endregion
    }
}