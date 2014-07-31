using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.History;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Dialogs;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddDialog : BaseDialog
    {
        private const string separatorCharacter = "-";
        private const float Kb = 1024;
        private const float Mb = 1048577;

        private readonly TreeNode createRegistryEntryNode = new TreeNode("Create registry entry");
        private readonly TreeNode deleteNode = new TreeNode("Delete file", 9, 9);
        private readonly TreeNode deleteRegistryEntryNode = new TreeNode("Delete registry entry");
        private readonly FTPManager ftp = new FTPManager();
        private readonly UpdatePackage package = new UpdatePackage();
        private readonly TreeNode renameNode = new TreeNode("Rename file", 10, 10);
        private readonly TreeNode replaceNode = new TreeNode("Replace file/folder", 11, 11);
        private readonly TreeNode setRegistryKeyValueNode = new TreeNode("Set registry key value");
        private readonly TreeNode startProcessNode = new TreeNode("Start process", 8, 8);
        private readonly TreeNode startServiceNode = new TreeNode("Start service", 5, 5);
        private readonly TreeNode stopServiceNode = new TreeNode("Stop service", 6, 6);
        private readonly TreeNode terminateProcessNode = new TreeNode("Terminate process", 7, 7);
        private readonly Log updateLog = new Log();
        private bool allowCancel = true;
        private int architectureIndex = 2;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private UpdateConfiguration configuration = new UpdateConfiguration();
        private int developmentBuild = 1;
        private bool hasFailedOnLoading;
        private bool mustUpdate;

        private string packageFolder;
        private UpdateVersion packageVersion;
        private bool publishUpdate;
        private Uri remoteInfoFileUrl;
        private string updateConfigFile;
        private ZipFile zip = new ZipFile();

        public PackageAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The newest package version.
        /// </summary>
        internal UpdateVersion NewestVersion { get; set; }

        /// <summary>
        ///     Sets the developmental stage of the package.
        /// </summary>
        public DevelopmentalStage DevStage { get; set; }

        /// <summary>
        ///     Returns all operations set.
        /// </summary>
        internal List<Operation> Operations { get; set; }

        #region "Localization"

        private string configDownloadErrorCaption;
        private string creatingPackageDataErrorCaption;
        private string ftpDataLoadErrorCaption;
        private string gettingUrlErrorCaption;
        private string initializingArchiveInfoText;
        private string initializingConfigInfoText;
        private string invalidArgumentCaption;
        private string invalidArgumentText;
        private string invalidServerDirectoryErrorCaption;
        private string invalidServerDirectoryErrorText;
        private string invalidVersionCaption;
        private string invalidVersionText;
        private string loadingProjectDataErrorCaption;
        private string noChangelogCaption;
        private string noChangelogText;
        private string noFilesCaption;
        private string noFilesText;
        private string noNetworkCaption;
        private string noNetworkText;
        private string preparingUpdateInfoText;
        private string readingPackageBytesErrorCaption;
        private string relativeUriErrorText;
        private string savingInformationErrorCaption;
        private string serializingDataErrorCaption;
        private string signingPackageInfoText;
        private string unsupportedArchiveCaption;
        private string unsupportedArchiveText;
        private string uploadFailedErrorCaption;
        private string uploadingConfigInfoText;
        private string uploadingPackageInfoText;

        private void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            var ls = new LocalizationProperties();
            if (File.Exists(languageFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            noNetworkCaption = ls.PackageAddDialogNoInternetWarningCaption;
            noNetworkText = ls.PackageAddDialogNoInternetWarningText;
            noFilesCaption = ls.PackageAddDialogNoFilesSpecifiedWarningCaption;
            noFilesText = ls.PackageAddDialogNoFilesSpecifiedWarningText;
            unsupportedArchiveCaption = ls.PackageAddDialogUnsupportedArchiveWarningCaption;
            unsupportedArchiveText = ls.PackageAddDialogUnsupportedArchiveWarningText;
            invalidVersionCaption = ls.PackageAddDialogVersionInvalidWarningCaption;
            invalidVersionText = ls.PackageAddDialogVersionInvalidWarningText;
            noChangelogCaption = ls.PackageAddDialogNoChangelogWarningCaption;
            noChangelogText = ls.PackageAddDialogNoChangelogWarningText;
            invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
            invalidArgumentText = ls.InvalidArgumentErrorText;
            creatingPackageDataErrorCaption = ls.PackageAddDialogPackageDataCreationErrorCaption;
            loadingProjectDataErrorCaption = ls.PackageAddDialogProjectDataLoadingErrorCaption;
            gettingUrlErrorCaption = ls.PackageAddDialogGettingUrlErrorCaption;
            readingPackageBytesErrorCaption = ls.PackageAddDialogReadingPackageBytesErrorCaption;
            invalidServerDirectoryErrorCaption = ls.PackageAddDialogInvalidServerDirectoryErrorCaption;
            invalidServerDirectoryErrorText = ls.PackageAddDialogInvalidServerDirectoryErrorText;
            ftpDataLoadErrorCaption = ls.PackageAddDialogLoadingFtpDataErrorCaption;
            configDownloadErrorCaption = ls.PackageAddDialogConfigurationDownloadErrorCaption;
            serializingDataErrorCaption = ls.PackageAddDialogSerializingDataErrorCaption;
            relativeUriErrorText = ls.PackageAddDialogRelativeUriErrorText;
            savingInformationErrorCaption = ls.PackageAddDialogPackageInformationSavingErrorCaption;
            uploadFailedErrorCaption = ls.PackageAddDialogUploadFailedErrorCaption;

            initializingArchiveInfoText = ls.PackageAddDialogArchiveInitializerInfoText;
            preparingUpdateInfoText = ls.PackageAddDialogPrepareInfoText;
            signingPackageInfoText = ls.PackageAddDialogSigningInfoText;
            initializingConfigInfoText = ls.PackageAddDialogConfigInitializerInfoText;
            uploadingPackageInfoText = ls.PackageAddDialogUploadingPackageInfoText;
            uploadingConfigInfoText = ls.PackageAddDialogUploadingConfigInfoText;

            Text = String.Format(ls.PackageAddDialogTitle, Project.Name, ls.ProductTitle);
            cancelButton.Text = ls.CancelButtonText;
            createButton.Text = ls.CreatePackageButtonText;

            devStageLabel.Text = ls.PackageAddDialogDevelopmentalStageLabelText;
            versionLabel.Text = ls.PackageAddDialogVersionLabelText;
            descriptionLabel.Text = ls.PackageAddDialogDescriptionLabelText;
            publishCheckBox.Text = ls.PackageAddDialogPublishCheckBoxText;
            publishInfoLabel.Text = ls.PackageAddDialogPublishInfoLabelText;
            environmentLabel.Text = ls.PackageAddDialogEnvironmentLabelText;
            environmentInfoLabel.Text = ls.PackageAddDialogEnvironmentInfoLabelText;

            changelogLoadButton.Text = ls.PackageAddDialogLoadButtonText;
            changelogClearButton.Text = ls.PackageAddDialogClearButtonText;

            addFilesButton.Text = ls.PackageAddDialogAddFileButtonText;
            removeEntryButton.Text = ls.PackageAddDialogRemoveFileButtonText;
            filesList.Columns[0].Text = ls.PackageAddDialogNameHeaderText;
            filesList.Columns[1].Text = ls.PackageAddDialogSizeHeaderText;

            allVersionsRadioButton.Text = ls.PackageAddDialogAvailableForAllRadioButtonText;
            someVersionsRadioButton.Text = ls.PackageAddDialogAvailableForSomeRadioButtonText;
            allVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForAllInfoText;
            someVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForSomeInfoText;
        }

        #endregion

        /// <summary>
        ///     Initializes the tree nodes for the update operations.
        /// </summary>
        private void InitializeOperationNodes()
        {
            replaceNode.Tag = "ReplaceFile";
            deleteNode.Tag = "DeleteFile";
            renameNode.Tag = "RenameFile";
            createRegistryEntryNode.Tag = "CreateRegistryEntry";
            deleteRegistryEntryNode.Tag = "DeleteRegistryEntry";
            setRegistryKeyValueNode.Tag = "SetRegistryKeyValue";
            startProcessNode.Tag = "StartProcess";
            terminateProcessNode.Tag = "StopProcess";
            startServiceNode.Tag = "StartService";
            stopServiceNode.Tag = "StopService";
        }

        /// <summary>
        ///     Initializes the FTP-data.
        /// </summary>
        private void InitializeFtpData()
        {
            try
            {
                ftp.FtpServer = Project.FtpHost;
                ftp.FtpPort = Project.FtpPort;
                ftp.FtpUserName = Project.FtpUsername;

                if (Settings.Default.SaveCredentials)
                {
                    var ftpPassword = new SecureString();
                    foreach (char sign in Project.FtpPassword)
                    {
                        ftpPassword.AppendChar(sign);
                    }
                    ftp.FtpPassword = ftpPassword;
                }

                try
                {
                    if (Project.FtpProtocol == "FTP")
                        ftp.Protocol = FTPProtocol.NormalFtp;
                    else if (Project.FtpProtocol == "FTP/SSL")
                        ftp.Protocol = FTPProtocol.SecureFtp;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while reading FTP-data.", ex, PopupButtons.OK);
                    hasFailedOnLoading = true;
                    return;
                }

                ftp.FtpModeUsePassive = Project.FtpUsePassiveMode;
                ftp.Directory = Project.FtpDirectory;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.",
                    "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
        }

        private void PackageAddDialog_Load(object sender, EventArgs e)
        {
            Project.HasUnsavedChanges = false;
            ftp.ProgressChanged += ProgressChanged;

            updateLog.Project = Project;
            Operations = new List<Operation>();

            InitializeFtpData();
            if (hasFailedOnLoading)
            {
                // TODO: Failed to load FTP-data. *Take the code from ProjectDialog*
                Close();
                return;
            }

            SetLanguage();
            InitializeOperationNodes();

            categoryTreeView.Nodes[3].Nodes.Add(replaceNode);
            categoryTreeView.Nodes[3].Toggle();

            architectureComboBox.SelectedIndex = 2;
            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            developmentalStageComboBox.SelectedIndex = 2;
            unsupportedVersionsPanel.Enabled = false;

            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, noNetworkCaption, noNetworkText, PopupButtons.OK);

                publishCheckBox.Checked = false;
                publishCheckBox.Enabled = false;
                publishInfoLabel.ForeColor = Color.Gray;
            }

            publishUpdate = publishCheckBox.Checked;
            mustUpdate = mustUpdateCheckBox.Checked;
            majorNumericUpDown.Minimum = NewestVersion.Major;
            minorNumericUpDown.Value = NewestVersion.Minor;
            buildNumericUpDown.Value = NewestVersion.Build;
            revisionNumericUpDown.Value = NewestVersion.Revision;

            if (!String.IsNullOrEmpty(Project.AssemblyVersionPath))
            {
                Assembly projectAssembly = Assembly.LoadFile(Project.AssemblyVersionPath);
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                var assemblyVersion = new UpdateVersion(info.FileVersion);

                majorNumericUpDown.Value = assemblyVersion.Major;
                minorNumericUpDown.Value = assemblyVersion.Minor;
                buildNumericUpDown.Value = assemblyVersion.Build;
                revisionNumericUpDown.Value = assemblyVersion.Revision;
            }

            generalTabPage.DoubleBuffer();
        }

        private void PackageAddDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUIState(bool enabled)
        {
            if (enabled)
                allowCancel = true;

            Invoke(new Action(() =>
            {
                foreach (Control c in Controls)
                {
                    if (c.Visible)
                        c.Enabled = enabled;
                }

                loadingPanel.Visible = !enabled;
            }));
        }

        /// <summary>
        ///     Resets all the package options set.
        /// </summary>
        private void Reset()
        {
            Directory.Delete(Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString()), true);
                // Directory

            zip.Dispose(); // Zip-file
            zip = new ZipFile();

            configuration = null; // Configuration
            configuration = new UpdateConfiguration();

            Project.Packages.Remove(package); // Remove the saved package again
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.Nodes[0].Nodes.Count == 0)
            {
                Popup.ShowPopup(this, SystemIcons.Error, noFilesCaption, noFilesText, PopupButtons.OK);
                filesPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[3].Nodes[0];
                return;
            }

            if (DevStage == DevelopmentalStage.Release)
            {
                packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value);
            }
            else
            {
                packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value, DevStage,
                    (int) developmentBuildNumericUpDown.Value);
            }

            if (Project.Packages != null && Project.Packages.Count != 0)
            {
                var equalItems = new List<UpdateVersion>();
                Project.Packages.ForEach(item => equalItems.Add(item.Version));
                if (packageVersion <= UpdateVersion.GetHighestUpdateVersion(equalItems))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, invalidVersionCaption,
                        String.Format(
                            "Version \"{0}\" is whether already existing or older than the newest one released.",
                            packageVersion.FullText), PopupButtons.OK);
                    generalPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                    return;
                }
            }

            if (String.IsNullOrEmpty(changelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, noChangelogCaption, noChangelogText, PopupButtons.OK);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            allowCancel = false;
            SetUIState(false);

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = new CancellationTokenSource();
            }

            ThreadPool.QueueUserWorkItem(delegate { InitializePackage(); }, null);
        }

        /// <summary>
        ///     Initializes the contents for the archive.
        /// </summary>
        /// <param name="nodeIndex">The current index of the node where the entries should be taken from.</param>
        /// <param name="currentDirectory">The current directory in the archive to paste the entries.</param>
        private void InitializeArchiveContents(TreeNode treeNode, string currentDirectory)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                FileAttributes attr = File.GetAttributes(node.Tag.ToString());
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string tmpDir = string.Format("{0}/{1}", currentDirectory, node.Text);
                    try
                    {
                        zip.AddDirectoryByName(tmpDir);
                        InitializeArchiveContents(node, tmpDir);
                    }
                    catch (ArgumentException)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        String.Format(
                                            "The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.",
                                            node.Text), PopupButtons.OK)));
                    }
                }
                else
                {
                    try
                    {
                        zip.AddFile(node.Tag.ToString(), currentDirectory);
                    }
                    catch (ArgumentException)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        String.Format(
                                            "The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.",
                                            node.Text), PopupButtons.OK)));
                    }
                }
            }
        }

        /// <summary>
        ///     Initializes the update package and uploads it, if set.
        /// </summary>
        private void InitializePackage()
        {
            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            remoteInfoFileUrl = UriConnecter.ConnectUri(Project.UpdateUrl, "updates.json");
            packageFolder = Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString());
            updateConfigFile = Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString(),
                "updates.json");

            Invoke(new Action(() => loadingLabel.Text = initializingArchiveInfoText));

            // Save the package first
            // ----------------------

            try
            {
                Directory.CreateDirectory(packageFolder); // Create the content folder
                using (FileStream fs = File.Create(updateConfigFile))
                {
                }
                ;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating local package data.", ex,
                                PopupButtons.OK)));
                cancellationToken.Cancel();
            }

            if (cancellationToken.IsCancellationRequested)
            {
                SetUIState(true);
                Reset();
                return;
            }

            zip.AddDirectoryByName("Program");
            zip.AddDirectoryByName("AppData");
            zip.AddDirectoryByName("Temp");
            zip.AddDirectoryByName("Desktop");

            InitializeArchiveContents(filesDataTreeView.Nodes[0], "Program");
            InitializeArchiveContents(filesDataTreeView.Nodes[1], "AppData");
            InitializeArchiveContents(filesDataTreeView.Nodes[2], "Temp");
            InitializeArchiveContents(filesDataTreeView.Nodes[3], "Desktop");

            string packageFile = String.Format("{0}.zip", Project.Id);
            zip.Save(Path.Combine(packageFolder, packageFile));

            updateLog.Write(LogEntry.Create, packageVersion.ToString());
            Invoke(new Action(() => loadingLabel.Text = preparingUpdateInfoText));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersions = null;

            if (unsupportedVersionsList.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsList.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                var itemArray = new List<string>();
                foreach (string item in unsupportedVersionsList.Items)
                {
                    itemArray.Add(item);
                }

                unsupportedVersions = itemArray.ToArray();
            }

            // Create a new package configuration
            configuration.Changelog = changelogTextBox.Text;
            configuration.MustUpdate = mustUpdateCheckBox.Checked;

            switch (architectureIndex)
            {
                case 0:
                    configuration.Architecture = "x86";
                    break;
                case 1:
                    configuration.Architecture = "x64";
                    break;
                case 2:
                    configuration.Architecture = "AnyCPU";
                    break;
            }

            Invoke(new Action(() => loadingLabel.Text = signingPackageInfoText));

            try
            {
                byte[] data = File.ReadAllBytes(Path.Combine(packageFolder, String.Format("{0}.zip", Project.Id)));
                configuration.Signature = Convert.ToBase64String(new RsaSignature(Project.PrivateKey).SignData(data));
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while signing the package.", ex,
                                PopupButtons.OK)));
                cancellationToken.Cancel();
            }

            if (cancellationToken.IsCancellationRequested)
            {
                SetUIState(true);
                Reset();
                return;
            }

            configuration.UnsupportedVersions = unsupportedVersions;
            configuration.UpdatePhpFileUrl = UriConnecter.ConnectUri(Project.UpdateUrl, "getfile.php");
                // Get the URL of the PHP-file
            configuration.RelativePackageUri = String.Format("{0}/{1}.zip", packageVersion, Project.Id);
            configuration.Version = packageVersion.ToString();

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => loadingLabel.Text = initializingConfigInfoText));

            var configurationList = new List<UpdateConfiguration>();

            // Load the configuration
            try
            {
                configurationList = UpdateConfiguration.DownloadUpdateConfiguration(remoteInfoFileUrl, Project.Proxy);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the old configuration.", ex,
                                PopupButtons.OK)));
                cancellationToken.Cancel();
            }

            if (cancellationToken.IsCancellationRequested)
            {
                SetUIState(true);
                Reset();
                return;
            }

            if (configurationList == null)
                configurationList = new List<UpdateConfiguration>();

            configurationList.Add(configuration);
            File.WriteAllText(updateConfigFile, Serializer.Serialize(configurationList));

            /* ------------- Save package info  ------------- */
            Invoke(new Action(() => package.Description = descriptionTextBox.Text));
            package.IsReleased = publishUpdate;
            package.LocalPackagePath = Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString(),
                String.Format("{0}.zip", Project.Id));
            package.Version = packageVersion;

            if (Project.Packages == null)
                Project.Packages = new List<UpdatePackage>();
            Project.Packages.Add(package);

            /* ------------ Upload ----------- */
            if (publishUpdate)
            {
                if (!Settings.Default.SaveCredentials)
                {
                    Invoke(new Action(() =>
                    {
                        var credentialsForm = new CredentialsDialog();
                        if (credentialsForm.ShowDialog() == DialogResult.OK)
                        {
                            ftp.FtpUserName = credentialsForm.Username;
                            ftp.FtpPassword = credentialsForm.Password;
                        }
                        else
                            cancellationToken.Cancel();
                    }));
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    SetUIState(true);
                    Reset();
                    return;
                }

                /* -------------- Package upload -----------------*/
                Invoke(new Action(() => loadingLabel.Text = String.Format(uploadingPackageInfoText, "0%")));
                try
                {
                    ftp.UploadPackage(
                        Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString(),
                            String.Format("{0}.zip", Project.Id)), packageVersion.ToString());
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.",
                                    ex, PopupButtons.OK)));
                    cancellationToken.Cancel();
                }

                while (!ftp.HasFinishedUploading)
                {
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    SetUIState(true);
                    Reset();
                    return;
                }

                if (ftp.PackageUploadException != null)
                {
                    if (ftp.PackageUploadException.InnerException != null)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        ftp.PackageUploadException.InnerException, PopupButtons.OK)));
                        cancellationToken.Cancel();
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    SetUIState(true);
                    Reset();
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = uploadingConfigInfoText));
                try
                {
                    ftp.UploadFile(updateConfigFile);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.", ex,
                            PopupButtons.OK);
                        loadingLabel.Text = "Undoing changes...";
                    }));

                    try
                    {
                        ftp.DeleteDirectory(packageVersion.ToString());
                        updateLog.Write(LogEntry.Delete, packageVersion.ToString());
                    }
                    catch (Exception deletingEx)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                        deletingEx, PopupButtons.OK)));
                        cancellationToken.Cancel();
                    }
                }

                updateLog.Write(LogEntry.Upload, packageVersion.ToString());
            }

            if (cancellationToken.IsCancellationRequested)
            {
                SetUIState(true);
                Reset();
            }
            else
            {
                if (publishUpdate)
                {
                    Project.NewestPackage = packageVersion.FullText;
                    Project.ReleasedPackages += 1;
                }

                // Has now unsaved changes we need to save
                Project.HasUnsavedChanges = true;

                Invoke(new Action(() =>
                {
                    SetUIState(true);
                    DialogResult = DialogResult.OK;
                    Close();
                }));
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            String.Format(uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage * 2))));
        }

        private void changelogLoadButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.SupportMultiDottedExtensions = false;
                ofd.Multiselect = false;

                ofd.Filter = "Textdocument (*.txt)|*.txt|RTF-Document (*.rtf)|*.rtf";

                if (ofd.ShowDialog() == DialogResult.OK)
                    changelogTextBox.Text = File.ReadAllText(ofd.FileName, Encoding.Default);
            }
        }

        private void changelogClearButton_Click(object sender, EventArgs e)
        {
            changelogTextBox.Clear();
        }

        /// <summary>
        ///     Lists the directory content recursively.
        /// </summary>
        private void ListDirectory(TreeView treeView, string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            TreeNode selectedNode = filesDataTreeView.SelectedNode;
            selectedNode.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        /// <summary>
        ///     Creates a new subnode for the corresponding directory info.
        /// </summary>
        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name, 2, 2);
            directoryNode.Tag = directoryInfo.FullName;
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name, 1, 1);
                fileNode.Tag = file.FullName;
                directoryNode.Nodes.Add(fileNode);
            }

            return directoryNode;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (filesDataTreeView.SelectedNode != null)
                    {
                        ListDirectory(filesDataTreeView, folderDialog.SelectedPath);
                        if (!filesDataTreeView.SelectedNode.IsExpanded)
                            filesDataTreeView.SelectedNode.Toggle();
                    }
                }
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode != null)
            {
                using (var fileDialog = new OpenFileDialog())
                {
                    fileDialog.SupportMultiDottedExtensions = true;
                    fileDialog.Multiselect = true;
                    fileDialog.Filter = "All Files (*.*)| *.*";

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string path in fileDialog.FileNames)
                        {
                            var node = new TreeNode(Path.GetFileName(path));
                            node.Tag = path;
                            node.ImageIndex = 1;
                            node.SelectedImageIndex = 1;
                            filesDataTreeView.SelectedNode.Nodes.Add(node);

                            if (!filesDataTreeView.SelectedNode.IsExpanded)
                                filesDataTreeView.SelectedNode.Toggle();
                        }
                    }
                }
            }
        }

        private void removeEntryButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode != null && filesDataTreeView.SelectedNode.Parent != null)
                filesDataTreeView.SelectedNode.Remove();
        }

        private void someVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = true;
        }

        private void allVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = false;
        }

        private void addVersionButton_Click(object sender, EventArgs e)
        {
            string version = unsupportedMajorNumericUpDown.Value + "." + unsupportedMinorNumericUpDown.Value + "." +
                             unsupportedBuildNumericUpDown.Value + "." + unsupportedRevisionNumericUpDown.Value;
            unsupportedVersionsList.Items.Add(version);
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            unsupportedVersionsList.Items.Remove(unsupportedVersionsList.SelectedItem);
        }

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevStage =
                (DevelopmentalStage)
                    Enum.Parse(typeof (DevelopmentalStage),
                        developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem));
            if (DevStage == DevelopmentalStage.Alpha || DevStage == DevelopmentalStage.Beta)
                developmentBuildNumericUpDown.Enabled = true;
            else
                developmentBuildNumericUpDown.Enabled = false;
        }

        private void publishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            publishUpdate = publishCheckBox.Checked;
        }

        private void mustUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mustUpdate = mustUpdateCheckBox.Checked;
        }

        private void architectureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            architectureIndex = architectureComboBox.SelectedIndex;
        }

        private void preReleaseVersionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            developmentBuild = (int) developmentBuildNumericUpDown.Value;
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (categoryTreeView.SelectedNode.Parent == null) // Check if the selected node is an operation
            {
                switch (categoryTreeView.SelectedNode.Index)
                {
                    case 0:
                        tablessTabControl1.SelectedTab = generalTabPage;
                        break;
                    case 1:
                        tablessTabControl1.SelectedTab = changelogTabPage;
                        break;
                    case 2:
                        tablessTabControl1.SelectedTab = availabilityTabPage;
                        break;
                    case 3:
                        tablessTabControl1.SelectedTab = operationsTabPage;
                        break;
                }
            }
            else
            {
                switch (categoryTreeView.SelectedNode.Tag.ToString())
                {
                    case "ReplaceFile":
                        tablessTabControl1.SelectedTab = replaceFilesTabPage;
                        break;
                    case "DeleteFile":
                        tablessTabControl1.SelectedTab =
                            tablessTabControl1.TabPages[4 + categoryTreeView.SelectedNode.Index];
                        break;
                    case "RenameFile":
                        tablessTabControl1.SelectedTab =
                            tablessTabControl1.TabPages[4 + categoryTreeView.SelectedNode.Index];
                        break;
                }
            }
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode != null)
            {
                if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) && categoryTreeView.SelectedNode.Parent != null &&
                    categoryTreeView.SelectedNode.Text != "Replace file/folder")
                {
                    tablessTabControl1.TabPages.Remove(
                        tablessTabControl1.TabPages[4 + categoryTreeView.SelectedNode.Index]);
                    categoryTreeView.SelectedNode.Remove();
                }
            }
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 3) // Operations-node
                return;

            object data = e.Data.GetData(typeof (string));
            if (data == null)
                return;

            switch (data.ToString())
            {
                case "DeleteFile":
                    var newDeleteNode = (TreeNode) deleteNode.Clone();
                    categoryTreeView.Nodes[3].Nodes.Add(newDeleteNode);

                    var deletePage = new TabPage("Delete file");
                    deletePage.BackColor = SystemColors.Window;
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    tablessTabControl1.TabPages.Add(deletePage);
                    break;

                case "RenameFile":
                    var newRenameNode = (TreeNode) renameNode.Clone();
                    categoryTreeView.Nodes[3].Nodes.Add(newRenameNode);

                    var renamePage = new TabPage("Rename file");
                    renamePage.BackColor = SystemColors.Window;
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    tablessTabControl1.TabPages.Add(renamePage);
                    break;

                case "CreateRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) createRegistryEntryNode.Clone());
                    break;

                case "DeleteRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) deleteRegistryEntryNode.Clone());
                    break;

                case "SetRegistryKeyValue":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) setRegistryKeyValueNode.Clone());
                    break;
                case "StartProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) startProcessNode.Clone());
                    break;
                case "TerminateProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) terminateProcessNode.Clone());
                    break;
                case "StartService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) startServiceNode.Clone());
                    break;
                case "StopService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) stopServiceNode.Clone());
                    break;
            }
        }

        private void categoryTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void operationsListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (operationsListView.SelectedItems.Count > 0)
                operationsListView.DoDragDrop(operationsListView.SelectedItems[0].Tag, DragDropEffects.Move);
        }

        private void operationsListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}