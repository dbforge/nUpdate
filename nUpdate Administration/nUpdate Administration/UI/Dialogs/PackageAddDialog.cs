using Ionic.Zip;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.History;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Dialogs;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddDialog : BaseDialog
    {
        private const string separatorCharacter = "-";
        private const float Kb = 1024;
        private const float Mb = 1048577;

        private bool publishUpdate;
        private bool mustUpdate;
        private bool allowCancel = true;
        private bool hasFailedOnLoading;

        private UpdatePackage package = new UpdatePackage();
        private UpdateConfiguration configuration = new UpdateConfiguration();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private FTPManager ftp = new FTPManager();
        private Log updateLog = new Log();
        private ZipFile zip = new ZipFile();

        private TreeNode replaceNode = new TreeNode("Replace file/folder", 11, 11);
        private TreeNode deleteNode = new TreeNode("Delete file", 9, 9);
        private TreeNode renameNode = new TreeNode("Rename file", 10, 10);
        private TreeNode createRegistryEntryNode = new TreeNode("Create registry entry");
        private TreeNode deleteRegistryEntryNode = new TreeNode("Delete registry entry");
        private TreeNode setRegistryKeyValueNode = new TreeNode("Set registry key value");
        private TreeNode startProcessNode = new TreeNode("Start process", 8, 8);
        private TreeNode terminateProcessNode = new TreeNode("Terminate process", 7, 7);
        private TreeNode startServiceNode = new TreeNode("Start service", 5, 5);
        private TreeNode stopServiceNode = new TreeNode("Stop service", 6, 6);

        private UpdateVersion packageVersion;
        private string packageFolder;
        private string updateConfigFile;
        private Uri remoteInfoFileUrl;

        private int developmentBuild = 1;
        private int architectureIndex = 2;

        /// <summary>
        /// The newest package version.
        /// </summary>
        internal UpdateVersion NewestVersion { get; set; }

        /// <summary>
        /// Sets the developmental stage of the package.
        /// </summary>
        public DevelopmentalStage DevStage { get; set; }

        /// <summary>
        /// Returns all operations set.
        /// </summary>
        internal List<Operation> Operations { get; set; }

        #region "Localization"

        private string noNetworkCaption;
        private string noNetworkText;
        private string noFilesCaption;
        private string noFilesText;
        private string unsupportedArchiveCaption;
        private string unsupportedArchiveText;
        private string invalidVersionCaption;
        private string invalidVersionText;
        private string noChangelogCaption;
        private string noChangelogText;
        private string invalidArgumentCaption;
        private string invalidArgumentText;
        private string creatingPackageDataErrorCaption;
        private string loadingProjectDataErrorCaption;
        private string gettingUrlErrorCaption;
        private string readingPackageBytesErrorCaption;
        private string invalidServerDirectoryErrorCaption;
        private string invalidServerDirectoryErrorText;
        private string ftpDataLoadErrorCaption;
        private string configDownloadErrorCaption;
        private string serializingDataErrorCaption;
        private string relativeUriErrorText;
        private string savingInformationErrorCaption;
        private string uploadFailedErrorCaption;
        private string initializingArchiveInfoText;
        private string preparingUpdateInfoText;
        private string signingPackageInfoText;
        private string initializingConfigInfoText;
        private string uploadingPackageInfoText;
        private string uploadingConfigInfoText;

        private void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", Properties.Settings.Default.Language.Name));
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(languageFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.noNetworkCaption = ls.PackageAddDialogNoInternetWarningCaption;
            this.noNetworkText = ls.PackageAddDialogNoInternetWarningText;
            this.noFilesCaption = ls.PackageAddDialogNoFilesSpecifiedWarningCaption;
            this.noFilesText = ls.PackageAddDialogNoFilesSpecifiedWarningText;
            this.unsupportedArchiveCaption = ls.PackageAddDialogUnsupportedArchiveWarningCaption;
            this.unsupportedArchiveText = ls.PackageAddDialogUnsupportedArchiveWarningText;
            this.invalidVersionCaption = ls.PackageAddDialogVersionInvalidWarningCaption;
            this.invalidVersionText = ls.PackageAddDialogVersionInvalidWarningText;
            this.noChangelogCaption = ls.PackageAddDialogNoChangelogWarningCaption;
            this.noChangelogText = ls.PackageAddDialogNoChangelogWarningText;
            this.invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
            this.invalidArgumentText = ls.InvalidArgumentErrorText;
            this.creatingPackageDataErrorCaption = ls.PackageAddDialogPackageDataCreationErrorCaption;
            this.loadingProjectDataErrorCaption = ls.PackageAddDialogProjectDataLoadingErrorCaption;
            this.gettingUrlErrorCaption = ls.PackageAddDialogGettingUrlErrorCaption;
            this.readingPackageBytesErrorCaption = ls.PackageAddDialogReadingPackageBytesErrorCaption;
            this.invalidServerDirectoryErrorCaption = ls.PackageAddDialogInvalidServerDirectoryErrorCaption;
            this.invalidServerDirectoryErrorText = ls.PackageAddDialogInvalidServerDirectoryErrorText;
            this.ftpDataLoadErrorCaption = ls.PackageAddDialogLoadingFtpDataErrorCaption;
            this.configDownloadErrorCaption = ls.PackageAddDialogConfigurationDownloadErrorCaption;
            this.serializingDataErrorCaption = ls.PackageAddDialogSerializingDataErrorCaption;
            this.relativeUriErrorText = ls.PackageAddDialogRelativeUriErrorText;
            this.savingInformationErrorCaption = ls.PackageAddDialogPackageInformationSavingErrorCaption;
            this.uploadFailedErrorCaption = ls.PackageAddDialogUploadFailedErrorCaption;

            this.initializingArchiveInfoText = ls.PackageAddDialogArchiveInitializerInfoText;
            this.preparingUpdateInfoText = ls.PackageAddDialogPrepareInfoText;
            this.signingPackageInfoText = ls.PackageAddDialogSigningInfoText;
            this.initializingConfigInfoText = ls.PackageAddDialogConfigInitializerInfoText;
            this.uploadingPackageInfoText = ls.PackageAddDialogUploadingPackageInfoText;
            this.uploadingConfigInfoText = ls.PackageAddDialogUploadingConfigInfoText;

            this.Text = String.Format(ls.PackageAddDialogTitle, this.Project.Name, ls.ProductTitle);
            this.cancelButton.Text = ls.CancelButtonText;
            this.createButton.Text = ls.CreatePackageButtonText;

            this.devStageLabel.Text = ls.PackageAddDialogDevelopmentalStageLabelText;
            this.versionLabel.Text = ls.PackageAddDialogVersionLabelText;
            this.descriptionLabel.Text = ls.PackageAddDialogDescriptionLabelText;
            this.publishCheckBox.Text = ls.PackageAddDialogPublishCheckBoxText;
            this.publishInfoLabel.Text = ls.PackageAddDialogPublishInfoLabelText;
            this.environmentLabel.Text = ls.PackageAddDialogEnvironmentLabelText;
            this.environmentInfoLabel.Text = ls.PackageAddDialogEnvironmentInfoLabelText;

            this.changelogLoadButton.Text = ls.PackageAddDialogLoadButtonText;
            this.changelogClearButton.Text = ls.PackageAddDialogClearButtonText;

            this.addFilesButton.Text = ls.PackageAddDialogAddFileButtonText;
            this.removeEntryButton.Text = ls.PackageAddDialogRemoveFileButtonText;
            this.filesList.Columns[0].Text = ls.PackageAddDialogNameHeaderText;
            this.filesList.Columns[1].Text = ls.PackageAddDialogSizeHeaderText;

            this.allVersionsRadioButton.Text = ls.PackageAddDialogAvailableForAllRadioButtonText;
            this.someVersionsRadioButton.Text = ls.PackageAddDialogAvailableForSomeRadioButtonText;
            this.allVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForAllInfoText;
            this.someVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForSomeInfoText;
        }

        #endregion

        public PackageAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the tree nodes for the update operations.
        /// </summary>
        private void InitializeOperationNodes()
        {
            this.replaceNode.Tag = "ReplaceFile";
            this.deleteNode.Tag = "DeleteFile";
            this.renameNode.Tag = "RenameFile";
            this.createRegistryEntryNode.Tag = "CreateRegistryEntry";
            this.deleteRegistryEntryNode.Tag = "DeleteRegistryEntry";
            this.setRegistryKeyValueNode.Tag = "SetRegistryKeyValue";
            this.startProcessNode.Tag = "StartProcess";
            this.terminateProcessNode.Tag = "StopProcess";
            this.startServiceNode.Tag = "StartService";
            this.stopServiceNode.Tag = "StopService";
        }

        /// <summary>
        /// Initializes the FTP-data.
        /// </summary>
        private void InitializeFtpData()
        {
            try
            {
                this.ftp.FtpServer = this.Project.FtpHost;
                this.ftp.FtpPort = this.Project.FtpPort;
                this.ftp.FtpUserName = this.Project.FtpUsername;

                if (Properties.Settings.Default.SaveCredentials)
                {
                    SecureString ftpPassword = new SecureString();
                    foreach (char sign in this.Project.FtpPassword)
                    {
                        ftpPassword.AppendChar(sign);
                    }
                    this.ftp.FtpPassword = ftpPassword;
                }

                try
                {
                    if (this.Project.FtpProtocol == "FTP")
                    {
                        this.ftp.Protocol = FTPProtocol.NormalFtp;
                    }
                    else if (this.Project.FtpProtocol == "FTP/SSL")
                    {
                        this.ftp.Protocol = FTPProtocol.SecureFtp;
                    }
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while reading FTP-data.", ex, PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                    return;
                }

                this.ftp.FtpModeUsePassive = this.Project.FtpUsePassiveMode;
                this.ftp.Directory = this.Project.FtpDirectory;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
        }

        private void PackageAddDialog_Load(object sender, EventArgs e)
        {
            this.Project.HasUnsavedChanges = false;
            this.ftp.ProgressChanged += ProgressChanged;

            this.updateLog.Project = this.Project;
            this.Operations = new List<Operation>();

            this.InitializeFtpData();
            if (hasFailedOnLoading)
            {
                // TODO: Failed to load FTP-data. *Take the code from ProjectDialog*
                this.Close();
                return;
            }

            this.SetLanguage();
            this.InitializeOperationNodes();

            this.categoryTreeView.Nodes[3].Nodes.Add(replaceNode);
            this.categoryTreeView.Nodes[3].Toggle();

            this.architectureComboBox.SelectedIndex = 2;
            this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[0];
            this.developmentalStageComboBox.SelectedIndex = 2;
            this.unsupportedVersionsPanel.Enabled = false;

            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.noNetworkCaption, this.noNetworkText, PopupButtons.OK);

                this.publishCheckBox.Checked = false;
                this.publishCheckBox.Enabled = false;
                this.publishInfoLabel.ForeColor = Color.Gray;
            }

            this.publishUpdate = this.publishCheckBox.Checked;
            this.mustUpdate = this.mustUpdateCheckBox.Checked;
            this.majorNumericUpDown.Minimum = this.NewestVersion.Major;
            this.minorNumericUpDown.Value = this.NewestVersion.Minor;
            this.buildNumericUpDown.Value = this.NewestVersion.Build;
            this.revisionNumericUpDown.Value = this.NewestVersion.Revision;

            if (!String.IsNullOrEmpty(this.Project.AssemblyVersionPath))
            {
                Assembly projectAssembly = Assembly.LoadFile(this.Project.AssemblyVersionPath);
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                UpdateVersion assemblyVersion = new UpdateVersion(info.FileVersion);

                this.majorNumericUpDown.Value = assemblyVersion.Major;
                this.minorNumericUpDown.Value = assemblyVersion.Minor;
                this.buildNumericUpDown.Value = assemblyVersion.Build;
                this.revisionNumericUpDown.Value = assemblyVersion.Revision;
            }

            this.generalTabPage.DoubleBuffer();
        }

        private void PackageAddDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.allowCancel)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUIState(bool enabled)
        {
            if (enabled == true)
            {
                this.allowCancel = true;
            }

            Invoke(new Action(() => 
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c.Visible)
                        {
                            c.Enabled = enabled;
                        }
                    }

                    this.loadingPanel.Visible = !enabled;
                }));
        }

        /// <summary>
        /// Resets all the package options set.
        /// </summary>
        private void Reset()
        {
            Directory.Delete(Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString()), true); // Directory

            this.zip.Dispose(); // Zip-file
            this.zip = new ZipFile();

            this.configuration = null; // Configuration
            this.configuration = new UpdateConfiguration();

            this.Project.Packages.Remove(this.package); // Remove the saved package again
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.Nodes[0].Nodes.Count == 0)
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.noFilesCaption, this.noFilesText, PopupButtons.OK);
                this.filesPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[3].Nodes[0];
                return;
            }

            if (this.DevStage == DevelopmentalStage.Release)
            {
                this.packageVersion = new UpdateVersion((int)this.majorNumericUpDown.Value, (int)this.minorNumericUpDown.Value, (int)this.buildNumericUpDown.Value, (int)this.revisionNumericUpDown.Value);
            }
            else
            {
                this.packageVersion = new UpdateVersion((int)this.majorNumericUpDown.Value, (int)this.minorNumericUpDown.Value, (int)this.buildNumericUpDown.Value, (int)this.revisionNumericUpDown.Value, this.DevStage, (int)this.developmentBuildNumericUpDown.Value);
            }

            if (this.Project.Packages != null && this.Project.Packages.Count != 0)
            {
                List<UpdateVersion> equalItems = new List<UpdateVersion>();
                this.Project.Packages.ForEach(item => equalItems.Add(item.Version));
                if (this.packageVersion <= UpdateVersion.GetHighestUpdateVersion(equalItems))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, this.invalidVersionCaption, String.Format("Version \"{0}\" is whether already existing or older than the newest one released.", this.packageVersion.FullText), PopupButtons.OK);
                    this.generalPanel.BringToFront();
                    this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[0];
                    return;
                }
            }

            if (String.IsNullOrEmpty(this.changelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.noChangelogCaption, this.noChangelogText, PopupButtons.OK);
                this.changelogPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[1];
                return;
            }

            this.allowCancel = false;
            this.SetUIState(false);

            this.loadingPanel.Location = new Point(180, 91);
            this.loadingPanel.BringToFront();
            this.loadingPanel.Visible = true;

            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
                this.cancellationToken = new CancellationTokenSource();
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                { this.InitializePackage(); }), null);
        }

        /// <summary>
        /// Initializes the contents for the archive.
        /// </summary>
        /// <param name="nodeIndex">The current index of the node where the entries should be taken from.</param>
        /// <param name="currentDirectory">The current directory in the archive to paste the entries.</param>
        private void InitializeArchiveContents(TreeNode treeNode, string currentDirectory)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                var attr = File.GetAttributes(node.Tag.ToString());
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var tmpDir = string.Format("{0}/{1}", currentDirectory, node.Text);
                    try
                    {
                        zip.AddDirectoryByName(tmpDir);
                        InitializeArchiveContents(node, tmpDir);
                    }
                    catch (ArgumentException)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.", String.Format("The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.", node.Text), PopupButtons.OK)));
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
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.", String.Format("The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.", node.Text), PopupButtons.OK)));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the update package and uploads it, if set.
        /// </summary>
        private void InitializePackage()
        {
            if (!this.Project.UpdateUrl.EndsWith("/"))
            {
                this.Project.UpdateUrl += "/";
            }
            this.remoteInfoFileUrl = UriConnecter.ConnectUri(this.Project.UpdateUrl, "updates.json");
            this.packageFolder = Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString());
            this.updateConfigFile = Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString(), "updates.json");

            Invoke(new Action(() => this.loadingLabel.Text = this.initializingArchiveInfoText));

            // Save the package first
            // ----------------------

            try
            {
                Directory.CreateDirectory(this.packageFolder); // Create the content folder
                using (FileStream fs = File.Create(this.updateConfigFile)) { };
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating local package data.", ex, PopupButtons.OK)));
                this.cancellationToken.Cancel();
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                this.SetUIState(true);
                this.Reset();
                return;
            }

            zip.AddDirectoryByName("Program");
            zip.AddDirectoryByName("AppData");
            zip.AddDirectoryByName("Temp");
            zip.AddDirectoryByName("Desktop");

            InitializeArchiveContents(this.filesDataTreeView.Nodes[0], "Program");
            InitializeArchiveContents(this.filesDataTreeView.Nodes[1], "AppData");
            InitializeArchiveContents(this.filesDataTreeView.Nodes[2], "Temp");
            InitializeArchiveContents(this.filesDataTreeView.Nodes[3], "Desktop");

            var packageFile = String.Format("{0}.zip", this.Project.Id);
            zip.Save(Path.Combine(packageFolder, packageFile));

            this.updateLog.Write(LogEntry.Create, this.packageVersion.ToString());
            Invoke(new Action(() => this.loadingLabel.Text = this.preparingUpdateInfoText));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersions = null;

            if (this.unsupportedVersionsList.Items.Count == 0)
            {
                this.allVersionsRadioButton.Checked = true;
            }
            else if (this.unsupportedVersionsList.Items.Count > 0 && this.someVersionsRadioButton.Checked)
            {
                List<string> itemArray = new List<string>();
                foreach (string item in this.unsupportedVersionsList.Items)
                {
                    itemArray.Add(item);
                }

                unsupportedVersions = itemArray.ToArray();
            }

            // Create a new package configuration
            this.configuration.Changelog = this.changelogTextBox.Text;
            this.configuration.MustUpdate = this.mustUpdateCheckBox.Checked;

            switch (this.architectureIndex)
            {
                case 0:
                    this.configuration.Architecture = "x86";
                    break;
                case 1:
                    this.configuration.Architecture = "x64";
                    break;
                case 2:
                    this.configuration.Architecture = "AnyCPU";
                    break;
            }

            Invoke(new Action(() => this.loadingLabel.Text = this.signingPackageInfoText));

            try
            {
                byte[] data = File.ReadAllBytes(Path.Combine(this.packageFolder, String.Format("{0}.zip", this.Project.Id)));
                this.configuration.Signature = Convert.ToBase64String(new RsaSignature(this.Project.PrivateKey).SignData(data));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while signing the package.", ex, PopupButtons.OK)));
                this.cancellationToken.Cancel();
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                this.SetUIState(true);
                this.Reset();
                return;
            }

            this.configuration.UnsupportedVersions = unsupportedVersions;
            this.configuration.UpdatePhpFileUrl = UriConnecter.ConnectUri(this.Project.UpdateUrl, "getfile.php"); // Get the URL of the PHP-file
            this.configuration.RelativePackageUri = String.Format("{0}/{1}.zip", this.packageVersion.ToString(), this.Project.Id);
            this.configuration.Version = this.packageVersion.ToString();

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => this.loadingLabel.Text = this.initializingConfigInfoText));

            var configurationList = new List<UpdateConfiguration>();

            // Load the configuration
            try
            { 
                configurationList = UpdateConfiguration.DownloadUpdateConfiguration(this.remoteInfoFileUrl, this.Project.Proxy);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the old configuration.", ex, PopupButtons.OK)));
                this.cancellationToken.Cancel();
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                this.SetUIState(true);
                this.Reset();
                return;
            }

            if (configurationList == null)
            {
                configurationList = new List<UpdateConfiguration>();
            }

            configurationList.Add(this.configuration);
            File.WriteAllText(this.updateConfigFile, Serializer.Serialize(configurationList));

            /* ------------- Save package info  ------------- */
            Invoke(new Action(() => this.package.Description = this.descriptionTextBox.Text));
            this.package.IsReleased = this.publishUpdate;
            this.package.LocalPackagePath = Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString(), String.Format("{0}.zip", Project.Id));
            this.package.Version = this.packageVersion;

            if (this.Project.Packages == null)
            {
                this.Project.Packages = new List<UpdatePackage>();
            }
            this.Project.Packages.Add(this.package);

            /* ------------ Upload ----------- */
            if (this.publishUpdate)
            {
                if (!Properties.Settings.Default.SaveCredentials)
                {
                    Invoke(new Action(() =>
                    {
                        var credentialsForm = new CredentialsDialog();
                        if (credentialsForm.ShowDialog() == DialogResult.OK)
                        {
                            this.ftp.FtpUserName = credentialsForm.Username;
                            this.ftp.FtpPassword = credentialsForm.Password;
                        }
                        else
                        {
                            cancellationToken.Cancel();
                        }
                    }));
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.SetUIState(true);
                    this.Reset();
                    return;
                }

                /* -------------- Package upload -----------------*/
                Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, "0%")));
                try
                {
                    this.ftp.UploadPackage(Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString(), String.Format("{0}.zip", this.Project.Id)), this.packageVersion.ToString());
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.", ex, PopupButtons.OK)));
                    this.cancellationToken.Cancel();
                }

                while (!this.ftp.HasFinishedUploading)
                {
                    continue;
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.SetUIState(true);
                    this.Reset();
                    return;
                }

                if (ftp.PackageUploadException != null)
                {
                    if (ftp.PackageUploadException.InnerException != null)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.", ftp.PackageUploadException.InnerException, PopupButtons.OK)));
                        this.cancellationToken.Cancel();
                    }
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    this.SetUIState(true);
                    this.Reset();
                    return;
                }

                Invoke(new Action(() => this.loadingLabel.Text = this.uploadingConfigInfoText));
                try
                {
                    this.ftp.UploadFile(this.updateConfigFile);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.", ex, PopupButtons.OK);
                            this.loadingLabel.Text = "Undoing changes...";
                        }));

                    try
                    {
                        this.ftp.DeleteDirectory(this.packageVersion.ToString());
                        this.updateLog.Write(LogEntry.Delete, this.packageVersion.ToString());
                    }
                    catch (Exception deletingEx)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.", deletingEx, PopupButtons.OK)));
                        this.cancellationToken.Cancel();
                    }
                }

                this.updateLog.Write(LogEntry.Upload, this.packageVersion.ToString());
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                this.SetUIState(true);
                this.Reset();
            }
            else
            {
                if (this.publishUpdate)
                {
                    this.Project.NewestPackage = packageVersion.FullText;
                    this.Project.ReleasedPackages += 1;
                }

                // Has now unsaved changes we need to save
                this.Project.HasUnsavedChanges = true;

                Invoke(new Action(() =>
                {
                    this.SetUIState(true);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }));
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage * 2))));
        }

        private void changelogLoadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.SupportMultiDottedExtensions = false;
                ofd.Multiselect = false;

                ofd.Filter = "Textdocument (*.txt)|*.txt|RTF-Document (*.rtf)|*.rtf";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.changelogTextBox.Text = File.ReadAllText(ofd.FileName, Encoding.Default);
                }
            }
        }

        private void changelogClearButton_Click(object sender, EventArgs e)
        {
            this.changelogTextBox.Clear();
        }

        /// <summary>
        /// Lists the directory content recursively.
        /// </summary>
        private void ListDirectory(TreeView treeView, string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            var selectedNode = filesDataTreeView.SelectedNode;
            selectedNode.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        /// <summary>
        /// Creates a new subnode for the corresponding directory info.
        /// </summary>
        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name, 2, 2);
            directoryNode.Tag = directoryInfo.FullName;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name, 1, 1);
                fileNode.Tag = file.FullName;
                directoryNode.Nodes.Add(fileNode);
            }
            
            return directoryNode;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (this.filesDataTreeView.SelectedNode != null)
                    {
                        this.ListDirectory(filesDataTreeView, folderDialog.SelectedPath);
                        if (!this.filesDataTreeView.SelectedNode.IsExpanded)
                        {
                            this.filesDataTreeView.SelectedNode.Toggle();
                        }
                    }
                }
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode != null)
            {
                using (OpenFileDialog fileDialog = new OpenFileDialog())
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
                            this.filesDataTreeView.SelectedNode.Nodes.Add(node);

                            if (!this.filesDataTreeView.SelectedNode.IsExpanded)
                            {
                                this.filesDataTreeView.SelectedNode.Toggle();
                            }
                        }
                    }
                }
            }
        }

        private void removeEntryButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode != null && filesDataTreeView.SelectedNode.Parent != null)
            {
                filesDataTreeView.SelectedNode.Remove();
            }
        }

        private void someVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.unsupportedVersionsPanel.Enabled = true;
        }

        private void allVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.unsupportedVersionsPanel.Enabled = false;
        }

        private void addVersionButton_Click(object sender, EventArgs e)
        {
            string version = this.unsupportedMajorNumericUpDown.Value.ToString() + "." + this.unsupportedMinorNumericUpDown.Value.ToString() + "." + this.unsupportedBuildNumericUpDown.Value.ToString() + "." + this.unsupportedRevisionNumericUpDown.Value.ToString();
            this.unsupportedVersionsList.Items.Add(version);
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            this.unsupportedVersionsList.Items.Remove(this.unsupportedVersionsList.SelectedItem);
        }

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DevStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), this.developmentalStageComboBox.GetItemText(this.developmentalStageComboBox.SelectedItem));
            if (DevStage == DevelopmentalStage.Alpha || DevStage == DevelopmentalStage.Beta)
                developmentBuildNumericUpDown.Enabled = true;
            else
                developmentBuildNumericUpDown.Enabled = false;
        }

        private void publishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.publishUpdate = this.publishCheckBox.Checked;
        }

        private void mustUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.mustUpdate = this.mustUpdateCheckBox.Checked;
        }

        private void architectureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.architectureIndex = this.architectureComboBox.SelectedIndex;
        }

        private void preReleaseVersionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.developmentBuild = (int)developmentBuildNumericUpDown.Value;
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.categoryTreeView.SelectedNode.Parent == null) // Check if the selected node is an operation
            {
                switch (this.categoryTreeView.SelectedNode.Index)
                {
                    case 0:
                        this.tablessTabControl1.SelectedTab = this.generalTabPage;
                        break;
                    case 1:
                        this.tablessTabControl1.SelectedTab = this.changelogTabPage;
                        break;
                    case 2:
                        this.tablessTabControl1.SelectedTab = this.availabilityTabPage;
                        break;
                    case 3:
                        this.tablessTabControl1.SelectedTab = this.operationsTabPage;
                        break;
                }
            }
            else
            {
                switch (this.categoryTreeView.SelectedNode.Tag.ToString())
                {
                    case "ReplaceFile":
                        this.tablessTabControl1.SelectedTab = this.replaceFilesTabPage;
                        break;
                    case "DeleteFile":
                        this.tablessTabControl1.SelectedTab = this.tablessTabControl1.TabPages[4 + this.categoryTreeView.SelectedNode.Index];
                        break;
                    case "RenameFile":
                        this.tablessTabControl1.SelectedTab = this.tablessTabControl1.TabPages[4 + this.categoryTreeView.SelectedNode.Index];
                        break;
                }
            }
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.categoryTreeView.SelectedNode != null)
            {
                if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) && this.categoryTreeView.SelectedNode.Parent != null && this.categoryTreeView.SelectedNode.Text != "Replace file/folder")
                {
                    this.tablessTabControl1.TabPages.Remove(this.tablessTabControl1.TabPages[4 + this.categoryTreeView.SelectedNode.Index]);
                    this.categoryTreeView.SelectedNode.Remove();
                }
            }
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodeToDropIn = this.categoryTreeView.GetNodeAt(this.categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 3)  // Operations-node
            {
                return; 
            }

            object data = e.Data.GetData(typeof(string));
            if (data == null) 
            {
                return; 
            }

            switch (data.ToString())
            {
                case "DeleteFile":
                    var newDeleteNode = (TreeNode)this.deleteNode.Clone();
                    this.categoryTreeView.Nodes[3].Nodes.Add(newDeleteNode);

                    var deletePage = new TabPage("Delete file");
                    deletePage.BackColor = SystemColors.Window;
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    this.tablessTabControl1.TabPages.Add(deletePage);
                    break;

                case "RenameFile":
                    var newRenameNode = (TreeNode)this.renameNode.Clone();
                    this.categoryTreeView.Nodes[3].Nodes.Add(newRenameNode);

                    var renamePage = new TabPage("Rename file");
                    renamePage.BackColor = SystemColors.Window;
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    this.tablessTabControl1.TabPages.Add(renamePage);
                    break;

                case "CreateRegistryEntry":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.createRegistryEntryNode.Clone());
                    break;

                case "DeleteRegistryEntry":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.deleteRegistryEntryNode.Clone());
                    break;

                case "SetRegistryKeyValue":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.setRegistryKeyValueNode.Clone());
                    break;
                case "StartProcess":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.startProcessNode.Clone());
                    break;
                case "TerminateProcess":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.terminateProcessNode.Clone());
                    break;
                case "StartService":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.startServiceNode.Clone());
                    break;
                case "StopService":
                    this.categoryTreeView.Nodes[3].Nodes.Add((TreeNode)this.stopServiceNode.Clone());
                    break;
            }
        }

        private void categoryTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void operationsListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.operationsListView.SelectedItems.Count > 0)
            {
                this.operationsListView.DoDragDrop(this.operationsListView.SelectedItems[0].Tag, DragDropEffects.Move);
            }
        }

        private void operationsListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}
