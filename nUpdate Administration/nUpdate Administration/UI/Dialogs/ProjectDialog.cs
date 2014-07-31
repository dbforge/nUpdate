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
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.History;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectDialog : BaseDialog
    {
        private const float Kb = 1024;
        private const float Mb = 1048577;

        private const int COR_E_ENDOFSTREAM = unchecked((int) 0x80070026);
        private const int COR_E_FILELOAD = unchecked((int) 0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int) 0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int) 0x80070003);
        private const int COR_E_PATHTOOLONG = unchecked((int) 0x800700CE);
        private const int COR_E_IO = unchecked((int) 0x80131620);
        private readonly FTPManager ftp = new FTPManager();
        private readonly Log updateLog = new Log();
        private bool allowCancel = true;
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private HttpWebResponse configResponse;
        private int groupIndex;
        private bool isSetByUser;

        private MySqlConnection myConnection;
        private string packageDescription;
        private UpdateVersion packageVersion;
        private Uri updateInfoFileUrl;

        public ProjectDialog()
        {
            InitializeComponent();
        }

        #region "Localization"

        private string ftpDataLoadErrorCaption = "Failed to load FTP-data.";
        private string gettingUrlErrorCaption = "Error while getting url.";
        private string invalidArgumentCaption = "Invalid argument found.";
        private string invalidArgumentText = "The entry {0} can't be parsed to {1}.";
        private string invalidServerDirectoryErrorCaption = "Invalid server directory.";

        private string invalidServerDirectoryErrorText =
            "The directory for the update files on the server is not valid. Please edit it.";

        private string readingPackageBytesErrorCaption = "Reading package bytes failed.";

        private string relativeUriErrorText = "The server-directory can't be set as a relative uri.";
        private string savingInformationErrorCaption = "Saving package information failed.";
        private string uploadFailedErrorCaption = "Upload failed.";

        private string uploadingConfigInfoText = "Uploading configuration...";
        private string uploadingPackageInfoText = "Uploading package - {0}";

        /// <summary>
        ///     Sets the language.
        /// </summary>
        public void SetLanguage()
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

            Text = String.Format("{0} - {1}", Project.Name, ls.ProductTitle);
            overviewHeader.Text = ls.ProjectDialogOverviewText;
            overviewTabPage.Text = ls.ProjectDialogOverviewTabText;
            packagesTabPage.Text = ls.ProjectDialogPackagesTabText;
            nameLabel.Text = ls.ProjectDialogNameLabelText;
            updateUrlLabel.Text = ls.ProjectDialogUpdateUrlLabelText;
            ftpHostLabel.Text = ls.ProjectDialogFtpHostLabelText;
            ftpDirectoryLabel.Text = ls.ProjectDialogFtpDirectoryLabelText;
            releasedPackagesAmountLabel.Text = ls.ProjectDialogPackagesAmountLabelText;
            newestPackageReleasedLabel.Text = ls.ProjectDialogNewestPackageLabelText;
            infoFileloadingLabel.Text = ls.ProjectDialogInfoFileloadingLabelText;
            checkUpdateConfigurationLinkLabel.Text = ls.ProjectDialogCheckInfoFileStatusLinkLabelText;
            projectDataHeader.Text = ls.ProjectDialogProjectDataText;
            publicKeyLabel.Text = ls.ProjectDialogPublicKeyLabelText;
            projectIdLabel.Text = ls.ProjectDialogProjectIdLabelText;
            stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

            addButton.Text = ls.ProjectDialogAddButtonText;
            editButton.Text = ls.ProjectDialogEditButtonText;
            deleteButton.Text = ls.ProjectDialogDeleteButtonText;
            uploadButton.Text = ls.ProjectDialogUploadButtonText;
            historyButton.Text = ls.ProjectDialogHistoryButtonText;

            packagesList.Columns[0].Text = ls.ProjectDialogVersionText;
            packagesList.Columns[1].Text = ls.ProjectDialogReleasedText;
            packagesList.Columns[2].Text = ls.ProjectDialogSizeText;
            packagesList.Columns[3].Text = ls.ProjectDialogDescriptionText;

            searchTextBox.Cue = ls.ProjectDialogSearchText;
        }

        #endregion

        private void ProjectDialog_Load(object sender, EventArgs e)
        {
            // Set the handler for the progress
            ftp.ProgressChanged += ProgressChanged;

            InitializeProjectDetails(); // Initialize everything, here project details
            if (hasFailedOnLoading)
            {
                Close();
                return;
            }

            InitializeFtpData(); // ... FTP-data
            if (hasFailedOnLoading)
            {
                Close();
                return;
            }

            InitializePackageItems(); // ... package items

            // The log preferences
            updateLog.Project = Project;

            // The programming language
            switch (Project.ProgrammingLanguage)
            {
                case "VB.NET":
                    programmingLanguageComboBox.SelectedIndex = 0;
                    break;
                case "C#":
                    programmingLanguageComboBox.SelectedIndex = 1;
                    break;
            }

            // Set the language for this dialog
            SetLanguage();

            // Update-URL
            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            updateInfoFileUrl = UriConnecter.ConnectUri(Project.UpdateUrl, "updates.json");

            // Control position- and draw settings
            checkingUrlPictureBox.Location =
                new Point(
                    checkUpdateConfigurationLinkLabel.Location.X + checkUpdateConfigurationLinkLabel.Size.Width + 2,
                    checkingUrlPictureBox.Location.Y);
            tickPictureBox.Location =
                new Point(
                    checkUpdateConfigurationLinkLabel.Location.X + checkUpdateConfigurationLinkLabel.Size.Width + 2,
                    tickPictureBox.Location.Y);
            packagesList.DoubleBuffer();
            tabControl1.DoubleBuffer();

            var credentialsDialog = new CredentialsDialog();
            if (credentialsDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Program.FtpPassword = AESManager.Decrypt(Convert.FromBase64String(Project.FtpPassword),
                        credentialsDialog.Username, credentialsDialog.Password.ToString());
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.",
                        "The entred credentials are invalid. Try again?", PopupButtons.YesNo);
                }
            }

            // Check for network
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.",
                    String.Format(
                        "The configuration file could not be checked because no network connection is available. {0}",
                        updateInfoFileUrl), PopupButtons.OK);
            }
            else
            {
                // Initialize configuration
                StartCheckingUpdateInfo();
            }

            // Statistics
            string lastUpdatedInfoString = String.Format("Last updated on {0} at {1}.",
                DateTime.Now.ToString("dd.MM.yyy"), DateTime.Now.ToString("HH:mm:ss tt"));
            lastUpdatedLabel.Text = lastUpdatedInfoString;

            // All actions now are done by the user ...
            isSetByUser = true;

            try
            {
                string myConnectionString = "SERVER=web1.php-friends.de;" +
                                            "DATABASE=vstradesql2;" +
                                            "UID=vstrade_ext;" +
                                            "PASSWORD=5HmvI2VfRu9WGdeCg7jB;";

                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "MySQL Exception",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unknown Exception",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MySqlCommand command = myConnection.CreateCommand();
            command.CommandText =
                "SELECT Version_ID, v.Version, COUNT(*) AS 'Anzahl Downloads' FROM Download LEFT JOIN Version v ON (v.ID = Version_ID) GROUP BY Version_ID;";

            MySqlDataReader Reader = command.ExecuteReader();
            ListViewItem item1;

            while (Reader.Read())
            {
                // Create one ListViewItem and add Data
                item1 = new ListViewItem(Reader.GetValue(1).ToString(), 0);
                item1.SubItems.Add(Reader.GetValue(2).ToString());

                extendedListView1.Items.Add(item1);
            }
            Reader.Close();
        }

        private void ProjectDialog_Shown(object sender, EventArgs e)
        {
            packagesList.MakeCollapsable();
        }

        private void ProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ListViewItem matchingItem = packagesList.FindItemWithText(searchTextBox.Text, true, 0);
                int index;

                if (matchingItem != null)
                {
                    index = matchingItem.Index;
                    packagesList.Items[index].Selected = true;
                }
                else
                    packagesList.SelectedItems.Clear();

                searchTextBox.Clear();
                e.SuppressKeyPress = true;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var packageAddDialog = new PackageAddDialog();
            var existingUpdateVersions = new List<UpdateVersion>();
            foreach (ListViewItem lvi in packagesList.Items)
            {
                existingUpdateVersions.Add(new UpdateVersion(lvi.Tag.ToString()));
            }
            if (existingUpdateVersions != null)
                packageAddDialog.NewestVersion = UpdateVersion.GetHighestUpdateVersion(existingUpdateVersions);
            else
                packageAddDialog.NewestVersion = new UpdateVersion(0, 1, 0, 0);

            packageAddDialog.Project = Project;

            if (packageAddDialog.ShowDialog() == DialogResult.OK)
            {
                if (packageAddDialog.Project.HasUnsavedChanges)
                {
                    Project = packageAddDialog.Project;
                    File.WriteAllText(Project.Path, Serializer.Serialize(Project));

                    packagesList.Items.Clear();
                    InitializePackageItems();
                    InitializeProjectDetails();
                }
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            string updateUrl = updateUrlTextBox.Text;
            if (!updateUrl.EndsWith("/"))
                updateUrl += "/";

            string vbSource =
                String.Format(
                    "Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New UpdateVersion(\"0.0.0.0\"))",
                    UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);
            string cSharpSource =
                String.Format(
                    "UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new UpdateVersion(\"0.0.0.0\"));",
                    UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);

            try
            {
                if (programmingLanguageComboBox.SelectedIndex == 0)
                    Clipboard.SetText(vbSource);
                else if (programmingLanguageComboBox.SelectedIndex == 1)
                    Clipboard.SetText(cSharpSource);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while copying source.", ex, PopupButtons.OK);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var packageEditDialog = new PackageEditDialog();
            packageEditDialog.Project = Project;
            try
            {
                var allConfigurations = new List<UpdateConfiguration>();
                allConfigurations =
                    UpdateConfiguration.LoadFromFile(Path.Combine(Program.Path, "Projects", Project.Name,
                        packageVersion.ToString(), "updates.json"));
                packageEditDialog.PackageConfiguration =
                    allConfigurations.Where(item => item.Version == packageVersion.ToString()).First();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package configuration.", ex,
                    PopupButtons.OK);
                return;
            }
            packageEditDialog.ShowDialog();
        }

        private void editFtpButton_Click(object sender, EventArgs e)
        {
            var ftpEditDialog = new FtpEditDialog();
            ftpEditDialog.CanCancel = true;
            ftpEditDialog.Project = Project;
            if (ftpEditDialog.ShowDialog() == DialogResult.OK)
            {
                if (ftpEditDialog.Project.HasUnsavedChanges)
                {
                    Project = ftpEditDialog.Project;

                    try
                    {
                        ApplicationInstance.SaveProject(Project.Path, Project);
                        InitializeFtpData();
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.OK);
                    }
                }
                InitializeFtpData();
                InitializeProjectDetails();
            }
        }

        private void editProjectButton_Click(object sender, EventArgs e)
        {
            var projectEditDialog = new ProjectEditDialog();
            projectEditDialog.Project = Project;
            if (projectEditDialog.ShowDialog() == DialogResult.OK)
            {
                if (projectEditDialog.Project.HasUnsavedChanges)
                {
                    string projectFilePath = Project.Path;
                    Project = projectEditDialog.Project;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, Project);
                        if (projectFilePath != Project.Path)
                            File.Move(projectFilePath, Project.Path);

                        InitializeProjectDetails();
                        updateInfoFileUrl = UriConnecter.ConnectUri(updateUrlTextBox.Text, "updates.json");
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.OK);
                    }
                }
            }
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            var historyDialog = new HistoryDialog();
            historyDialog.Project = Project;
            historyDialog.ShowDialog();
        }

        private void packagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packagesList.FocusedItem != null)
            {
                packageVersion = (UpdateVersion) packagesList.FocusedItem.Tag;
                packageDescription = packagesList.FocusedItem.SubItems[3].Text;

                if (packagesList.FocusedItem.Group == packagesList.Groups[0])
                {
                    groupIndex = 0;
                    editButton.Enabled = false;
                    uploadButton.Enabled = false;
                }
                else
                {
                    groupIndex = 1;
                    editButton.Enabled = true;
                    uploadButton.Enabled = true;
                }
            }
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUIState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (Control c in Controls)
                {
                    if (c.Visible)
                        c.Enabled = enabled;
                }

                if (!enabled)
                {
                    allowCancel = false;
                    loadingPanel.Visible = true;
                    loadingPanel.Location = new Point(179, 135);
                    loadingPanel.BringToFront();

                    editButton.Enabled = false;
                    uploadButton.Enabled = false;
                }
                else
                {
                    allowCancel = true;
                    loadingPanel.Visible = false;
                }
            }));
        }

        private void browseAssemblyButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.SupportMultiDottedExtensions = false;
                fileDialog.Filter = "Executable files (*.exe)|*.exe|Executable extension files (*.dll)|*.dll";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Assembly projectAssembly = Assembly.LoadFile(fileDialog.FileName);
                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                    }
                    catch
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Invalid assembly found.",
                            "The version of the assembly for the selected executable (extension) file could not be read.",
                            PopupButtons.OK);
                        loadFromAssemblyCheckBox.Checked = false;
                        return;
                    }

                    assemblyPathTextBox.Text = fileDialog.FileName;
                    Project.AssemblyVersionPath = assemblyPathTextBox.Text;
                    ApplicationInstance.SaveProject(Project.Path, Project);
                    InitializeProjectDetails();
                }
            }
        }

        private void loadFromAssemblyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loadFromAssemblyCheckBox.Checked)
            {
                assemblyPathTextBox.Enabled = true;
                browseAssemblyButton.Enabled = true;
            }
            else
            {
                assemblyPathTextBox.Enabled = false;
                browseAssemblyButton.Enabled = false;

                if (isSetByUser)
                {
                    Project.AssemblyVersionPath = null;
                    ApplicationInstance.SaveProject(Project.Path, Project);
                    assemblyPathTextBox.Clear();
                    InitializeProjectDetails();
                }
            }
        }

        #region "Initializing"

        private bool hasFailedOnLoading;

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
            catch (NullReferenceException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
        }

        /// <summary>
        ///     Sets all the details for the project.
        /// </summary>
        private void InitializeProjectDetails()
        {
            try
            {
                Invoke(new Action(() =>
                {
                    nameTextBox.Text = Project.Name;
                    updateUrlTextBox.Text = Project.UpdateUrl;
                    ftpHostTextBox.Text = Project.FtpHost;
                    ftpDirectoryTextBox.Text = Project.FtpDirectory;
                    amountLabel.Text = Project.ReleasedPackages.ToString();

                    if (!String.IsNullOrEmpty(Project.AssemblyVersionPath))
                    {
                        loadFromAssemblyCheckBox.Checked = true;
                        assemblyPathTextBox.Text = Project.AssemblyVersionPath;
                    }

                    if (Project.NewestPackage != null)
                        newestPackageLabel.Text = Project.NewestPackage;
                    else
                        newestPackageLabel.Text = "-";

                    projectIdTextBox.Text = Project.Id;
                    publicKeyTextBox.Text = Project.PublicKey;
                }));
            }
            catch (IOException ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex,
                                PopupButtons.OK)));
                hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.",
                                "The project file is corrupt and does not have the necessary arguments.",
                                PopupButtons.OK)));
                hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex,
                                PopupButtons.OK)));
                hasFailedOnLoading = true;
            }
        }

        /// <summary>
        ///     Adds the package items to the listview.
        /// </summary>
        private void InitializePackageItems()
        {
            Invoke(new Action(() =>
            {
                if (packagesList.Items.Count != 0)
                    packagesList.Items.Clear();
            }));

            if (Project.Packages != null)
            {
                foreach (UpdatePackage package in Project.Packages)
                {
                    try
                    {
                        var lviPackage = new ListViewItem(package.Version.FullText);
                        var diPackage = new DirectoryInfo(package.LocalPackagePath);
                        lviPackage.SubItems.Add(diPackage.CreationTime.ToString());

                        // Get the size of the package
                        var fiPackageFile = new FileInfo(package.LocalPackagePath);
                        long sizeInBytes = fiPackageFile.Length;
                        float size;
                        string sizeText;

                        if (sizeInBytes > 104857.6)
                        {
                            size = (float) Math.Round(sizeInBytes / Mb, 1);
                            sizeText = String.Format("{0} MB", size);
                        }
                        else
                        {
                            size = (float) Math.Round(sizeInBytes / Kb, 1);
                            sizeText = String.Format("{0} KB", size);
                        }

                        lviPackage.SubItems.Add(sizeText);
                        lviPackage.SubItems.Add(package.Description);

                        if (package.IsReleased)
                            lviPackage.Group = packagesList.Groups[0];
                        else
                            lviPackage.Group = packagesList.Groups[1];

                        lviPackage.Tag = package.Version;
                        Invoke(new Action(() => packagesList.Items.Add(lviPackage)));
                    }
                    catch (IOException ex)
                    {
                        var dialogResult = DialogResult.None;
                        Invoke(
                            new Action(
                                () =>
                                    dialogResult =
                                        Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.",
                                            String.Format(
                                                "{0} - Should the entry for package {1} in the project be deleted in order to hide this error the next time?",
                                                GetNameOfExceptionType(ex), package.Version), PopupButtons.YesNo)));

                        if (dialogResult == DialogResult.Yes)
                        {
                            // Remove the package info from the project file and stop the loading
                            Project.Packages.RemoveAll(item => item.Version == package.Version);

                            if (Project.ReleasedPackages != 0)
                                // Set the released packages again with subtracting 1 for the project that was just removed
                                Project.ReleasedPackages -= 1;

                            // The released packages
                            if (Project.Packages != null && Project.Packages.Count != 0)
                                Project.NewestPackage = Project.Packages.Last().Version.FullText;
                            else
                                Project.NewestPackage = null;

                            ApplicationInstance.SaveProject(Project.Path, Project);
                            InitializeProjectDetails();
                            InitializePackageItems(); //Re-call the methods
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.", ex,
                                        PopupButtons.OK)));
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the name/description of the current exception.
        /// </summary>
        private string GetNameOfExceptionType(Exception ex)
        {
            int hrEx = Marshal.GetHRForException(ex);
            switch (hrEx)
            {
                case COR_E_DIRECTORYNOTFOUND:
                    return "DirectoryNotFound";
                case COR_E_ENDOFSTREAM:
                    return "EndOfStream";
                case COR_E_FILELOAD:
                    return "FileLoadException";
                case COR_E_FILENOTFOUND:
                    return "FileNotFound";
            }
            return "Unknown Exception";
        }

        #endregion

        #region "Upload"

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (packagesList.FocusedItem != null)
            {
                if (cancellationToken != null)
                {
                    cancellationToken.Dispose();
                    cancellationToken = new CancellationTokenSource();
                }

                ThreadPool.QueueUserWorkItem(delegate { UploadPackage(); }, null);
            }
        }

        /// <summary>
        ///     Provides a new thread that uploads the package.
        /// </summary>
        private void UploadPackage()
        {
            try
            {
                SetUIState(false);

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
                    return;

                string packageFileName = String.Empty;
                Invoke(new Action(() => { packageFileName = String.Format("{0}.zip", projectIdTextBox.Text); }));

                string updateConfigurationFilePath = Path.Combine(Program.Path, "Projects", Project.Name,
                    packageVersion.ToString(), "updates.json");
                string packagePath = Project.Packages.Where(x => x.Version == packageVersion).First().LocalPackagePath;
                try
                {
                    ftp.UploadPackage(packagePath, packageVersion.ToString());
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

                if (cancellationToken.IsCancellationRequested)
                    return;

                while (!ftp.HasFinishedUploading)
                {
                    continue;
                }

                if (ftp.PackageUploadException != null)
                {
                    if (ftp.PackageUploadException.InnerException != null)
                    {
                        if (ftp.PackageUploadException.InnerException.GetType() == typeof (WebException))
                        {
                            switch (
                                ((int)
                                    ((FtpWebResponse) ((WebException) ftp.PackageUploadException).Response).StatusCode))
                            {
                                case 550:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    String.Format(
                                                        "The server returned 550. Make sure the given FTP-directory exists and then try again. - {0}",
                                                        ftpDirectoryTextBox.Text), PopupButtons.OK)));
                                    break;
                                case 530:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    "The server login failed. Make sure the login credentials are correct and then try again.",
                                                    PopupButtons.OK)));
                                    break;
                                default:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    ftp.PackageUploadException.InnerException, PopupButtons.OK)));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Exception ex = null;
                        if (ftp.PackageUploadException.InnerException != null)
                            ex = ftp.PackageUploadException.InnerException;
                        else
                            ex = ftp.PackageUploadException;

                        // Just handle it normally
                        Invoke(new Action(() =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                PopupButtons.OK);
                            cancellationToken.Cancel();
                        }));
                    }
                    cancellationToken.Cancel();
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    ftp.DeleteFile("updates.json"); // Configuration
                    ftp.UploadFile(updateConfigurationFilePath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                    PopupButtons.OK)));
                    cancellationToken.Cancel();
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                // Write to log
                updateLog.Write(LogEntry.Upload, packageVersion.ToString());

                Project.Packages.Where(x => x.Version == packageVersion).First().IsReleased = true;
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    Project.Packages.Where(x => x.Version == packageVersion).First().IsReleased = true;
                    ApplicationInstance.SaveProject(Project.Path, Project);
                }

                SetUIState(true);
                InitializeProjectDetails();
                InitializePackageItems();
            }
        }

        /// <summary>
        ///     Called when the progress changes.
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 100)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text =
                                String.Format(uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage * 2))));
            }
        }

        #endregion

        #region "Configuration"

        /// <summary>
        ///     Starts checking if the update configuration exists.
        /// </summary>
        private void StartCheckingUpdateInfo()
        {
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.",
                    String.Format(
                        "The configuration file could not be checked because no network connection is available. {0}",
                        updateInfoFileUrl), PopupButtons.OK);
                return;
            }

            checkingUrlPictureBox.Visible = true;

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = new CancellationTokenSource();
            }

            ThreadPool.QueueUserWorkItem(delegate { CheckUpdateConfigurationStatus(updateInfoFileUrl); }, null);
        }

        private void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StartCheckingUpdateInfo();
        }

        private void CheckUpdateConfigurationStatus(Uri infoFileUrl)
        {
            // Check if file exists on the server
            WebRequest request = WebRequest.Create(infoFileUrl);
            request.Timeout = 5000;
            request.Method = "HEAD";

            // Cheeck for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };

            try
            {
                configResponse = (HttpWebResponse) request.GetResponse();
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    SetUIState(false);
                    loadingPanel.Location = new Point(187, 145);
                    loadingPanel.BringToFront();
                    loadingLabel.Text = "Updating info file...";

                    checkUpdateConfigurationLinkLabel.Enabled = false;
                    checkingUrlPictureBox.Visible = false;
                }));

                // Create file
                string temporaryNewUpdateInfoFile = Path.Combine(Program.Path, "updates.json");
                if (!File.Exists(temporaryNewUpdateInfoFile))
                {
                    using (FileStream newUpdateInfoFileStream = File.Create(temporaryNewUpdateInfoFile))
                    {
                    }
                }

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
                    return;

                try
                {
                    // Upload the file now
                    ftp.UploadFile(temporaryNewUpdateInfoFile);
                }
                catch (Exception ex)
                {
                    // Get the status code to handle it 
                    if (ex.GetType() == typeof (WebException))
                    {
                        switch (((int) ((FtpWebResponse) ((WebException) ex).Response).StatusCode))
                        {
                            case 550:
                                Invoke(
                                    new Action(
                                        () =>
                                            Popup.ShowPopup(this, SystemIcons.Error,
                                                "Error while creating new config file.",
                                                String.Format(
                                                    "The server returned 550. Make sure the given FTP-directory exists and then try again. - {0}",
                                                    ftpDirectoryTextBox.Text), PopupButtons.OK)));
                                break;
                            case 530:
                                Invoke(
                                    new Action(
                                        () =>
                                            Popup.ShowPopup(this, SystemIcons.Error,
                                                "Error while creating new config file.",
                                                "The server login failed. Make sure the login credentials are correct and then try again.",
                                                PopupButtons.OK)));
                                break;
                            default:
                                Invoke(
                                    new Action(
                                        () =>
                                            Popup.ShowPopup(this, SystemIcons.Error,
                                                "Error while creating new config file.", ex, PopupButtons.OK)));
                                break;
                        }
                    }
                    else
                    {
                        // Just handle it normally
                        Invoke(new Action(() =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                PopupButtons.OK);
                            cancellationToken.Cancel();
                        }));
                    }
                    // Cancel
                    cancellationToken.Cancel();
                }
            }
            finally
            {
                Invoke(new Action(() =>
                {
                    SetUIState(true);
                    loadingPanel.Visible = false;
                    checkingUrlPictureBox.Visible = false;
                    checkUpdateConfigurationLinkLabel.Enabled = true;

                    if (!cancellationToken.IsCancellationRequested)
                        tickPictureBox.Visible = true; // Upload succeeded
                }));
                allowCancel = true;

                if (configResponse != null)
                    configResponse.Close();
            }
        }

        #endregion

        #region "Deleting"

        private bool shouldKeepErrorsSecret;

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packagesList.FocusedItem != null)
            {
                DialogResult answer = Popup.ShowPopup(this, SystemIcons.Question,
                    String.Format("Delete the update package {0}?", packageVersion),
                    "Are you sure that you want to delete this package?", PopupButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    if (cancellationToken != null)
                    {
                        cancellationToken.Dispose();
                        cancellationToken = new CancellationTokenSource();
                    }

                    ThreadPool.QueueUserWorkItem(delegate { DeletePackage(groupIndex); }, null);
                }
            }
        }

        /// <summary>
        ///     Initializes a new thread for deleting the package.
        /// </summary>
        private void DeletePackage(int groupIndex)
        {
            try
            {
                SetUIState(false);
                if (Equals(this.groupIndex, 0)) // Must be deleted online, too.
                {
                    Invoke(new Action(() => loadingLabel.Text = "Waiting for credentials..."));
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
                        SetUIState(false);
                        return;
                    }

                    Invoke(new Action(() => loadingLabel.Text = "Deleting package..."));

                    try
                    {
                        // Delete package folder
                        ftp.DeleteDirectory(packageVersion.ToString());
                    }
                    catch (Exception ex)
                    {
                        // Get the status code to handle it 
                        if (ex.GetType() == typeof (WebException))
                        {
                            switch (((int) ((FtpWebResponse) ((WebException) ex).Response).StatusCode))
                            {
                                case 404:
                                    shouldKeepErrorsSecret = true;
                                    break;
                                case 450:
                                    shouldKeepErrorsSecret = true;
                                    break;
                                case 530:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while deleting the package directory.",
                                                    "The server login failed. Make sure the login credentials are correct and then try again.",
                                                    PopupButtons.OK)));
                                    break;
                                default:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while deleting the package directory.", ex, PopupButtons.OK)));
                                    break;
                            }
                        }
                        else
                        {
                            // Just handle it normally
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while deleting the package directory.", ex, PopupButtons.OK)));
                        }

                        if (!shouldKeepErrorsSecret)
                        {
                            // Cancel
                            cancellationToken.Cancel();
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        SetUIState(true);
                        return;
                    }

                    Invoke(new Action(() => loadingLabel.Text = "Getting old configuration..."));

                    var updateConfig = new List<UpdateConfiguration>();
                    try
                    {
                        updateConfig = UpdateConfiguration.DownloadUpdateConfiguration(updateInfoFileUrl, Project.Proxy);
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                {
                                    Popup.ShowPopup(this, SystemIcons.Error,
                                        "Error while downloading the old configuration.", ex, PopupButtons.OK);
                                }));
                    }

                    if (updateConfig != null && updateConfig.Count != 0)
                    {
                        bool isContainingCurrentVersion = false;
                        foreach (UpdateConfiguration configEntry in updateConfig)
                        {
                            if (new UpdateVersion(configEntry.Version) == packageVersion)
                                isContainingCurrentVersion = true;
                        }

                        string updateInfoFilePath = Path.Combine(Program.Path, "updates.json");
                            // The path to the temporary new update config

                        if (isContainingCurrentVersion)
                        {
                            updateConfig.Remove(
                                updateConfig.Where(item => new UpdateVersion(item.Version) == packageVersion).First());
                            string content = Serializer.Serialize(updateConfig);

                            try
                            {
                                File.WriteAllText(updateInfoFilePath, content);
                            }
                            catch (Exception ex)
                            {
                                Invoke(
                                    new Action(
                                        () =>
                                            Popup.ShowPopup(this, SystemIcons.Error,
                                                "Error while writing to local configuration file.", ex, PopupButtons.OK)));
                                cancellationToken.Cancel();
                            }

                            if (cancellationToken.IsCancellationRequested)
                            {
                                SetUIState(true);
                                return;
                            }
                        }

                        Invoke(new Action(() => loadingLabel.Text = "Uploading new configuration..."));

                        try
                        {
                            // Upload new configuration file
                            ftp.UploadFile(updateInfoFilePath);
                        }
                        catch (Exception ex)
                        {
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while uploading new configuration file.", ex, PopupButtons.OK)));
                            cancellationToken.Cancel();
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            SetUIState(true);
                            return;
                        }

                        File.WriteAllText(updateInfoFilePath, String.Empty);
                    }
                }

                Invoke(new Action(() => loadingLabel.Text = "Deleting local directory..."));

                try
                {
                    // Delete local folder
                    Directory.Delete(Path.Combine(Program.Path, "Projects", Project.Name, packageVersion.ToString()),
                        true);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting local package directory.",
                                    ex, PopupButtons.OK)));
                }

                try
                {
                    // Remove current package entry and save the edited project
                    Project.Packages.RemoveAll(x => x.Version == packageVersion);
                    if (Project.ReleasedPackages != 0) // The amount of released packages
                        Project.ReleasedPackages -= 1;

                    // The newest package
                    if (Project.Packages != null && Project.Packages.Count != 0)
                        Project.NewestPackage = Project.Packages.Last().Version.FullText;
                    else
                        Project.NewestPackage = null;

                    ApplicationInstance.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                    PopupButtons.OK)));
                }
            }
            finally
            {
                updateLog.Write(LogEntry.Delete, packageVersion.ToString());
                ApplicationInstance.SaveProject(Project.Path, Project);

                SetUIState(true);
                Invoke(new Action(() =>
                {
                    packagesList.Items.Clear();
                    InitializePackageItems();
                    InitializeProjectDetails();
                }));
            }
        }

        #endregion
    }
}