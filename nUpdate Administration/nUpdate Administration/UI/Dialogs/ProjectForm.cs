using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectForm : BaseForm
    {
        private bool allowCancel = true;
        private string packageVersionString = null;
        private string packageDescription;
        private int groupIndex = 0;

        private const float Kb = 1024;
        private const float Mb = 1048577;

        private FtpManager ftp = new FtpManager();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public ProjectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName { get; set; }


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
            this.ProjectName = this.Project.Name;

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

            this.Text = String.Format("{0} - {1}", this.ProjectName, ls.ProductTitle);
            this.overviewHeader.Text = ls.ProjectFormOverviewText;
            this.overviewTabPage.Text = ls.ProjectFormOverviewTabText;
            this.packagesTabPage.Text = ls.ProjectFormPackagesTabText;
            this.nameLabel.Text = ls.ProjectFormNameLabelText;
            this.updateUrlLabel.Text = ls.ProjectFormUpdateUrlLabelText;
            this.ftpHostLabel.Text = ls.ProjectFormFtpHostLabelText;
            this.ftpDirectoryLabel.Text = ls.ProjectFormFtpDirectoryLabelText;
            this.releasedPackagesAmountLabel.Text = ls.ProjectFormPackagesAmountLabelText;
            this.newestPackageReleasedLabel.Text = ls.ProjectFormNewestPackageLabelText;
            this.infoFileloadingLabel.Text = ls.ProjectFormInfoFileloadingLabelText;
            this.checkUpdateInfoLinkLabel.Text = ls.ProjectFormCheckInfoFileStatusLinkLabelText;
            this.projectDataHeader.Text = ls.ProjectFormProjectDataText;
            this.publicKeyLabel.Text = ls.ProjectFormPublicKeyLabelText;
            this.projectIdLabel.Text = ls.ProjectFormProjectIdLabelText;
            this.stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

            this.addButton.Text = ls.ProjectFormAddButtonText;
            this.editButton.Text = ls.ProjectFormEditButtonText;
            this.deleteButton.Text = ls.ProjectFormDeleteButtonText;
            this.uploadButton.Text = ls.ProjectFormUploadButtonText;
            this.historyButton.Text = ls.ProjectFormHistoryButtonText;

            this.packagesList.Columns[0].Text = ls.ProjectFormVersionText;
            this.packagesList.Columns[1].Text = ls.ProjectFormReleasedText;
            this.packagesList.Columns[2].Text = ls.ProjectFormSizeText;
            this.packagesList.Columns[3].Text = ls.ProjectFormDescriptionText;

            this.searchTextBox.Cue = ls.ProjectFormSearchText;
        }

        #endregion

        private void ProjectForm_Load(object sender, EventArgs e)
        {
            this.ftp.ProgressChanged += ProgressChanged;
            this.InitializeProjectDetails();
            if (this.hasFailedOnLoading)
            {
                this.Close();
                return;
            }

            this.InitializePackageItems();
            if (this.hasFailedOnLoading)
            {
                this.Close();
                return;
            }

            this.InitializeFtpData();
            if (this.hasFailedOnLoading)
            {
                this.Close();
                return;
            }

            this.SetLanguage();

            if (!this.Project.UpdateUrl.EndsWith("/"))
            {
                this.Project.UpdateUrl += "/";
            }
            this.updateInfoFileUrl = UriConnecter.ConnectUri(this.Project.UpdateUrl, "updates.json");

            this.checkingUrlPictureBox.Location = new Point(this.checkUpdateInfoLinkLabel.Location.X + this.checkUpdateInfoLinkLabel.Size.Width + 25, this.checkingUrlPictureBox.Location.Y);
            this.tickPictureBox.Location = new Point(this.checkUpdateInfoLinkLabel.Location.X + this.checkUpdateInfoLinkLabel.Size.Width + 25, this.tickPictureBox.Location.Y);
            this.packagesList.DoubleBuffer();
            this.programmingLanguageComboBox.SelectedIndex = 0;
        }

        private void ProjectForm_Shown(object sender, EventArgs e)
        {
            this.packagesList.MakeCollapsable();
        }

        private void ProjectForm_FormClosing(object sender, FormClosingEventArgs e)
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
                ListViewItem matchingItem = this.packagesList.FindItemWithText(this.searchTextBox.Text);
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
            PackageAddForm packageAddForm = new PackageAddForm();
            var r = Enumerable.Empty<ListViewItem>();

            if (this.packagesList.Items.Count > 0)
            {
                r = this.packagesList.Items.OfType<ListViewItem>();
            }

            var last = r.LastOrDefault();
            if (last != null)
            {
                packageAddForm.NewestVersion = new Version(last.Text);
            }

            else
            {
                packageAddForm.NewestVersion = new Version("0.0.0.0");
            }

            packageAddForm.Project = this.Project;

            if (packageAddForm.ShowDialog() == DialogResult.OK)
            {
                this.Project = packageAddForm.Project;
                if (this.Project.HasUnsavedChanges)
                {
                    File.WriteAllText(this.Project.Path, Serializer.Serialize(this.Project));
                }

                this.packagesList.Items.Clear();
                this.InitializePackageItems();
                this.InitializeProjectDetails();
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            string updateUrl = this.updateUrlTextBox.Text;
            if (!updateUrl.EndsWith("/"))
            {
                updateUrl += "/";
            }

            string vbSource = String.Format("Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New Version(\"{2}\"))", UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text, newestPackageLabel.Text);
            string cSharpSource = String.Format("UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new Version(\"{2}\"));", UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text, newestPackageLabel.Text);

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
            //TODO: Edit packages-form
        }

        private void editFtpButton_Click(object sender, EventArgs e)
        {
            var result = ShowDialog<FtpEditForm>(this, this.Project);
            if (result.DialogResult == DialogResult.OK)
            {
                if (result.UpdateProject.HasUnsavedChanges)
                {
                    string projectFilePath = this.Project.Path;
                    this.Project = result.UpdateProject;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, this.Project);
                        File.Move(projectFilePath, this.Project.Path);
                        this.InitializeFtpData();
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.OK);
                    }
                }
            }
        }

        private void editProjectButton_Click(object sender, EventArgs e)
        {
            var result = ShowDialog<ProjectEditForm>(this, Project);
            if (result.DialogResult == DialogResult.OK)
            {
                if (result.UpdateProject.HasUnsavedChanges)
                {
                    string projectFilePath = this.Project.Path;
                    this.Project = result.UpdateProject;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, this.Project);
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
            ShowDialog<HistoryForm>(this, this.Project);
        }

        private void packagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.packagesList.FocusedItem != null)
            {
                this.packageVersionString = this.packagesList.FocusedItem.Text;
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

        /// <summary>
        /// Sets all the details for the project.
        /// </summary>
        private void InitializeProjectDetails()
        {
            try
            {
                this.nameTextBox.Text = this.Project.Name;
                this.updateUrlTextBox.Text = this.Project.UpdateUrl;
                this.ftpHostTextBox.Text = this.Project.FtpHost;
                this.ftpDirectoryTextBox.Text = this.Project.FtpDirectory;
                this.amountLabel.Text = this.Project.ReleasedPackages;

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
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK);
                this.hasFailedOnLoading = true;
            }
        }

        /// <summary>
        /// Adds the items to the listview.
        /// </summary>
        private void InitializePackageItems()
        {
            if (this.Project.Packages != null)
            {
                try
                {
                    foreach (UpdatePackage package in this.Project.Packages)
                    {
                        var lviPackage = new ListViewItem(package.Version);
                        var diPackage = new DirectoryInfo(package.LocalPackagePath);
                        lviPackage.SubItems.Add(diPackage.CreationTime.ToString());

                        // Get the size of the package
                        var fiPackageFile = new FileInfo(Path.Combine(package.LocalPackagePath, String.Format("{0}.zip", this.projectIdTextBox.Text)));
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

                        bool isReleased;
                        if (!bool.TryParse(package.IsReleased, out isReleased)) ;
                        {
                            // Damaged
                        }

                        if (isReleased)
                        {
                            lviPackage.Group = this.packagesList.Groups[0];
                        }
                        else
                        {
                            lviPackage.Group = this.packagesList.Groups[1];
                        }

                        this.packagesList.Items.Add(lviPackage);
                    }
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading packages.", ex, PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading packages.", ex, PopupButtons.OK);
                    this.hasFailedOnLoading = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Hides the elements showing a progress and re-enables all controls.
        /// </summary>
        private void PerformUICleanUp()
        {
            Invoke(new Action(() =>
            {
                foreach (Control control in this.tabControl1.Controls)
                {
                    control.Enabled = true;
                }

                this.loadingPanel.Visible = false;

                this.packagesList.Items.Clear();
                this.InitializePackageItems();
                this.InitializeProjectDetails();
            }));

            allowCancel = true;
        }

        /// <summary>
        /// Terminates the upload/deletion processes when an error appears.
        /// </summary>
        private void TerminateProcesses()
        {
            this.PerformUICleanUp();
            this.cancellationToken.Cancel();
        }

        #region "Package upload"

        private void uploadButton_Click(object sender, EventArgs e)
        {
            new Thread(InitializePackage).Start();
        }

        /// <summary>
        /// Provides a new thread that initializes the package.
        /// </summary>
        private void InitializePackage()
        {
            Invoke(new Action(() =>
            {
                foreach (Control control in this.Controls)
                {
                    if (control.Visible == true)
                    {
                        control.Enabled = false;
                    }
                }

                this.loadingPanel.Location = new Point(179, 135);
                this.loadingPanel.Visible = true;
            }));

            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
                this.cancellationToken = new CancellationTokenSource();
            }

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
                    else
                    {
                        this.TerminateProcesses();
                    }
                }));
            }

            string packageFileName = String.Empty;
            Invoke(new Action(() =>
                {
                    packageFileName = String.Format("{0}.zip", this.projectIdTextBox.Text);
                }));

            string packagePath = Path.Combine(Project.Packages.Where(x => x.Version == this.packageVersionString).First().LocalPackagePath, packageFileName);
            this.ftp.UploadPackage(packagePath, this.packageVersionString);

            this.PerformUICleanUp();

            // Write to log
            //Log log = new Log(ProjectName);
            //log.Write(String.Format("{0}-U-{1}", DateTime.Now, packageVersionString));
        }

        #endregion

        #region "Info file"

        private Uri updateInfoFileUrl;
        private void checkUpdateInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.allowCancel = false;
            this.checkingUrlPictureBox.Visible = true;

            var task =
            Task.Factory.StartNew(() => this.CheckInfoFileStatus(this.updateInfoFileUrl)).ContinueWith(this.CheckingInfoFileUrlFailed,
                    cancellationToken.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => this.CheckingInfoFileUrFinished(),
                            cancellationToken.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private HttpWebResponse response = null;
        private void CheckInfoFileStatus(Uri infoFileUrl)
        {
            if (this.cancellationToken != null)
            {
                this.cancellationToken.Dispose();
                this.cancellationToken = new CancellationTokenSource();
            }

            // Check if file exists on the server
            var request = HttpWebRequest.Create(infoFileUrl);
            request.Method = "HEAD";

            // Cheeck for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };

            try
            {
                this.response = (HttpWebResponse)request.GetResponse();

                this.PerformUICleanUp();
                Invoke(new Action(() =>
                {
                    this.tickPictureBox.Visible = true;
                    this.checkingUrlPictureBox.Visible = false;
                }));
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    this.checkingUrlPictureBox.Visible = false;
                    foreach (Control control in tabControl1.Controls)
                    {
                        control.Enabled = false;
                    }

                    this.loadingPanel.Location = new Point(187, 145);
                    this.loadingPanel.Visible = true;
                    this.loadingLabel.Text = "Updating info file...";
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
                        var credentialsForm = new CredentialsForm();
                        if (credentialsForm.ShowDialog() == DialogResult.OK)
                        {
                            this.ftp.FtpUserName = credentialsForm.Username;
                            this.ftp.FtpPassword = credentialsForm.Password;
                        }
                        else
                        {
                            this.TerminateProcesses();
                        }
                    }));
                }

                if (this.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Upload the file now
                this.ftp.UploadFile(temporaryNewUpdateInfoFile);

                while (!this.ftp.HasFinishedUploading)
                {
                    continue;
                }

                this.PerformUICleanUp();
                Invoke(new Action(() =>
                {
                    this.tickPictureBox.Visible = true;
                    this.checkingUrlPictureBox.Visible = false;
                }));
            }

            finally
            {
                if (this.response != null)
                {
                    this.response.Close();
                }
            }
        }

        private void CheckingInfoFileUrlFailed(Task task)
        {
            Invoke(new Action(() =>
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new info file.", task.Exception.InnerException, PopupButtons.OK);

                foreach (Control control in this.Controls)
                {
                    if (control.GetType() != typeof(Panel))
                    {
                        control.Enabled = true;
                    }
                }
                this.loadingPanel.Visible = false;
                this.checkingUrlPictureBox.Visible = false;
            }));
        }

        private void CheckingInfoFileUrFinished()
        {
            // TODO: Implement
        }

        #endregion

        #region "Deleting"

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (this.packagesList.FocusedItem != null)
            {
                DialogResult answer = Popup.ShowPopup(this, SystemIcons.Question, "Delete update package?", "Are you sure that you want to delete this package?", PopupButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    Thread thread = new Thread(delegate() { InitializeDeleting(this.groupIndex); });
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Initializes a new thread for deleting the package.
        /// </summary>
        private void InitializeDeleting(int groupIndex)
        {
            this.allowCancel = false;
            if (int.Equals(this.groupIndex, 0)) // Must be deleted online, too.
            {
                Invoke(new Action(() =>
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Visible == true)
                            {
                                control.Enabled = false;
                            }
                        }

                        this.loadingPanel.Location = new Point(179, 135);
                        this.loadingPanel.Visible = true;
                        this.loadingLabel.Text = "Getting configuration...";
                    }));

                UpdateConfiguration config = new UpdateConfiguration();
                List<UpdateConfiguration> updateConfig = config.LoadUpdateConfiguration(this.updateInfoFileUrl);
                updateConfig.Remove(updateConfig.Where(item => item.UpdateVersion == this.packageVersionString).First());

                string updateInfoFilePath = Path.Combine(Program.Path, "updates.json");
                try
                {
                    string content = Serializer.Serialize(updateConfig);
                    File.WriteAllText(updateInfoFilePath, content);
                }
                catch (Exception ex)
                {
                    // Damaged
                }

                // Upload file
                this.ftp.UploadFile(updateInfoFilePath);

                while (!this.ftp.HasFinishedUploading)
                {
                    continue;
                }

                File.WriteAllText(updateInfoFilePath, String.Empty);
            }
            this.PerformUICleanUp();
        }

        #endregion

        #region "FTP-handler"

        /// <summary>
        /// Called when the upload fails.
        /// </summary>
        private void UploadFailed(Exception ex)
        {
            Invoke(new Action(() =>
                {
                    if (ex.GetType() == typeof(WebException) && (ex as WebException).Status != WebExceptionStatus.ProtocolError)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", String.Format("{0} - {1}", ex.Message, this.ftp.ServerAdress), PopupButtons.OK);
                    }
                    else if (ex.GetType() == typeof(WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", ex, PopupButtons.OK);
                    }
                    else
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", String.Format("{0} - {1}", ex.Message, this.ftp.ServerAdress), PopupButtons.OK);
                    }

                    this.tickPictureBox.Visible = false;
                    this.checkingUrlPictureBox.Visible = false;
                }));
            PerformUICleanUp();
        }

        /// <summary>
        /// Called when the deletion fails.
        /// </summary>
        private void DeleteFailed(Exception ex)
        {
            Invoke(new Action(() =>
                {
                    if (ex.GetType() == typeof(WebException) && (ex as WebException).Status != WebExceptionStatus.ProtocolError)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", String.Format("{0} - {1}", ex.Message, this.ftp.ServerAdress), PopupButtons.OK);
                    }
                    else if (ex.GetType() == typeof(WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError)
                    {
                        int statusCode = (int)((FtpWebResponse)((ex as WebException).Response)).StatusCode;
                        if (statusCode != 404 && statusCode != 450)
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", ex, PopupButtons.OK);
                        }
                    }
                    else
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", ex, PopupButtons.OK);
                    }

                    this.tickPictureBox.Visible = false;
                    this.checkingUrlPictureBox.Visible = false;
                }));

        }

        /// <summary>
        /// Called when the progress changes.
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() => this.loadingLabel.Text = String.Format(this.uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage))));
        }

        #endregion

    }
}
