using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using nUpdate.Administration.Core.Application.History;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddForm : BaseForm
    {
        #region "Fields"

        const string separatorCharacter = "-";
        const float KB = 1024;
        const float MB = 1048577;

        bool publishUpdate;
        bool mustUpdate;
        bool containsUnsupportedFormat;
        bool allowCancel = true;
        bool hasFailedOnLoading;
        bool deletePackageFromServer = false;

        private List<string> paths = new List<string>();
        private Dictionary<string, string> archiveTypes = new Dictionary<string, string>();
        private UpdateConfiguration configuration = new UpdateConfiguration();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private FtpManager ftp = new FtpManager();
        private Log updateLog = new Log();

        string packageVersionString;
        string packageFolder;
        string updateConfigFile;
        Uri remoteInfoFileUrl;

        int architectureIndex = 2;

        #endregion

        /// <summary>
        /// The newest version
        /// </summary>
        public Version NewestVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the name of the project.
        /// </summary>
        public string ProjectName
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the developmental stage of the package.
        /// </summary>
        public DevelopmentalStage DevStage
        {
            get;
            set;
        }

        /// <summary>
        /// Returns all operations set.
        /// </summary>
        public List<String> Operations { get; set; }

        #region "Localization"

        string noNetworkCaption;
        string noNetworkText;
        string noFilesCaption;
        string noFilesText;
        string unsupportedArchiveCaption;
        string unsupportedArchiveText;
        string invalidVersionCaption;
        string invalidVersionText;
        string noChangelogCaption;
        string noChangelogText;
        string invalidArgumentCaption;
        string invalidArgumentText;
        string creatingPackageDataErrorCaption;
        string loadingProjectDataErrorCaption;
        string gettingUrlErrorCaption;
        string readingPackageBytesErrorCaption;
        string invalidServerDirectoryErrorCaption;
        string invalidServerDirectoryErrorText;
        string ftpDataLoadErrorCaption;
        string configDownloadErrorCaption;
        string serializingDataErrorCaption;
        string relativeUriErrorText;
        string savingInformationErrorCaption;
        string uploadFailedErrorCaption;
        string initializingArchiveInfoText;
        string preparingUpdateInfoText;
        string signingPackageInfoText;
        string initializingConfigInfoText;
        string uploadingPackageInfoText;
        string uploadingConfigInfoText;

        private void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            else
            {

                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            noNetworkCaption = ls.PackageAddFormNoInternetWarningCaption;
            noNetworkText = ls.PackageAddFormNoInternetWarningText;
            noFilesCaption = ls.PackageAddFormNoFilesSpecifiedWarningCaption;
            noFilesText = ls.PackageAddFormNoFilesSpecifiedWarningText;
            unsupportedArchiveCaption = ls.PackageAddFormUnsupportedArchiveWarningCaption;
            unsupportedArchiveText = ls.PackageAddFormUnsupportedArchiveWarningText;
            invalidVersionCaption = ls.PackageAddFormVersionInvalidWarningCaption;
            invalidVersionText = ls.PackageAddFormVersionInvalidWarningText;
            noChangelogCaption = ls.PackageAddFormNoChangelogWarningCaption;
            noChangelogText = ls.PackageAddFormNoChangelogWarningText;
            invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
            invalidArgumentText = ls.InvalidArgumentErrorText;
            creatingPackageDataErrorCaption = ls.PackageAddFormPackageDataCreationErrorCaption;
            loadingProjectDataErrorCaption = ls.PackageAddFormProjectDataLoadingErrorCaption;
            gettingUrlErrorCaption = ls.PackageAddFormGettingUrlErrorCaption;
            readingPackageBytesErrorCaption = ls.PackageAddFormReadingPackageBytesErrorCaption;
            invalidServerDirectoryErrorCaption = ls.PackageAddFormInvalidServerDirectoryErrorCaption;
            invalidServerDirectoryErrorText = ls.PackageAddFormInvalidServerDirectoryErrorText;
            ftpDataLoadErrorCaption = ls.PackageAddFormLoadingFtpDataErrorCaption;
            configDownloadErrorCaption = ls.PackageAddFormConfigurationDownloadErrorCaption;
            serializingDataErrorCaption = ls.PackageAddFormSerializingDataErrorCaption;
            relativeUriErrorText = ls.PackageAddFormRelativeUriErrorText;
            savingInformationErrorCaption = ls.PackageAddFormPackageInformationSavingErrorCaption;
            uploadFailedErrorCaption = ls.PackageAddFormUploadFailedErrorCaption;

            initializingArchiveInfoText = ls.PackageAddFormArchiveInitializerInfoText;
            preparingUpdateInfoText = ls.PackageAddFormPrepareInfoText;
            signingPackageInfoText = ls.PackageAddFormSigningInfoText;
            initializingConfigInfoText = ls.PackageAddFormConfigInitializerInfoText;
            uploadingPackageInfoText = ls.PackageAddFormUploadingPackageInfoText;
            uploadingConfigInfoText = ls.PackageAddFormUploadingConfigInfoText;

            Text = String.Format(ls.PackageAddFormTitle, ProjectName, ls.ProductTitle);
            cancelButton.Text = ls.CancelButtonText;
            createButton.Text = ls.CreatePackageButtonText;

            devStageLabel.Text = ls.PackageAddFormDevelopmentalStageLabelText;
            versionLabel.Text = ls.PackageAddFormVersionLabelText;
            descriptionLabel.Text = ls.PackageAddFormDescriptionLabelText;
            publishCheckBox.Text = ls.PackageAddFormPublishCheckBoxText;
            publishInfoLabel.Text = ls.PackageAddFormPublishInfoLabelText;
            environmentLabel.Text = ls.PackageAddFormEnvironmentLabelText;
            environmentInfoLabel.Text = ls.PackageAddFormEnvironmentInfoLabelText;

            changelogLoadButton.Text = ls.PackageAddFormLoadButtonText;
            changelogClearButton.Text = ls.PackageAddFormClearButtonText;

            addFilesButton.Text = ls.PackageAddFormAddFileButtonText;
            removeFilesButton.Text = ls.PackageAddFormRemoveFileButtonText;
            filesList.Columns[0].Text = ls.PackageAddFormNameHeaderText;
            filesList.Columns[1].Text = ls.PackageAddFormSizeHeaderText;

            allVersionsRadioButton.Text = ls.PackageAddFormAvailableForAllRadioButtonText;
            someVersionsRadioButton.Text = ls.PackageAddFormAvailableForSomeRadioButtonText;
            allVersionsInfoLabel.Text = ls.PackageAddFormAvailableForAllInfoText;
            someVersionsInfoLabel.Text = ls.PackageAddFormAvailableForSomeInfoText;
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
            archiveTypes.Add("7z", ".7z");
            archiveTypes.Add("ACE", ".ace");
            archiveTypes.Add("ALZip", ".alz");
            archiveTypes.Add("APK", ".apk");
            archiveTypes.Add("ARC", ".arc");
            archiveTypes.Add("ARJ", ".alj");
            archiveTypes.Add("Scifer", ".ba");
            archiveTypes.Add("Cabinet", ".cab");
            archiveTypes.Add("Compact File Set", ".cfs");
            archiveTypes.Add("Disk Archiver", ".dar");
            archiveTypes.Add("DGCA", ".dgc");
            archiveTypes.Add("WinHKI", ".hki");
            archiveTypes.Add("ICE", ".ice");
            archiveTypes.Add("Jar", ".j");
            archiveTypes.Add("KGB Archiver", ".kgb");
            archiveTypes.Add("LHA", ".lha");
            archiveTypes.Add("LHZ", ".lzh");
            archiveTypes.Add("PartImage", ".partimg");
            archiveTypes.Add("PAQ6", ".paq6");
            archiveTypes.Add("PAQ7", ".paq7");
            archiveTypes.Add("PAQ8", ".paq8");
            archiveTypes.Add("PeaZip", ".pea");
            archiveTypes.Add("PIM", ".pim");
            archiveTypes.Add("Quadruple D", ".qda");
            archiveTypes.Add("RAR", ".rar");
            archiveTypes.Add("RK", ".rk");
            archiveTypes.Add("Scifer (sen)", ".sen");
            archiveTypes.Add("Stuffit", ".sit");
            archiveTypes.Add("Stuffit X", ".sitx");
            archiveTypes.Add("SQX", ".sqx");
            archiveTypes.Add("TAR.GZ", ".tar.gz");
            archiveTypes.Add("TGZ", ".tgz");
            archiveTypes.Add("TAR.Z", "tar.Z");
            archiveTypes.Add("TAR.BZ2", ".tar.bz2");
            archiveTypes.Add("TBZ2", ".tbz2");
            archiveTypes.Add("TAR.LZMA", ".tar.lzma");
            archiveTypes.Add("TLZ", ".tlz");
            archiveTypes.Add("PerfectCompress", ".uca");
            archiveTypes.Add("UHarc", ".uha");
            archiveTypes.Add("Windows Image", ".wim");
            archiveTypes.Add("XAR", ".xar");
            archiveTypes.Add("KiriKiri", ".xp3");
            archiveTypes.Add("YZ1", ".yz1");
            archiveTypes.Add("zoo", ".zoo");
            archiveTypes.Add("ZPAQ", ".zpaq");
            archiveTypes.Add("Zzip", ".zz");
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
                ftp.FtpServer = Project.FtpHost;
                int port;
                if (!int.TryParse(Project.FtpPort, out port))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid argument found.", "The entry for the port can't be parsed to int.", PopupButtons.OK);
                    hasFailedOnLoading = true;
                    return;
                }
                ftp.FtpPort = port;

                if (Properties.Settings.Default.SaveCredentials)
                {
                    ftp.FtpUserName = Project.FtpUsername;

                    SecureString pwd = new SecureString();
                    foreach (char sign in Project.FtpPassword)
                    {
                        pwd.AppendChar(sign);
                    }
                    ftp.FtpPassword = pwd;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    if (Project.FtpProtocol == "FTP")
                        ftp.Protocol = FtpProtocol.NormalFtp;
                    else if (Project.FtpProtocol == "FTP/SSL")
                        ftp.Protocol = FtpProtocol.SecureFtp;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while reading FTP-data.", ex, PopupButtons.OK);
                    hasFailedOnLoading = true;
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                bool usePassive;
                if (!bool.TryParse(Project.FtpUsePassiveMode, out usePassive))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid argument found.", "The entry for passive mode can't be parsed to bool.", PopupButtons.OK);
                    hasFailedOnLoading = true;
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                ftp.Directory = Project.FtpDirectory;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
        }

        private void PackageAddForm_Load(object sender, EventArgs e)
        {
            ftp.ProgressChanged += ProgressChanged;

            updateLog.Project = Project;
            Operations = new List<String>();

            InitializeArchiveTypes();
            InitializeFtpData();
            if (hasFailedOnLoading)
            {
                // TODO: Failed to load FTP-data. *Take the code from ProjectForm*
                Close();
                return;
            }

            SetLanguage();

            architectureComboBox.SelectedIndex = 2;
            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            stageComboBox.SelectedIndex = 2;
            unsupportedVersionsPanel.Enabled = false;
            SetWindowTheme(operationsListView.Handle, "explorer", null);

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
            minorNumericUpDown.Minimum = NewestVersion.Minor;
            buildNumericUpDown.Minimum = NewestVersion.Build;
            revisionNumericUpDown.Minimum = NewestVersion.Revision;
        }

        private void PackageAddForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (int.Equals(paths.Count, 0))
            {
                Popup.ShowPopup(this, SystemIcons.Error, noFilesCaption, noFilesText, PopupButtons.OK);
                filesPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[2];
                return;
            }

            foreach (KeyValuePair<string, string> entry in archiveTypes)
            {
                if (paths[0].EndsWith(entry.Value))
                    containsUnsupportedFormat = true;
            }

            if (filesList.Items.Count == 1 && containsUnsupportedFormat)
            {
                if (Popup.ShowPopup(this, SystemIcons.Warning, unsupportedArchiveCaption, unsupportedArchiveText, PopupButtons.YesNo) == DialogResult.No)
                {

                    filesPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[2];
                    return;
                }
                containsUnsupportedFormat = false;
            }

            packageVersionString = String.Format("{0}.{1}.{2}.{3}",
                majorNumericUpDown.Value, minorNumericUpDown.Value, buildNumericUpDown.Value, revisionNumericUpDown.Value);

            if (packageVersionString.StartsWith("0.0"))
            {
                Popup.ShowPopup(this, SystemIcons.Error, invalidVersionCaption, invalidVersionText, PopupButtons.OK);
                generalPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                return;
            }

            if (new Version(packageVersionString) == NewestVersion)
            {
                Popup.ShowPopup(this, SystemIcons.Error, invalidVersionCaption, String.Format("Version \"{0}\" is already existing.", packageVersionString), PopupButtons.OK);
                generalPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                return;
            }

            if (String.IsNullOrEmpty(changelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, noChangelogCaption, noChangelogText, PopupButtons.OK);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            // Disallow closing now
            allowCancel = false;

            foreach (Control control in this.Controls)
            {
                if (control.Visible == true)
                    control.Enabled = false;
            }

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;

            var task =
            Task.Factory.StartNew(this.InitializePackage).ContinueWith(this.InitializingFailed,
                    cancellationToken.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith( o => this.InitializingFinished(),
                            cancellationToken.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Initializes the update package and uploads it if set.
        /// </summary>
        private void InitializePackage()
        {
            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            remoteInfoFileUrl = UriConnecter.ConnectUri(Project.UpdateUrl, "updates.json");
            packageFolder = Path.Combine(Program.Path, packageVersionString);
            updateConfigFile = Path.Combine(Program.Path, packageVersionString, "updates.json");

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = new CancellationTokenSource();
            }

            Invoke(new Action(() => loadingLabel.Text = initializingArchiveInfoText));

            // Save the package first
            // ----------------------

            Directory.CreateDirectory(packageFolder); // Create the content folder
            using (FileStream fs = File.Create(updateConfigFile)) { };

            if (int.Equals(paths.Count, 1))
                File.Copy(paths[0], Path.Combine(packageFolder, String.Format("{0}.zip", Project.Id)), true); // Create the archive
            else
            {
                ZipFile zip = new ZipFile();
                zip.AddFiles(paths);
                zip.Save(Path.Combine(packageFolder, String.Format("{0}.zip", Project.Id)));
            }

            updateLog.Write(LogEntry.Create, packageVersionString);
            Invoke(new Action(() => loadingLabel.Text = preparingUpdateInfoText));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersions = null;

            if (versionsList.Items.Count == 0)
                allVersionsRadioButton.Checked = true;

            else if (versionsList.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                List<string> itemArray = new List<string>();
                foreach (string item in versionsList.Items)
                {
                    itemArray.Add(item);
                }

                unsupportedVersions = itemArray.ToArray();
            }

            // Create a new package configuration
            configuration.Changelog = changelogTextBox.Text;
            configuration.DevelopmentalStage = DevStage.ToString();
            configuration.MustUpdate = mustUpdateCheckBox.Checked;

            switch (architectureIndex)
            {
                case 0:
                    configuration.Architecture = "x64";
                    break;
                case 1:
                    configuration.Architecture = "x86";
                    break;
                case 2:
                    configuration.Architecture = "AnyCPU";
                    break;
            }

            Invoke(new Action(() => loadingLabel.Text = signingPackageInfoText));

            byte[] data = File.ReadAllBytes(Path.Combine(packageFolder, String.Format("{0}.zip", Project.Id)));
            configuration.Signature = Convert.ToBase64String(new RsaSignature(Project.PrivateKey).SignData(data));

            string remotePackageDirectory = String.Format("{0}/", 
                UriConnecter.ConnectUri(Project.UpdateUrl, packageVersionString).ToString());

            configuration.UnsupportedVersions = unsupportedVersions;
            configuration.UpdatePackageUrl = UriConnecter.ConnectUri(remotePackageDirectory, String.Format("{0}.zip", Project.Id)).ToString(); // Get the URL with the GUID of the project
            configuration.UpdateVersion = packageVersionString;

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => loadingLabel.Text = initializingConfigInfoText));

            var configurationList = new List<UpdateConfiguration>();
            if (Project.ProxyHost != null)
            {
                var pwd = new SecureString();
                foreach (Char sign in Project.ProxyPassword)
                {
                    pwd.AppendChar(sign);
                }

                var proxySettings = new ProxySettings()
                {
                    Host = Project.ProxyHost,
                    Port = int.Parse(Project.ProxyPort),
                    Username = Project.ProxyUsername,
                    Password = pwd,
                };

                configurationList = configuration.LoadUpdateConfiguration(remoteInfoFileUrl, proxySettings);
            }
            else {
                configurationList = configuration.LoadUpdateConfiguration(remoteInfoFileUrl);
            }

            if (configurationList != null)
            {
                configurationList.Add(configuration);
                File.WriteAllText(updateConfigFile, Serializer.Serialize(configurationList));
            }
            else
            {
                File.WriteAllText(updateConfigFile, Serializer.Serialize(configuration));
            }

            /* ------------- Save package info  ------------- */
            var package = new UpdatePackage();
            package.Description = descriptionTextBox.Text;
            package.IsReleased = publishUpdate.ToString();
            package.LocalPackagePath = Path.Combine(Program.Path, packageVersionString, 
                String.Format("{0}.zip", Project.Id)); 
            package.Version = packageVersionString;

            if (Project.Packages == null)
                Project.Packages = new List<UpdatePackage>();
            Project.Packages.Add(package);

            /* ------------ Upload ----------- */
            if (publishUpdate)
            {
                if (!Properties.Settings.Default.SaveCredentials)
                {
                    Invoke(new Action(() =>
                        {
                            var credentialsForm = new CredentialsForm();
                            if (credentialsForm.ShowDialog() == DialogResult.OK)
                            {
                                ftp.FtpUserName = credentialsForm.Username;
                                ftp.FtpPassword = credentialsForm.Password;
                            }
                        }));
                }

                Invoke(new Action(() => loadingLabel.Text = String.Format(uploadingPackageInfoText, "0%")));
                ftp.UploadPackage(Path.Combine(Program.Path, packageVersionString, String.Format("{0}.zip", Project.Id)),
                    packageVersionString);
                while (!ftp.HasFinishedUploading)
                    continue;

                Invoke(new Action(() => loadingLabel.Text = uploadingConfigInfoText));
                ftp.UploadFile(updateConfigFile);
                while (!ftp.HasFinishedUploading)
                    continue;
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

                if (Directory.Exists(packageFolder))
                    Directory.Delete(packageFolder, true);

                if (deletePackageFromServer)
                    ftp.DeleteDirectory(packageVersionString);

                updateLog.Write(LogEntry.Delete, packageVersionString);

                Invoke(new Action(() =>
                {
                    foreach (Control control in this.Controls)
                        control.Enabled = true;
                    loadingPanel.Visible = false;

                    allowCancel = true;
                }));
            }
        }

        /// <summary>
        /// The handler when the task has finished.
        /// </summary>
        private void InitializingFinished()
        {
            Project.HasUnsavedChanges = true;

            Invoke(new Action(() =>
            {
                foreach (Control control in this.Controls)
                    control.Enabled = true;
                loadingPanel.Visible = false;

                allowCancel = true;
                DialogResult = DialogResult.OK;
                Close();
            }));
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() => loadingLabel.Text = String.Format(uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage))));
        }

        private void changelogLoadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
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
                        paths.Add(file);

                        FileInfo fileInfo = new FileInfo(file);
                        string name = fileInfo.Name;
                        long sizeInBytes = fileInfo.Length;

                        float size;
                        string sizeText;

                        if (sizeInBytes > 104857.6)
                        {
                            size = (float)Math.Round(sizeInBytes / MB, 1);
                            sizeText = String.Format("{0} MB", size);
                        }

                        else
                        {
                            size = (float)Math.Round(sizeInBytes / KB, 1);
                            sizeText = String.Format("{0} KB", size);
                        }

                        ListViewItem fileItem = new ListViewItem(name);

                        ListViewItem item = new ListViewItem();
                        item = filesList.FindItemWithText(name);

                        if (item != null)
                        {
                            DialogResult dr = MessageBox.Show("The file \"" + name + "\" is already imported. Should it be replaced by the new one?", "File already imported", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr == DialogResult.Yes)
                                filesList.Items.Remove(item);
                            else
                                continue;
                        }

                        fileItem.ImageIndex = file.EndsWith(".zip") ? 0 : 1;

                        fileItem.SubItems.Add(sizeText);
                        filesList.Items.Add(fileItem);
                    }
                }
            }
        }

        private void removeFilesButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itemToDelete in filesList.SelectedItems)
            {
                paths.RemoveAt(itemToDelete.Index);
                filesList.Items.Remove(itemToDelete);
            }
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
            string version = unsupportedMajorNumericUpDown.Value.ToString() + "." + unsupportedMinorNumericUpDown.Value.ToString() + "." + unsupportedBuildNumericUpDown.Value.ToString() + "." + unsupportedRevisionNumericUpDown.Value.ToString();
            versionsList.Items.Add(version);
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            versionsList.Items.Remove(versionsList.SelectedItem);
        }

        private void stageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), stageComboBox.GetItemText(stageComboBox.SelectedItem));
        }

        private void publishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            publishUpdate = publishCheckBox.Checked;
        }

        private void mustUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mustUpdate = mustUpdateCheckBox.Checked;
        }

        private void environmentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            architectureIndex = architectureComboBox.SelectedIndex;
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (categoryTreeView.SelectedNode.Index)
            {
                case 0:
                    generalPanel.BringToFront();
                    break;
                case 1:
                    changelogPanel.BringToFront();
                    break;
                case 2:
                    filesPanel.BringToFront();
                    break;
                case 3:
                    availabilityPanel.BringToFront();
                    break;
                case 4:
                    operationsPanel.BringToFront();
                    break;
            }
        }
    }
}
