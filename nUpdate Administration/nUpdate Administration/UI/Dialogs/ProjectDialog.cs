using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.History;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectDialog : BaseDialog
    {
        private bool isSetByUser = false;
        private bool allowCancel = true;
        private int groupIndex = 0;
        private UpdateVersion packageVersion = null;
        private string packageDescription;
        private Uri updateInfoFileUrl;
        private HttpWebResponse configResponse = null;

        private const float Kb = 1024;
        private const float Mb = 1048577;

        private FTPManager ftp = new FTPManager();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private Log updateLog = new Log();

        private const int COR_E_ENDOFSTREAM = unchecked((int)0x80070026);
        private const int COR_E_FILELOAD = unchecked((int)0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int)0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int)0x80070003);
        private const int COR_E_PATHTOOLONG = unchecked((int)0x800700CE);
        private const int COR_E_IO = unchecked((int)0x80131620);

        public ProjectDialog()
        {
            InitializeComponent();
        }

        #region "Localization"

        private string invalidArgumentCaption = "Invalid argument found.";
        private string invalidArgumentText = "The entry {0} can't be parsed to {1}.";
        private string gettingUrlErrorCaption = "Error while getting url.";
        private string readingPackageBytesErrorCaption = "Reading package bytes failed.";
        private string invalidServerDirectoryErrorCaption = "Invalid server directory.";
        private string invalidServerDirectoryErrorText = "The directory for the update files on the server is not valid. Please edit it.";
        private string ftpDataLoadErrorCaption = "Failed to load FTP-data.";
        private string relativeUriErrorText = "The server-directory can't be set as a relative uri.";
        private string savingInformationErrorCaption = "Saving package information failed.";
        private string uploadFailedErrorCaption = "Upload failed.";

        private string uploadingPackageInfoText = "Uploading package - {0}";
        private string uploadingConfigInfoText = "Uploading configuration...";

        /// <summary>
        /// Sets the language.
        /// </summary>
        public void SetLanguage()
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

            this.Text = String.Format("{0} - {1}", this.Project.Name, ls.ProductTitle);
            this.overviewHeader.Text = ls.ProjectDialogOverviewText;
            this.overviewTabPage.Text = ls.ProjectDialogOverviewTabText;
            this.packagesTabPage.Text = ls.ProjectDialogPackagesTabText;
            this.nameLabel.Text = ls.ProjectDialogNameLabelText;
            this.updateUrlLabel.Text = ls.ProjectDialogUpdateUrlLabelText;
            this.ftpHostLabel.Text = ls.ProjectDialogFtpHostLabelText;
            this.ftpDirectoryLabel.Text = ls.ProjectDialogFtpDirectoryLabelText;
            this.releasedPackagesAmountLabel.Text = ls.ProjectDialogPackagesAmountLabelText;
            this.newestPackageReleasedLabel.Text = ls.ProjectDialogNewestPackageLabelText;
            this.infoFileloadingLabel.Text = ls.ProjectDialogInfoFileloadingLabelText;
            this.checkUpdateConfigurationLinkLabel.Text = ls.ProjectDialogCheckInfoFileStatusLinkLabelText;
            this.projectDataHeader.Text = ls.ProjectDialogProjectDataText;
            this.publicKeyLabel.Text = ls.ProjectDialogPublicKeyLabelText;
            this.projectIdLabel.Text = ls.ProjectDialogProjectIdLabelText;
            this.stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

            this.addButton.Text = ls.ProjectDialogAddButtonText;
            this.editButton.Text = ls.ProjectDialogEditButtonText;
            this.deleteButton.Text = ls.ProjectDialogDeleteButtonText;
            this.uploadButton.Text = ls.ProjectDialogUploadButtonText;
            this.historyButton.Text = ls.ProjectDialogHistoryButtonText;

            this.packagesList.Columns[0].Text = ls.ProjectDialogVersionText;
            this.packagesList.Columns[1].Text = ls.ProjectDialogReleasedText;
            this.packagesList.Columns[2].Text = ls.ProjectDialogSizeText;
            this.packagesList.Columns[3].Text = ls.ProjectDialogDescriptionText;

            this.searchTextBox.Cue = ls.ProjectDialogSearchText;
        }

        #endregion

        MySqlConnection myConnection;
        private void ProjectDialog_Load(object sender, EventArgs e)
        {
            // Set the handler for the progress
            this.ftp.ProgressChanged += ProgressChanged;

            this.InitializeProjectDetails(); // Initialize everything, here project details
            if (this.hasFailedOnLoading)
            {
                this.Close();
                return;
            }

            this.InitializeFtpData(); // ... FTP-data
            if (this.hasFailedOnLoading)
            {
                this.Close();
                return;
            }

            this.InitializePackageItems(); // ... package items

            // The log preferences
            this.updateLog.Project = this.Project;

            // The programming language
            switch (this.Project.ProgrammingLanguage)
            {
                case "VB.NET":
                    this.programmingLanguageComboBox.SelectedIndex = 0;
                    break;
                case "C#":
                    this.programmingLanguageComboBox.SelectedIndex = 1;
                    break;
            }

            // Set the language for this dialog
            this.SetLanguage();

            // Update-URL
            if (!this.Project.UpdateUrl.EndsWith("/")) 
            {
                this.Project.UpdateUrl += "/";
            }
            this.updateInfoFileUrl = UriConnecter.ConnectUri(this.Project.UpdateUrl, "updates.json");

            // Control position- and draw settings
            this.checkingUrlPictureBox.Location = new Point(this.checkUpdateConfigurationLinkLabel.Location.X + this.checkUpdateConfigurationLinkLabel.Size.Width + 2, this.checkingUrlPictureBox.Location.Y);
            this.tickPictureBox.Location = new Point(this.checkUpdateConfigurationLinkLabel.Location.X + this.checkUpdateConfigurationLinkLabel.Size.Width + 2, this.tickPictureBox.Location.Y);
            this.packagesList.DoubleBuffer();
            this.tabControl1.DoubleBuffer();

            var credentialsDialog = new CredentialsDialog();
            if (credentialsDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Program.FtpPassword = AESManager.Decrypt(Convert.FromBase64String(this.Project.FtpPassword), credentialsDialog.Username, credentialsDialog.Password.ToString());
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid credentials.", "The entred credentials are invalid. Try again?", PopupButtons.YesNo);
                }
            }

            // Check for network
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.", String.Format("The configuration file could not be checked because no network connection is available. {0}", updateInfoFileUrl), PopupButtons.OK);
            }
            else
            {
                // Initialize configuration
                this.StartCheckingUpdateInfo();
            }

            // Statistics
            string lastUpdatedInfoString = String.Format("Last updated on {0} at {1}.", DateTime.Now.ToString("dd.MM.yyy"), DateTime.Now.ToString("HH:mm:ss tt"));
            this.lastUpdatedLabel.Text = lastUpdatedInfoString;

            // All actions now are done by the user ...
            this.isSetByUser = true;

            try
            {
                string myConnectionString = "SERVER=web1.php-friends.de;" +
                            "DATABASE=vstradesql2;" +
                            "UID=vstrade_ext;" +
                            "PASSWORD=5HmvI2VfRu9WGdeCg7jB;";

                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
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
            command.CommandText = "SELECT Version_ID, v.Version, COUNT(*) AS 'Anzahl Downloads' FROM Download LEFT JOIN Version v ON (v.ID = Version_ID) GROUP BY Version_ID;";

            MySqlDataReader Reader = command.ExecuteReader();
            ListViewItem item1;

            while (Reader.Read())
            {
                // Create one ListViewItem and add Data
                item1 = new ListViewItem(Reader.GetValue(1).ToString(), 0);
                item1.SubItems.Add(Reader.GetValue(2).ToString());

                this.extendedListView1.Items.Add(item1);
            }
            Reader.Close();
        }

        private void ProjectDialog_Shown(object sender, EventArgs e)
        {
            this.packagesList.MakeCollapsable();
        }

        private void ProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.allowCancel)
            {
                e.Cancel = true;
            }
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ListViewItem matchingItem = this.packagesList.FindItemWithText(this.searchTextBox.Text, true, 0);
                int index;

                if (matchingItem != null)
                {
                    index = matchingItem.Index;
                    this.packagesList.Items[index].Selected = true;
                }
                else
                {
                    this.packagesList.SelectedItems.Clear();
                }

                this.searchTextBox.Clear();
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
            {
                packageAddDialog.NewestVersion = UpdateVersion.GetHighestUpdateVersion(existingUpdateVersions);
            }
            else
            {
                packageAddDialog.NewestVersion = new UpdateVersion(0, 1, 0, 0);
            }

            packageAddDialog.Project = this.Project;

            if (packageAddDialog.ShowDialog() == DialogResult.OK)
            {
                if (packageAddDialog.Project.HasUnsavedChanges)
                {
                    this.Project = packageAddDialog.Project;
                    File.WriteAllText(this.Project.Path, Serializer.Serialize(this.Project));

                    this.packagesList.Items.Clear();
                    this.InitializePackageItems();
                    this.InitializeProjectDetails();
                }
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            string updateUrl = this.updateUrlTextBox.Text;
            if (!updateUrl.EndsWith("/"))
            {
                updateUrl += "/";
            }

            string vbSource = String.Format("Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New UpdateVersion(\"0.0.0.0\"))",
                UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);
            string cSharpSource = String.Format("UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new UpdateVersion(\"0.0.0.0\"));", 
                UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);

            try
            {
                if (this.programmingLanguageComboBox.SelectedIndex == 0)
                {
                    Clipboard.SetText(vbSource);
                }
                else if (this.programmingLanguageComboBox.SelectedIndex == 1)
                {
                    Clipboard.SetText(cSharpSource);
                }
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while copying source.", ex, PopupButtons.OK);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var packageEditDialog = new PackageEditDialog();
            packageEditDialog.Project = this.Project;
            try
            {
                List<UpdateConfiguration> allConfigurations = new List<UpdateConfiguration>();
                allConfigurations = UpdateConfiguration.LoadFromFile(Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString(), "updates.json"));
                packageEditDialog.PackageConfiguration = allConfigurations.Where(item => item.Version == this.packageVersion.ToString()).First();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package configuration.", ex, PopupButtons.OK);
                return;
            }
            packageEditDialog.ShowDialog();
        }

        private void editFtpButton_Click(object sender, EventArgs e)
        {
            var ftpEditDialog = new FtpEditDialog();
            ftpEditDialog.CanCancel = true;
            ftpEditDialog.Project = this.Project;
            if (ftpEditDialog.ShowDialog() == DialogResult.OK)
            {
                if (ftpEditDialog.Project.HasUnsavedChanges)
                {
                    this.Project = ftpEditDialog.Project;

                    try
                    {
                        ApplicationInstance.SaveProject(this.Project.Path, this.Project);
                        this.InitializeFtpData();
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.OK);
                    }
                }
                this.InitializeFtpData();
                this.InitializeProjectDetails();
            }
        }

        private void editProjectButton_Click(object sender, EventArgs e)
        {
            var projectEditDialog = new ProjectEditDialog();
            projectEditDialog.Project = this.Project;
            if (projectEditDialog.ShowDialog() == DialogResult.OK)
            {
                if (projectEditDialog.Project.HasUnsavedChanges)
                {
                    string projectFilePath = this.Project.Path;
                    this.Project = projectEditDialog.Project;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, this.Project);
                        if (projectFilePath != this.Project.Path)
                        {
                            File.Move(projectFilePath, this.Project.Path);
                        }

                        this.InitializeProjectDetails();
                        this.updateInfoFileUrl = UriConnecter.ConnectUri(this.updateUrlTextBox.Text, "updates.json");
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
            historyDialog.Project = this.Project;
            historyDialog.ShowDialog();
        }

        private void packagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.packagesList.FocusedItem != null)
            {
                this.packageVersion = (UpdateVersion)this.packagesList.FocusedItem.Tag;
                this.packageDescription = this.packagesList.FocusedItem.SubItems[3].Text;

                if (this.packagesList.FocusedItem.Group == this.packagesList.Groups[0])
                {
                    this.groupIndex = 0;
                    this.editButton.Enabled = false;
                    this.uploadButton.Enabled = false;
                }
                else
                {
                    this.groupIndex = 1;
                    this.editButton.Enabled = true;
                    this.uploadButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUIState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (Control c in this.Controls)
                {
                    if (c.Visible)
                    {
                        c.Enabled = enabled;
                    }
                }

                if (!enabled)
                {
                    this.allowCancel = false;
                    this.loadingPanel.Visible = true;
                    this.loadingPanel.Location = new Point(179, 135);
                    this.loadingPanel.BringToFront();

                    this.editButton.Enabled = false;
                    this.uploadButton.Enabled = false;
                }
                else
                {
                    this.allowCancel = true;
                    this.loadingPanel.Visible = false;
                }
            }));
        }

        #region "Initializing"

        private bool hasFailedOnLoading = false;

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
            catch (NullReferenceException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
        }

        /// <summary>
        /// Sets all the details for the project.
        /// </summary>
        private void InitializeProjectDetails()
        {
            try
            {
                Invoke(new Action(() =>
                    {
                        this.nameTextBox.Text = this.Project.Name;
                        this.updateUrlTextBox.Text = this.Project.UpdateUrl;
                        this.ftpHostTextBox.Text = this.Project.FtpHost;
                        this.ftpDirectoryTextBox.Text = this.Project.FtpDirectory;
                        this.amountLabel.Text = this.Project.ReleasedPackages.ToString();

                        if (!String.IsNullOrEmpty(this.Project.AssemblyVersionPath))
                        {
                            this.loadFromAssemblyCheckBox.Checked = true;
                            this.assemblyPathTextBox.Text = this.Project.AssemblyVersionPath;
                        }

                        if (this.Project.NewestPackage != null)
                        {
                            this.newestPackageLabel.Text = this.Project.NewestPackage;
                        }
                        else
                        {
                            this.newestPackageLabel.Text = "-";
                        }

                        this.projectIdTextBox.Text = Project.Id;
                        this.publicKeyTextBox.Text = Project.PublicKey;
                    }));
            }
            catch (IOException ex)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK)));
                this.hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK)));
                this.hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK)));
                this.hasFailedOnLoading = true;
            }
        }

        /// <summary>
        /// Adds the package items to the listview.
        /// </summary>
        private void InitializePackageItems()
        {
            Invoke(new Action(() =>
                {
                    if (this.packagesList.Items.Count != 0)
                    {
                        this.packagesList.Items.Clear();
                    }
                }));

            if (this.Project.Packages != null)
            {
                foreach (UpdatePackage package in this.Project.Packages)
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
                            size = (float)Math.Round(sizeInBytes / Mb, 1);
                            sizeText = String.Format("{0} MB", size);
                        }
                        else
                        {
                            size = (float)Math.Round(sizeInBytes / Kb, 1);
                            sizeText = String.Format("{0} KB", size);
                        }

                        lviPackage.SubItems.Add(sizeText);
                        lviPackage.SubItems.Add(package.Description);

                        if (package.IsReleased)
                        {
                            lviPackage.Group = this.packagesList.Groups[0];
                        }
                        else
                        {
                            lviPackage.Group = this.packagesList.Groups[1];
                        }

                        lviPackage.Tag = package.Version;
                        Invoke(new Action(() => this.packagesList.Items.Add(lviPackage)));
                    }
                    catch (IOException ex)
                    {
                        DialogResult dialogResult = DialogResult.None;
                        Invoke(new Action(() => dialogResult = Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.", String.Format("{0} - Should the entry for package {1} in the project be deleted in order to hide this error the next time?", this.GetNameOfExceptionType(ex), package.Version.ToString()), PopupButtons.YesNo)));

                        if (dialogResult == DialogResult.Yes)
                        {
                            // Remove the package info from the project file and stop the loading
                            this.Project.Packages.RemoveAll(item => item.Version == package.Version);

                            if (this.Project.ReleasedPackages != 0) // Set the released packages again with subtracting 1 for the project that was just removed
                            {
                                this.Project.ReleasedPackages -= 1;
                            }
                            
                            // The released packages
                            if (this.Project.Packages != null && this.Project.Packages.Count != 0)
                            {
                                this.Project.NewestPackage = Project.Packages.Last().Version.FullText;
                            }
                            else
                            {
                                this.Project.NewestPackage = null;
                            }

                            ApplicationInstance.SaveProject(Project.Path, Project);
                            InitializeProjectDetails();
                            InitializePackageItems(); //Re-call the methods
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.", ex, PopupButtons.OK)));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the name/description of the current exception.
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
                if (this.cancellationToken != null)
                {
                    this.cancellationToken.Dispose();
                    this.cancellationToken = new CancellationTokenSource();
                }

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                        { this.UploadPackage(); }), null);
            }
        }

        /// <summary>
        /// Provides a new thread that uploads the package.
        /// </summary>
        private void UploadPackage()
        {
            try
            {
                this.SetUIState(false);

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

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                string packageFileName = String.Empty;
                Invoke(new Action(() =>
                    {
                        packageFileName = String.Format("{0}.zip", this.projectIdTextBox.Text);
                    }));

                string updateConfigurationFilePath = Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString(), "updates.json");
                string packagePath = this.Project.Packages.Where(x => x.Version == this.packageVersion).First().LocalPackagePath;
                try
                {
                    this.ftp.UploadPackage(packagePath, this.packageVersion.ToString());
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.", ex, PopupButtons.OK)));
                    this.cancellationToken.Cancel();
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                while (!this.ftp.HasFinishedUploading)
                    continue;

                if (ftp.PackageUploadException != null)
                {
                    if (ftp.PackageUploadException.InnerException != null)
                    {
                        if (ftp.PackageUploadException.InnerException.GetType() == typeof(WebException))
                        {
                            switch (((int)((FtpWebResponse)((WebException)ftp.PackageUploadException).Response).StatusCode))
                            {
                                case 550:
                                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", String.Format("The server returned 550. Make sure the given FTP-directory exists and then try again. - {0}", ftpDirectoryTextBox.Text), PopupButtons.OK)));
                                    break;
                                case 530:
                                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", "The server login failed. Make sure the login credentials are correct and then try again.", PopupButtons.OK)));
                                    break;
                                default:
                                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ftp.PackageUploadException.InnerException, PopupButtons.OK)));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Exception ex = null;
                        if (this.ftp.PackageUploadException.InnerException != null)
                        {
                            ex = this.ftp.PackageUploadException.InnerException;
                        }
                        else
                        {
                            ex = this.ftp.PackageUploadException;
                        }

                        // Just handle it normally
                        Invoke(new Action(() =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex, PopupButtons.OK);
                            this.cancellationToken.Cancel();
                        }));
                    }
                    this.cancellationToken.Cancel();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    this.ftp.DeleteFile("updates.json"); // Configuration
                    this.ftp.UploadFile(updateConfigurationFilePath);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex, PopupButtons.OK)));
                    cancellationToken.Cancel();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Write to log
                updateLog.Write(LogEntry.Upload, packageVersion.ToString());

                Project.Packages.Where(x => x.Version == this.packageVersion).First().IsReleased = true;
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            finally
            {
                if (!this.cancellationToken.IsCancellationRequested)
                {
                    this.Project.Packages.Where(x => x.Version == this.packageVersion).First().IsReleased = true;
                    ApplicationInstance.SaveProject(this.Project.Path, this.Project);
                }

                this.SetUIState(true);
                this.InitializeProjectDetails();
                this.InitializePackageItems();
            }
        }

        /// <summary>
        /// Called when the progress changes.
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 100)
            {
                Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage * 2))));
            }
        }

        #endregion

        #region "Configuration"

        /// <summary>
        /// Starts checking if the update configuration exists.
        /// </summary>
        private void StartCheckingUpdateInfo()
        {
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.", String.Format("The configuration file could not be checked because no network connection is available. {0}", updateInfoFileUrl), PopupButtons.OK);
                return;
            }

            this.checkingUrlPictureBox.Visible = true;

            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
                this.cancellationToken = new CancellationTokenSource();
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                { this.CheckUpdateConfigurationStatus(this.updateInfoFileUrl); }), null); 
        }
        
        private void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StartCheckingUpdateInfo();
        }
        
        private void CheckUpdateConfigurationStatus(Uri infoFileUrl)
        {
            // Check if file exists on the server
            var request = HttpWebRequest.Create(infoFileUrl);
            request.Timeout = 5000;
            request.Method = "HEAD";

            // Cheeck for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };

            try
            {
                this.configResponse = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    this.SetUIState(false);
                    this.loadingPanel.Location = new Point(187, 145);
                    this.loadingPanel.BringToFront();
                    this.loadingLabel.Text = "Updating info file...";

                    this.checkUpdateConfigurationLinkLabel.Enabled = false; 
                    this.checkingUrlPictureBox.Visible = false;
                }));

                // Create file
                string temporaryNewUpdateInfoFile = Path.Combine(Program.Path, "updates.json");
                if (!File.Exists(temporaryNewUpdateInfoFile))
                {
                    using (FileStream newUpdateInfoFileStream = File.Create(temporaryNewUpdateInfoFile)) { }
                }

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
                    return;
                }

                try
                {
                    // Upload the file now
                    this.ftp.UploadFile(temporaryNewUpdateInfoFile);
                }
                catch (Exception ex)
                {
                    // Get the status code to handle it 
                    if (ex.GetType() == typeof(WebException))
                    {
                        switch (((int)((FtpWebResponse)((WebException)ex).Response).StatusCode))
                        {
                            case 550:
                                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", String.Format("The server returned 550. Make sure the given FTP-directory exists and then try again. - {0}", ftpDirectoryTextBox.Text), PopupButtons.OK)));
                                break;
                            case 530:
                                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", "The server login failed. Make sure the login credentials are correct and then try again.", PopupButtons.OK)));
                                break;
                            default:
                                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex, PopupButtons.OK)));
                                break;
                        }
                    }
                    else
                    {
                        // Just handle it normally
                        Invoke(new Action(() =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex, PopupButtons.OK);
                                this.cancellationToken.Cancel();
                            }));
                    }
                    // Cancel
                    this.cancellationToken.Cancel();
                }
            }
            finally
            {
                Invoke(new Action(() =>
                {
                    this.SetUIState(true);
                    this.loadingPanel.Visible = false;
                    this.checkingUrlPictureBox.Visible = false;
                    this.checkUpdateConfigurationLinkLabel.Enabled = true;

                    if (!this.cancellationToken.IsCancellationRequested)
                    {
                        this.tickPictureBox.Visible = true; // Upload succeeded
                    }
                }));
                this.allowCancel = true;

                if (this.configResponse != null)
                {
                    this.configResponse.Close();
                }
            }
        }

        #endregion

        #region "Deleting"

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (this.packagesList.FocusedItem != null)
            {
                DialogResult answer = Popup.ShowPopup(this, SystemIcons.Question, String.Format("Delete the update package {0}?", this.packageVersion), "Are you sure that you want to delete this package?", PopupButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    if (cancellationToken != null)
                    {
                        cancellationToken.Dispose();
                        cancellationToken = new CancellationTokenSource();
                    }

                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                        { DeletePackage(this.groupIndex); }), null);
                }
            }
        }

        bool shouldKeepErrorsSecret = false;
        /// <summary>
        /// Initializes a new thread for deleting the package.
        /// </summary>
        private void DeletePackage(int groupIndex)
        {
            try
            {
                this.SetUIState(false);
                if (int.Equals(this.groupIndex, 0)) // Must be deleted online, too.
                {
                    Invoke(new Action(() => this.loadingLabel.Text = "Waiting for credentials..."));
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
                        this.SetUIState(false);
                        return;
                    }

                    Invoke(new Action(() => this.loadingLabel.Text = "Deleting package..."));

                    try
                    {
                        // Delete package folder
                        ftp.DeleteDirectory(packageVersion.ToString());
                    }
                    catch (Exception ex)
                    {
                        // Get the status code to handle it 
                        if (ex.GetType() == typeof(WebException))
                        {
                            switch (((int)((FtpWebResponse)((WebException)ex).Response).StatusCode))
                            {
                                case 404:
                                    shouldKeepErrorsSecret = true;
                                    break;
                                case 450:
                                    shouldKeepErrorsSecret = true;
                                    break;
                                case 530:
                                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the package directory.", "The server login failed. Make sure the login credentials are correct and then try again.", PopupButtons.OK)));
                                    break;
                                default:
                                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the package directory.", ex, PopupButtons.OK)));
                                    break;
                            }
                        }
                        else
                        {
                            // Just handle it normally
                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the package directory.", ex, PopupButtons.OK)));
                        }

                        if (!shouldKeepErrorsSecret)
                        {
                            // Cancel
                            this.cancellationToken.Cancel();
                        }
                    }

                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.SetUIState(true);
                        return;
                    }

                    Invoke(new Action(() => this.loadingLabel.Text = "Getting old configuration..."));
                    
                    List<UpdateConfiguration> updateConfig = new List<UpdateConfiguration>();
                    try
                    {
                        updateConfig = UpdateConfiguration.DownloadUpdateConfiguration(this.updateInfoFileUrl, this.Project.Proxy);
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(() =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the old configuration.", ex, PopupButtons.OK);
                            }));
                    }

                    if (updateConfig != null && updateConfig.Count != 0)
                    {
                        bool isContainingCurrentVersion = false;
                        foreach (var configEntry in updateConfig)
                        {
                            if (new UpdateVersion(configEntry.Version) == this.packageVersion)
                            {
                                isContainingCurrentVersion = true;
                            }
                        }

                        string updateInfoFilePath = Path.Combine(Program.Path, "updates.json"); // The path to the temporary new update config

                        if (isContainingCurrentVersion)
                        {
                            updateConfig.Remove(updateConfig.Where(item => new UpdateVersion(item.Version) == this.packageVersion).First());
                            string content = Serializer.Serialize(updateConfig);

                            try
                            {
                                File.WriteAllText(updateInfoFilePath, content);
                            }
                            catch (Exception ex)
                            {
                                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while writing to local configuration file.", ex, PopupButtons.OK)));
                                this.cancellationToken.Cancel();
                            }

                            if (this.cancellationToken.IsCancellationRequested)
                            {
                                this.SetUIState(true);
                                return;
                            }
                        }

                        Invoke(new Action(() => this.loadingLabel.Text = "Uploading new configuration..."));

                        try
                        {
                            // Upload new configuration file
                            this.ftp.UploadFile(updateInfoFilePath);
                        }
                        catch (Exception ex)
                        {
                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading new configuration file.", ex, PopupButtons.OK)));
                            this.cancellationToken.Cancel();
                        }

                        if (this.cancellationToken.IsCancellationRequested)
                        {
                            this.SetUIState(true);
                            return;
                        }

                        File.WriteAllText(updateInfoFilePath, String.Empty);
                    }
                }

                Invoke(new Action(() => this.loadingLabel.Text = "Deleting local directory..."));

                try
                {
                    // Delete local folder
                    Directory.Delete(Path.Combine(Program.Path, "Projects", this.Project.Name, this.packageVersion.ToString()), true);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting local package directory.", ex, PopupButtons.OK)));
                }

                try
                {
                    // Remove current package entry and save the edited project
                    this.Project.Packages.RemoveAll(x => x.Version == packageVersion);
                    if (this.Project.ReleasedPackages != 0) // The amount of released packages
                    {
                        this.Project.ReleasedPackages -= 1;
                    }

                    // The newest package
                    if (this.Project.Packages != null && this.Project.Packages.Count != 0)
                    {
                        this.Project.NewestPackage = Project.Packages.Last().Version.FullText;
                    }
                    else
                    {
                        this.Project.NewestPackage = null;
                    }

                    ApplicationInstance.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex, PopupButtons.OK)));
                }
            }
            finally
            {
                this.updateLog.Write(LogEntry.Delete, this.packageVersion.ToString());
                ApplicationInstance.SaveProject(this.Project.Path, this.Project);

                this.SetUIState(true);
                Invoke(new Action(() =>
                {
                    this.packagesList.Items.Clear();
                    this.InitializePackageItems();
                    this.InitializeProjectDetails();
                }));
            }
        }
        #endregion

        private void browseAssemblyButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
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
                        Popup.ShowPopup(this, SystemIcons.Error, "Invalid assembly found.", "The version of the assembly for the selected executable (extension) file could not be read.", PopupButtons.OK);
                        this.loadFromAssemblyCheckBox.Checked = false;
                        return;
                    }

                    this.assemblyPathTextBox.Text = fileDialog.FileName;
                    this.Project.AssemblyVersionPath = this.assemblyPathTextBox.Text;
                    ApplicationInstance.SaveProject(this.Project.Path, this.Project);
                    this.InitializeProjectDetails();
                }
            }
        }

        private void loadFromAssemblyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.loadFromAssemblyCheckBox.Checked)
            {
                this.assemblyPathTextBox.Enabled = true;
                this.browseAssemblyButton.Enabled = true;
            }
            else
            {
                this.assemblyPathTextBox.Enabled = false;
                this.browseAssemblyButton.Enabled = false;

                if (this.isSetByUser)
                {
                    this.Project.AssemblyVersionPath = null;
                    ApplicationInstance.SaveProject(this.Project.Path, this.Project);
                    this.assemblyPathTextBox.Clear();
                    this.InitializeProjectDetails();
                }
            }
        }
    }
}
