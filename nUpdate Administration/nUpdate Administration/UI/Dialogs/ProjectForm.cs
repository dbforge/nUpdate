using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.UI.Popups;
using System.Net;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using nUpdate.Administration.Core.Update;
using System.Security;
using nUpdate.Administration.Core.Application;
using System.Reflection;
using System.Threading.Tasks;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectForm : BaseForm
    {
        bool allowCancel = true;
        string packageVersionString = null;
        string packageDescription;
        int groupIndex = 0;

        private const float KB = 1024;
        private const float MB = 1048577;

        private FtpManager ftp = new FtpManager();
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public ProjectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName
        {
            get;
            set;
        }


        #region "Localization"

        string invalidArgumentCaption = "Invalid argument found.";
        string invalidArgumentText = "The entry {0} can't be parsed to {1}.";
        string gettingUrlErrorCaption = "Error while getting url.";
        string readingPackageBytesErrorCaption = "Reading package bytes failed.";
        string invalidServerDirectoryErrorCaption = "Invalid server directory.";
        string invalidServerDirectoryErrorText = "The directory for the update files on the server is not valid. Please edit it.";
        string ftpDataLoadErrorCaption = "Failed to load FTP-data.";
        string relativeUriErrorText = "The server-directory can't be set as a relative uri.";
        string savingInformationErrorCaption = "Saving package information failed.";
        string uploadFailedErrorCaption = "Upload failed.";

        string uploadingPackageInfoText = "Uploading package - {0}";
        string uploadingConfigInfoText = "Uploading configuration...";

        /// <summary>
        /// Sets the language.
        /// </summary>
        public void SetLanguage()
        {
            ProjectName = Project.Name;

            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            else {

                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = String.Format("{0} - {1}", ProjectName, ls.ProductTitle);
            overviewHeader.Text = ls.ProjectFormOverviewText;
            overviewTabPage.Text = ls.ProjectFormOverviewTabText;
            packagesTabPage.Text = ls.ProjectFormPackagesTabText;
            nameLabel.Text = ls.ProjectFormNameLabelText;
            updateUrlLabel.Text = ls.ProjectFormUpdateUrlLabelText;
            ftpHostLabel.Text = ls.ProjectFormFtpHostLabelText;
            ftpDirectoryLabel.Text = ls.ProjectFormFtpDirectoryLabelText;
            releasedPackagesAmountLabel.Text = ls.ProjectFormPackagesAmountLabelText;
            newestPackageReleasedLabel.Text = ls.ProjectFormNewestPackageLabelText;
            infoFileloadingLabel.Text = ls.ProjectFormInfoFileloadingLabelText;
            checkUpdateInfoLinkLabel.Text = ls.ProjectFormCheckInfoFileStatusLinkLabelText;
            projectDataHeader.Text = ls.ProjectFormProjectDataText;
            publicKeyLabel.Text = ls.ProjectFormPublicKeyLabelText;
            projectIdLabel.Text = ls.ProjectFormProjectIdLabelText;
            stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

            addButton.Text = ls.ProjectFormAddButtonText;
            editButton.Text = ls.ProjectFormEditButtonText;
            deleteButton.Text = ls.ProjectFormDeleteButtonText;
            uploadButton.Text = ls.ProjectFormUploadButtonText;
            historyButton.Text = ls.ProjectFormHistoryButtonText;

            packagesList.Columns[0].Text = ls.ProjectFormVersionText;
            packagesList.Columns[1].Text = ls.ProjectFormReleasedText;
            packagesList.Columns[2].Text = ls.ProjectFormSizeText;
            packagesList.Columns[3].Text = ls.ProjectFormDescriptionText;

            searchTextBox.Cue = ls.ProjectFormSearchText;
        }

        #endregion

        private void ProjectForm_Load(object sender, EventArgs e)
        {
            ftp.ProgressChanged += ProgressChanged;
            InitializeProjectDetails();
            if (hasFailedOnLoading) {
                Close();
                return;
            }

            InitializePackageItems();
            if (hasFailedOnLoading)
            {
                Close();
                return;
            }

            InitializeFtpData();
            if (hasFailedOnLoading)
            {
                Close();
                return;
            }

            SetLanguage();

            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            updateInfoFileUrl = UriConnecter.ConnectUri(Project.UpdateUrl, "updates.json");

            checkingUrlPictureBox.Location = new Point(checkUpdateInfoLinkLabel.Location.X + checkUpdateInfoLinkLabel.Size.Width + 25, checkingUrlPictureBox.Location.Y);
            tickPictureBox.Location = new Point(checkUpdateInfoLinkLabel.Location.X + checkUpdateInfoLinkLabel.Size.Width + 25, tickPictureBox.Location.Y);
            packagesList.DoubleBuffer();
            programmingLanguageComboBox.SelectedIndex = 0;
        }

        private void ProjectForm_Shown(object sender, EventArgs e)
        {
            packagesList.MakeCollapsable();
        }

        private void ProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ListViewItem matchingItem = packagesList.FindItemWithText(searchTextBox.Text);
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
            PackageAddForm packageAddForm = new PackageAddForm();
            var r = Enumerable.Empty<ListViewItem>();
            
            if (packagesList.Items.Count > 0)
                r = this.packagesList.Items.OfType<ListViewItem>();

            var last = r.LastOrDefault();
            if (last != null)
                packageAddForm.NewestVersion = new Version(last.Text);

            else
                packageAddForm.NewestVersion = new Version("0.0.0.0");

            packageAddForm.Project = this.Project;

            if (packageAddForm.ShowDialog() == DialogResult.OK)
            {
                this.Project = packageAddForm.Project;
                if (Project.HasUnsavedChanges)
                    File.WriteAllText(Project.Path, Serializer.Serialize(Project));

                packagesList.Items.Clear();
                InitializePackageItems();
                InitializeProjectDetails();
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            string updateUrl = updateUrlTextBox.Text;
            if (!updateUrl.EndsWith("/"))
                updateUrl += "/";

            string vbSource = String.Format("Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New Version(\"{2}\"))",
                UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text,
                newestPackageLabel.Text);
            string cSharpSource = String.Format("UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new Version(\"{2}\"));",
                UriConnecter.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text,
                newestPackageLabel.Text);

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
            //TODO: Edit packages-form
        }

        private void editFtpButton_Click(object sender, EventArgs e)
        {
            var result = ShowDialog<FtpEditForm>(this, Project);
            if (result.DialogResult == DialogResult.OK)
            {
                if (result.UpdateProject.HasUnsavedChanges)
                {
                    string projectFilePath = Project.Path;
                    Project = result.UpdateProject;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, Project);
                        File.Move(projectFilePath, Project.Path);
                        InitializeFtpData();
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
                    string projectFilePath = Project.Path;
                    Project = result.UpdateProject;

                    try
                    {
                        ApplicationInstance.SaveProject(projectFilePath, Project);
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
            ShowDialog<HistoryForm>(this, Project);
        }

        private void packagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packagesList.FocusedItem != null)
            {
                packageVersionString = packagesList.FocusedItem.Text;
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

        #region "Initializing"

        bool hasFailedOnLoading = false;

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

        /// <summary>
        /// Sets all the details for the project.
        /// </summary>
        private void InitializeProjectDetails()
        {
            try
            {
                nameTextBox.Text = Project.Name;
                updateUrlTextBox.Text = Project.UpdateUrl;
                ftpHostTextBox.Text = Project.FtpHost;
                ftpDirectoryTextBox.Text = Project.FtpDirectory;
                amountLabel.Text = Project.ReleasedPackages;

                if (Project.NewestPackage != null)
                    newestPackageLabel.Text = Project.NewestPackage;
                else
                    newestPackageLabel.Text = "-";

                projectIdTextBox.Text = Project.Id;
                publicKeyTextBox.Text = Project.PublicKey;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", "The project file is corrupt and does not have the necessary arguments.", PopupButtons.OK);
                hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex, PopupButtons.OK);
                hasFailedOnLoading = true;
            }
        }

        /// <summary>
        /// Adds the items to the listview.
        /// </summary>
        private void InitializePackageItems()
        {
            if (Project.Packages != null)
            {
                try
                {
                    foreach (UpdatePackage package in Project.Packages)
                    {
                        var lviPackage = new ListViewItem(package.Version);
                        var diPackage = new DirectoryInfo(package.LocalPackagePath);
                        lviPackage.SubItems.Add(diPackage.CreationTime.ToString());

                        // Get the size of the package
                        var fiPackageFile = new FileInfo(Path.Combine(package.LocalPackagePath, String.Format("{0}.zip", projectIdTextBox.Text)));
                        long sizeInBytes = fiPackageFile.Length;
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

                        lviPackage.SubItems.Add(sizeText);
                        lviPackage.SubItems.Add(package.Description);

                        bool isReleased;
                        if (!bool.TryParse(package.IsReleased, out isReleased)) ;
                        {
                            // Damaged
                        }

                        if (isReleased)
                            lviPackage.Group = packagesList.Groups[0];
                        else
                            lviPackage.Group = packagesList.Groups[1];

                        packagesList.Items.Add(lviPackage);
                    }
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading packages.", ex, PopupButtons.OK);
                    hasFailedOnLoading = true;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading packages.", ex, PopupButtons.OK);
                    hasFailedOnLoading = true;
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
                foreach (Control control in tabControl1.Controls)
                    control.Enabled = true;

                loadingPanel.Visible = false;

                packagesList.Items.Clear();
                InitializePackageItems();
                InitializeProjectDetails();
            }));

            allowCancel = true;
        }

        /// <summary>
        /// Terminates the upload/deletion processes when an error appears.
        /// </summary>
        private void TerminateProcesses()
        {
            PerformUICleanUp();
            cancellationToken.Cancel();
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
                        control.Enabled = false;
                }

                loadingPanel.Location = new Point(179, 135);
                loadingPanel.Visible = true;
            }));

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = new CancellationTokenSource();
            }

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
                    else
                        TerminateProcesses();
                }));
            }
            
            string packageFileName = String.Empty;
            Invoke(new Action(() =>
                {
                    packageFileName = String.Format("{0}.zip", projectIdTextBox.Text);
                }));

            string packagePath = Path.Combine(Project.Packages.Where(x => x.Version == packageVersionString)
                .First().LocalPackagePath, packageFileName);
            ftp.UploadPackage(packagePath, packageVersionString);

            PerformUICleanUp();

            // Write to log
            //Log log = new Log(ProjectName);
            //log.Write(String.Format("{0}-U-{1}", DateTime.Now, packageVersionString));
        }

        #endregion

        #region "Info file"

        Uri updateInfoFileUrl;
        private void checkUpdateInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            allowCancel = false;
            checkingUrlPictureBox.Visible = true;

            var task =
            Task.Factory.StartNew(() => this.CheckInfoFileStatus(updateInfoFileUrl)).ContinueWith(this.CheckingInfoFileUrlFailed,
                    cancellationToken.Token,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith(o => this.CheckingInfoFileUrFinished(),
                            cancellationToken.Token,
                            TaskContinuationOptions.NotOnFaulted,
                            TaskScheduler.FromCurrentSynchronizationContext());
        }

        HttpWebResponse response = null;
        private void CheckInfoFileStatus(Uri infoFileUrl)
        {
            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = new CancellationTokenSource();
            }
            
            // Check if file exists on the server
            var request = HttpWebRequest.Create(infoFileUrl);
            request.Method = "HEAD";

            // Cheeck for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                PerformUICleanUp();
                Invoke(new Action(() =>
                {
                    tickPictureBox.Visible = true;
                    checkingUrlPictureBox.Visible = false;
                }));
            }
            catch
            {
                Invoke(new Action(() =>
                {
                    checkingUrlPictureBox.Visible = false;
                    foreach (Control control in tabControl1.Controls)
                        control.Enabled = false;

                    loadingPanel.Location = new Point(187, 145);
                    loadingPanel.Visible = true;
                    loadingLabel.Text = "Updating info file...";
                }));

                // Create file
                string temporaryNewUpdateInfoFile = Path.Combine(Program.Path, "updates.json");
                if (!File.Exists(temporaryNewUpdateInfoFile))
                {
                    using ( FileStream newUpdateInfoFileStream = File.Create(temporaryNewUpdateInfoFile)) { }
                }

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
                        else
                            TerminateProcesses();
                    }));
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                // Upload the file now
                ftp.UploadFile(temporaryNewUpdateInfoFile);

                while (!ftp.HasFinishedUploading)
                    continue;

                PerformUICleanUp();
                Invoke(new Action(() =>
                {
                    tickPictureBox.Visible = true;
                    checkingUrlPictureBox.Visible = false;
                }));
            }

            finally
            {
                if (response != null)
                    response.Close();
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
                        control.Enabled = true;
                }
                loadingPanel.Visible = false;
                checkingUrlPictureBox.Visible = false;
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
            if (packagesList.FocusedItem != null)
            {
                DialogResult answer = Popup.ShowPopup(this, SystemIcons.Question, "Delete update package?", "Are you sure that you want to delete this package?", PopupButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    Thread thread = new Thread(delegate() { InitializeDeleting(groupIndex); });
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Initializes a new thread for deleting the package.
        /// </summary>
        private void InitializeDeleting(int groupIndex)
        {
            allowCancel = false;
            if (int.Equals(groupIndex, 0)) // Must be deleted online, too.
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

                        loadingPanel.Location = new Point(179, 135);
                        loadingPanel.Visible = true;
                        loadingLabel.Text = "Getting configuration...";
                    }));

                UpdateConfiguration config = new UpdateConfiguration();
                List<UpdateConfiguration> updateConfig = config.LoadUpdateConfiguration(updateInfoFileUrl);
                updateConfig.Remove(updateConfig.Where(item => item.UpdateVersion == packageVersionString).First());

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
                ftp.UploadFile(updateInfoFilePath);

                while (!ftp.HasFinishedUploading)
                    continue;

                File.WriteAllText(updateInfoFilePath, String.Empty);
            }
            PerformUICleanUp();
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
                            Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", String.Format("{0} - {1}", ex.Message, ftp.ServerAdress), PopupButtons.OK);
                    else if (ex.GetType() == typeof(WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError)
                        Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", ex, PopupButtons.OK);
                    else
                        Popup.ShowPopup(this, SystemIcons.Error, "Upload failed.", String.Format("{0} - {1}", ex.Message, ftp.ServerAdress), PopupButtons.OK);

                    tickPictureBox.Visible = false;
                    checkingUrlPictureBox.Visible = false;
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
                        Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", String.Format("{0} - {1}", ex.Message, ftp.ServerAdress), PopupButtons.OK);
                   
                    else if (ex.GetType() == typeof(WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError)
                    {
                        int statusCode = (int)((FtpWebResponse)((ex as WebException).Response)).StatusCode;
                        if (statusCode != 404 && statusCode != 450)
                            Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", ex, PopupButtons.OK);
                    }

                    else
                        Popup.ShowPopup(this, SystemIcons.Error, "Deleting failed.", ex, PopupButtons.OK);

                    tickPictureBox.Visible = false;
                    checkingUrlPictureBox.Visible = false;
                }));
            
        }

        /// <summary>
        /// Called when the progress changes.
        /// </summary>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() => loadingLabel.Text = String.Format(uploadingPackageInfoText, String.Format("{0}%", e.ProgressPercentage))));
        }

        #endregion
 
    }
}
