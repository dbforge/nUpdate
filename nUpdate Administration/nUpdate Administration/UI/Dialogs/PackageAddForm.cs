using Ionic.Zip;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.History;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddForm : BaseForm
    {
        #region "Fields"

        private const string separatorCharacter = "-";
        private const float Kb = 1024;
        private const float Mb = 1048577;

        private bool publishUpdate;
        private bool mustUpdate;
        private bool containsUnsupportedFormat;
        private bool allowCancel = true;
        private bool hasFailedOnLoading;
        private bool deletePackageFromServer = false;

        private List<string> paths = new List<string>();
        private Dictionary<string, string> archiveTypes = new Dictionary<string, string>();
        private UpdateConfiguration configuration = new UpdateConfiguration();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private FtpManager ftp = new FtpManager();
        private Log updateLog = new Log();

        private string packageVersionString;
        private string packageFolder;
        private string updateConfigFile;
        private Uri remoteInfoFileUrl;

        private int architectureIndex = 2;

        #endregion

        /// <summary>
        /// The newest version
        /// </summary>
        public Version NewestVersion { get; set; }

        /// <summary>
        /// Sets the name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Sets the developmental stage of the package.
        /// </summary>
        public DevelopmentalStage DevStage { get; set; }

        /// <summary>
        /// Returns all operations set.
        /// </summary>
        public List<String> Operations { get; set; }

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
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.noNetworkCaption = ls.PackageAddFormNoInternetWarningCaption;
            this.noNetworkText = ls.PackageAddFormNoInternetWarningText;
            this.noFilesCaption = ls.PackageAddFormNoFilesSpecifiedWarningCaption;
            this.noFilesText = ls.PackageAddFormNoFilesSpecifiedWarningText;
            this.unsupportedArchiveCaption = ls.PackageAddFormUnsupportedArchiveWarningCaption;
            this.unsupportedArchiveText = ls.PackageAddFormUnsupportedArchiveWarningText;
            this.invalidVersionCaption = ls.PackageAddFormVersionInvalidWarningCaption;
            this.invalidVersionText = ls.PackageAddFormVersionInvalidWarningText;
            this.noChangelogCaption = ls.PackageAddFormNoChangelogWarningCaption;
            this.noChangelogText = ls.PackageAddFormNoChangelogWarningText;
            this.invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
            this.invalidArgumentText = ls.InvalidArgumentErrorText;
            this.creatingPackageDataErrorCaption = ls.PackageAddFormPackageDataCreationErrorCaption;
            this.loadingProjectDataErrorCaption = ls.PackageAddFormProjectDataLoadingErrorCaption;
            this.gettingUrlErrorCaption = ls.PackageAddFormGettingUrlErrorCaption;
            this.readingPackageBytesErrorCaption = ls.PackageAddFormReadingPackageBytesErrorCaption;
            this.invalidServerDirectoryErrorCaption = ls.PackageAddFormInvalidServerDirectoryErrorCaption;
            this.invalidServerDirectoryErrorText = ls.PackageAddFormInvalidServerDirectoryErrorText;
            this.ftpDataLoadErrorCaption = ls.PackageAddFormLoadingFtpDataErrorCaption;
            this.configDownloadErrorCaption = ls.PackageAddFormConfigurationDownloadErrorCaption;
            this.serializingDataErrorCaption = ls.PackageAddFormSerializingDataErrorCaption;
            this.relativeUriErrorText = ls.PackageAddFormRelativeUriErrorText;
            this.savingInformationErrorCaption = ls.PackageAddFormPackageInformationSavingErrorCaption;
            this.uploadFailedErrorCaption = ls.PackageAddFormUploadFailedErrorCaption;

            this.initializingArchiveInfoText = ls.PackageAddFormArchiveInitializerInfoText;
            this.preparingUpdateInfoText = ls.PackageAddFormPrepareInfoText;
            this.signingPackageInfoText = ls.PackageAddFormSigningInfoText;
            this.initializingConfigInfoText = ls.PackageAddFormConfigInitializerInfoText;
            this.uploadingPackageInfoText = ls.PackageAddFormUploadingPackageInfoText;
            this.uploadingConfigInfoText = ls.PackageAddFormUploadingConfigInfoText;

            this.Text = String.Format(ls.PackageAddFormTitle, this.ProjectName, ls.ProductTitle);
            this.cancelButton.Text = ls.CancelButtonText;
            this.createButton.Text = ls.CreatePackageButtonText;

            this.devStageLabel.Text = ls.PackageAddFormDevelopmentalStageLabelText;
            this.versionLabel.Text = ls.PackageAddFormVersionLabelText;
            this.descriptionLabel.Text = ls.PackageAddFormDescriptionLabelText;
            this.publishCheckBox.Text = ls.PackageAddFormPublishCheckBoxText;
            this.publishInfoLabel.Text = ls.PackageAddFormPublishInfoLabelText;
            this.environmentLabel.Text = ls.PackageAddFormEnvironmentLabelText;
            this.environmentInfoLabel.Text = ls.PackageAddFormEnvironmentInfoLabelText;

            this.changelogLoadButton.Text = ls.PackageAddFormLoadButtonText;
            this.changelogClearButton.Text = ls.PackageAddFormClearButtonText;

            this.addFilesButton.Text = ls.PackageAddFormAddFileButtonText;
            this.removeFilesButton.Text = ls.PackageAddFormRemoveFileButtonText;
            this.filesList.Columns[0].Text = ls.PackageAddFormNameHeaderText;
            this.filesList.Columns[1].Text = ls.PackageAddFormSizeHeaderText;

            this.allVersionsRadioButton.Text = ls.PackageAddFormAvailableForAllRadioButtonText;
            this.someVersionsRadioButton.Text = ls.PackageAddFormAvailableForSomeRadioButtonText;
            this.allVersionsInfoLabel.Text = ls.PackageAddFormAvailableForAllInfoText;
            this.someVersionsInfoLabel.Text = ls.PackageAddFormAvailableForSomeInfoText;
        }

        #endregion

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// 
        /// Adds the key values for the unsupported archive types to the dictionary.
        /// </summary>
        private void InitializeArchiveTypes()
        {
            this.archiveTypes.Add("7z", ".7z");
            this.archiveTypes.Add("ACE", ".ace");
            this.archiveTypes.Add("ALZip", ".alz");
            this.archiveTypes.Add("APK", ".apk");
            this.archiveTypes.Add("ARC", ".arc");
            this.archiveTypes.Add("ARJ", ".alj");
            this.archiveTypes.Add("Scifer", ".ba");
            this.archiveTypes.Add("Cabinet", ".cab");
            this.archiveTypes.Add("Compact File Set", ".cfs");
            this.archiveTypes.Add("Disk Archiver", ".dar");
            this.archiveTypes.Add("DGCA", ".dgc");
            this.archiveTypes.Add("WinHKI", ".hki");
            this.archiveTypes.Add("ICE", ".ice");
            this.archiveTypes.Add("Jar", ".j");
            this.archiveTypes.Add("KGB Archiver", ".kgb");
            this.archiveTypes.Add("LHA", ".lha");
            this.archiveTypes.Add("LHZ", ".lzh");
            this.archiveTypes.Add("PartImage", ".partimg");
            this.archiveTypes.Add("PAQ6", ".paq6");
            this.archiveTypes.Add("PAQ7", ".paq7");
            this.archiveTypes.Add("PAQ8", ".paq8");
            this.archiveTypes.Add("PeaZip", ".pea");
            this.archiveTypes.Add("PIM", ".pim");
            this.archiveTypes.Add("Quadruple D", ".qda");
            this.archiveTypes.Add("RAR", ".rar");
            this.archiveTypes.Add("RK", ".rk");
            this.archiveTypes.Add("Scifer (sen)", ".sen");
            this.archiveTypes.Add("Stuffit", ".sit");
            this.archiveTypes.Add("Stuffit X", ".sitx");
            this.archiveTypes.Add("SQX", ".sqx");
            this.archiveTypes.Add("TAR.GZ", ".tar.gz");
            this.archiveTypes.Add("TGZ", ".tgz");
            this.archiveTypes.Add("TAR.Z", "tar.Z");
            this.archiveTypes.Add("TAR.BZ2", ".tar.bz2");
            this.archiveTypes.Add("TBZ2", ".tbz2");
            this.archiveTypes.Add("TAR.LZMA", ".tar.lzma");
            this.archiveTypes.Add("TLZ", ".tlz");
            this.archiveTypes.Add("PerfectCompress", ".uca");
            this.archiveTypes.Add("UHarc", ".uha");
            this.archiveTypes.Add("Windows Image", ".wim");
            this.archiveTypes.Add("XAR", ".xar");
            this.archiveTypes.Add("KiriKiri", ".xp3");
            this.archiveTypes.Add("YZ1", ".yz1");
            this.archiveTypes.Add("zoo", ".zoo");
            this.archiveTypes.Add("ZPAQ", ".zpaq");
            this.archiveTypes.Add("Zzip", ".zz");
        }

        public PackageAddForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the FTP-data.
        /// </summary>
        private void InitializeFtpData()
        {
            try
            {
                this.ftp.FtpServer = this.Project.FtpHost;
                int port;
                if (!int.TryParse(this.Project.FtpPort, out port))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid argument found.", "The entry for the port can't be parsed to int.", PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                    return;
                }
                this.ftp.FtpPort = port;

                if (Properties.Settings.Default.SaveCredentials)
                {
                    this.ftp.FtpUserName = this.Project.FtpUsername;

                    SecureString pwd = new SecureString();
                    foreach (char sign in this.Project.FtpPassword)
                    {
                        pwd.AppendChar(sign);
                    }
                    this.ftp.FtpPassword = pwd;
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    if (this.Project.FtpProtocol == "FTP")
                    {
                        this.ftp.Protocol = FtpProtocol.NormalFtp;
                    }
                    else if (this.Project.FtpProtocol == "FTP/SSL")
                    {
                        this.ftp.Protocol = FtpProtocol.SecureFtp;
                    }
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while reading FTP-data.", ex, PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                    return;
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                bool usePassive;
                if (!bool.TryParse(this.Project.FtpUsePassiveMode, out usePassive))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid argument found.", "The entry for passive mode can't be parsed to bool.", PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                    return;
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

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

        private void PackageAddForm_Load(object sender, EventArgs e)
        {
            this.ftp.ProgressChanged += ProgressChanged;

            this.updateLog.Project = this.Project;
            this.Operations = new List<String>();

            this.InitializeArchiveTypes();
            this.InitializeFtpData();
            if (hasFailedOnLoading)
            {
                // TODO: Failed to load FTP-data. *Take the code from ProjectForm*
                this.Close();
                return;
            }

            this.SetLanguage();

            this.architectureComboBox.SelectedIndex = 2;
            this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[0];
            this.stageComboBox.SelectedIndex = 2;
            this.unsupportedVersionsPanel.Enabled = false;
            SetWindowTheme(this.operationsListView.Handle, "explorer", null);

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
            this.minorNumericUpDown.Minimum = this.NewestVersion.Minor;
            this.buildNumericUpDown.Minimum = this.NewestVersion.Build;
            this.revisionNumericUpDown.Minimum = this.NewestVersion.Revision;
        }

        private void PackageAddForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.allowCancel)
            {
                e.Cancel = true;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (int.Equals(this.paths.Count, 0))
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.noFilesCaption, this.noFilesText, PopupButtons.OK);
                this.filesPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[2];
                return;
            }

            foreach (KeyValuePair<string, string> entry in archiveTypes)
            {
                if (paths[0].EndsWith(entry.Value))
                {
                    this.containsUnsupportedFormat = true;
                }
            }

            if (this.filesList.Items.Count == 1 && this.containsUnsupportedFormat)
            {
                if (Popup.ShowPopup(this, SystemIcons.Warning, this.unsupportedArchiveCaption, this.unsupportedArchiveText, PopupButtons.YesNo) == DialogResult.No)
                {
                    this.filesPanel.BringToFront();
                    this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[2];
                    return;
                }
                this.containsUnsupportedFormat = false;
            }

            this.packageVersionString = String.Format("{0}.{1}.{2}.{3}", this.majorNumericUpDown.Value, this.minorNumericUpDown.Value, this.buildNumericUpDown.Value, this.revisionNumericUpDown.Value);

            if (packageVersionString.StartsWith("0.0"))
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.invalidVersionCaption, this.invalidVersionText, PopupButtons.OK);
                this.generalPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[0];
                return;
            }

            if (new Version(this.packageVersionString) == this.NewestVersion)
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.invalidVersionCaption, String.Format("Version \"{0}\" is already existing.", this.packageVersionString), PopupButtons.OK);
                this.generalPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[0];
                return;
            }

            if (String.IsNullOrEmpty(this.changelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, this.noChangelogCaption, this.noChangelogText, PopupButtons.OK);
                this.changelogPanel.BringToFront();
                this.categoryTreeView.SelectedNode = this.categoryTreeView.Nodes[1];
                return;
            }

            // Disallow closing now
            this.allowCancel = false;

            foreach (Control control in this.Controls)
            {
                if (control.Visible == true)
                {
                    control.Enabled = false;
                }
            }

            this.loadingPanel.Location = new Point(180, 91);
            this.loadingPanel.BringToFront();
            this.loadingPanel.Visible = true;

            var task =
            Task.Factory.StartNew(this.InitializePackage).ContinueWith(this.InitializingFailed,
                    this.cancellationToken.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => this.InitializingFinished(),
                            this.cancellationToken.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Initializes the update package and uploads it if set.
        /// </summary>
        private void InitializePackage()
        {
            if (!this.Project.UpdateUrl.EndsWith("/"))
            {
                this.Project.UpdateUrl += "/";
            }
            this.remoteInfoFileUrl = UriConnecter.ConnectUri(this.Project.UpdateUrl, "updates.json");
            this.packageFolder = Path.Combine(Program.Path, this.packageVersionString);
            this.updateConfigFile = Path.Combine(Program.Path, this.packageVersionString, "updates.json");

            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
                this.cancellationToken = new CancellationTokenSource();
            }

            Invoke(new Action(() => this.loadingLabel.Text = this.initializingArchiveInfoText));

            // Save the package first
            // ----------------------

            Directory.CreateDirectory(this.packageFolder); // Create the content folder
            using (FileStream fs = File.Create(this.updateConfigFile)) { };

            if (int.Equals(this.paths.Count, 1))
            {
                File.Copy(this.paths[0], Path.Combine(this.packageFolder, String.Format("{0}.zip", this.Project.Id)), true); // Create the archive
            }
            else
            {
                ZipFile zip = new ZipFile();
                zip.AddFiles(this.paths);
                zip.Save(Path.Combine(this.packageFolder, String.Format("{0}.zip", this.Project.Id)));
            }

            this.updateLog.Write(LogEntry.Create, this.packageVersionString);
            Invoke(new Action(() => this.loadingLabel.Text = this.preparingUpdateInfoText));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersions = null;

            if (this.versionsList.Items.Count == 0)
            {
                this.allVersionsRadioButton.Checked = true;
            }
            else if (this.versionsList.Items.Count > 0 && this.someVersionsRadioButton.Checked)
            {
                List<string> itemArray = new List<string>();
                foreach (string item in this.versionsList.Items)
                {
                    itemArray.Add(item);
                }

                unsupportedVersions = itemArray.ToArray();
            }

            // Create a new package configuration
            this.configuration.Changelog = this.changelogTextBox.Text;
            this.configuration.DevelopmentalStage = this.DevStage.ToString();
            this.configuration.MustUpdate = this.mustUpdateCheckBox.Checked;

            switch (architectureIndex)
            {
                case 0:
                    this.configuration.Architecture = "x64";
                    break;
                case 1:
                    this.configuration.Architecture = "x86";
                    break;
                case 2:
                    this.configuration.Architecture = "AnyCPU";
                    break;
            }

            Invoke(new Action(() => this.loadingLabel.Text = this.signingPackageInfoText));

            byte[] data = File.ReadAllBytes(Path.Combine(this.packageFolder, String.Format("{0}.zip", this.Project.Id)));
            this.configuration.Signature = Convert.ToBase64String(new RsaSignature(this.Project.PrivateKey).SignData(data));

            string remotePackageDirectory = String.Format("{0}/", UriConnecter.ConnectUri(this.Project.UpdateUrl, this.packageVersionString).ToString());

            this.configuration.UnsupportedVersions = unsupportedVersions;
            this.configuration.UpdatePackageUrl = UriConnecter.ConnectUri(remotePackageDirectory, String.Format("{0}.zip", this.Project.Id)).ToString(); // Get the URL with the GUID of the project
            this.configuration.UpdateVersion = this.packageVersionString;

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => this.loadingLabel.Text = this.initializingConfigInfoText));

            var configurationList = new List<UpdateConfiguration>();
            if (this.Project.ProxyHost != null)
            {
                var pwd = new SecureString();
                foreach (Char sign in Project.ProxyPassword)
                {
                    pwd.AppendChar(sign);
                }

                var proxySettings = new ProxySettings()
                {
                    Host = this.Project.ProxyHost,
                    Port = int.Parse(this.Project.ProxyPort),
                    Username = this.Project.ProxyUsername,
                    Password = pwd,
                };

                configurationList = this.configuration.LoadUpdateConfiguration(this.remoteInfoFileUrl, proxySettings);
            }
            else
            {
                configurationList = this.configuration.LoadUpdateConfiguration(this.remoteInfoFileUrl);
            }

            if (configurationList != null)
            {
                configurationList.Add(this.configuration);
                File.WriteAllText(this.updateConfigFile, Serializer.Serialize(configurationList));
            }
            else
            {
                File.WriteAllText(this.updateConfigFile, Serializer.Serialize(this.configuration));
            }

            /* ------------- Save package info  ------------- */
            var package = new UpdatePackage();
            package.Description = this.descriptionTextBox.Text;
            package.IsReleased = this.publishUpdate.ToString();
            package.LocalPackagePath = Path.Combine(Program.Path, this.packageVersionString, String.Format("{0}.zip", Project.Id));
            package.Version = this.packageVersionString;

            if (this.Project.Packages == null)
            {
                this.Project.Packages = new List<UpdatePackage>();
            }

            this.Project.Packages.Add(package);

            /* ------------ Upload ----------- */
            if (this.publishUpdate)
            {
                if (!Properties.Settings.Default.SaveCredentials)
                {
                    Invoke(new Action(() =>
                        {
                            var credentialsForm = new CredentialsForm();
                            if (credentialsForm.ShowDialog() == DialogResult.OK)
                            {
                                this.ftp.FtpUserName = credentialsForm.Username;
                                this.ftp.FtpPassword = credentialsForm.Password;
                            }
                        }));
                }

                Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, "0%")));
                this.ftp.UploadPackage(Path.Combine(Program.Path, this.packageVersionString, String.Format("{0}.zip", this.Project.Id)), this.packageVersionString);
                while (!this.ftp.HasFinishedUploading)
                {
                    continue;
                }

                Invoke(new Action(() => this.loadingLabel.Text = this.uploadingConfigInfoText));
                this.ftp.UploadFile(this.updateConfigFile);
                while (!this.ftp.HasFinishedUploading)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// The handler when the task has failed.
        /// </summary>
        /// <param name="task"></param>
        private void InitializingFailed(Task task)
        {
            var ex = task.Exception;
            if (ex.InnerException != null)
            {
                string infoMessage = String.Empty;

                if (ex.InnerException.TargetSite != null)
                {
                    switch (ex.InnerException.TargetSite.Name)
                    {
                        case "UploadPackageAsync":
                            infoMessage = "Error while uploading the package.";
                            break;
                        case "DeleteFileAsync":
                            infoMessage = "Error while deleting the package.";
                            break;
                        case "LoadUpdateConfiguration":
                            infoMessage = "Error while downloading the config.";
                            break;
                        default:
                            infoMessage = "Error while initializing the package.";
                            break;
                    }
                }

                infoMessage = "Error while initializing the package.";
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, infoMessage, ex.InnerException, PopupButtons.OK)));

                if (Directory.Exists(this.packageFolder))
                {
                    Directory.Delete(this.packageFolder, true);
                }

                if (this.deletePackageFromServer)
                {
                    this.ftp.DeleteDirectory(this.packageVersionString);
                }

                this.updateLog.Write(LogEntry.Delete, this.packageVersionString);

                Invoke(new Action(() =>
                {
                    foreach (Control control in this.Controls)
                    {
                        control.Enabled = true;
                    }

                    this.loadingPanel.Visible = false;
                    this.allowCancel = true;
                }));
            }
        }

        /// <summary>
        /// The handler when the task has finished.
        /// </summary>
        private void InitializingFinished()
        {
            this.Project.HasUnsavedChanges = true;

            Invoke(new Action(() =>
            {
                foreach (Control control in this.Controls)
                {
                    control.Enabled = true;
                }

                this.loadingPanel.Visible = false;
                this.allowCancel = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }));
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage))));
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

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofdFiles = new OpenFileDialog())
            {
                ofdFiles.SupportMultiDottedExtensions = true;
                ofdFiles.Multiselect = true;
                ofdFiles.Filter = "All Files (*.*)| *.*";

                if (ofdFiles.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in ofdFiles.FileNames)
                    {
                        this.paths.Add(file);

                        FileInfo fileInfo = new FileInfo(file);
                        string name = fileInfo.Name;
                        long sizeInBytes = fileInfo.Length;

                        float size;
                        string sizeText;

                        if (sizeInBytes > 104857.6)
                        {
                            size = (float)Math.Round(sizeInBytes / Mb, 1);
                            sizeText = String.Format("{0} MB", size);
                        }
                        else
                        {
                            size = (float)Math.Round(sizeInBytes / Kb, 1);
                            sizeText = String.Format("{0} KB", size);
                        }

                        ListViewItem fileItem = new ListViewItem(name);

                        ListViewItem item = new ListViewItem();
                        item = this.filesList.FindItemWithText(name);

                        if (item != null)
                        {
                            DialogResult dr = MessageBox.Show("The file \"" + name + "\" is already imported. Should it be replaced by the new one?", "File already imported", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr == DialogResult.Yes)
                            {
                                this.filesList.Items.Remove(item);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        fileItem.ImageIndex = file.EndsWith(".zip") ? 0 : 1;
                        fileItem.SubItems.Add(sizeText);
                        this.filesList.Items.Add(fileItem);
                    }
                }
            }
        }

        private void removeFilesButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itemToDelete in this.filesList.SelectedItems)
            {
                this.paths.RemoveAt(itemToDelete.Index);
                this.filesList.Items.Remove(itemToDelete);
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
            this.versionsList.Items.Add(version);
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            this.versionsList.Items.Remove(this.versionsList.SelectedItem);
        }

        private void stageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DevStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), this.stageComboBox.GetItemText(this.stageComboBox.SelectedItem));
        }

        private void publishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.publishUpdate = this.publishCheckBox.Checked;
        }

        private void mustUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.mustUpdate = this.mustUpdateCheckBox.Checked;
        }

        private void environmentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.architectureIndex = this.architectureComboBox.SelectedIndex;
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (this.categoryTreeView.SelectedNode.Index)
            {
                case 0:
                    this.generalPanel.BringToFront();
                    break;
                case 1:
                    this.changelogPanel.BringToFront();
                    break;
                case 2:
                    this.filesPanel.BringToFront();
                    break;
                case 3:
                    this.availabilityPanel.BringToFront();
                    break;
                case 4:
                    this.operationsPanel.BringToFront();
                    break;
            }
        }
    }
}
