// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private readonly FtpManager _ftp = new FtpManager();
        private bool _allowCancel = true;
        private bool _generalTabPassed;

        //private LocalizationProperties _lp = new LocalizationProperties();
        private bool _phpFileCreated;
        private bool _phpFileUploaded;
        private bool _projectFileCreated;
        private bool _projectConfigurationEdited;
        private TabPage _sender;

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Returns the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     Returns the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     The url of the SQL-connection.
        /// </summary>
        public string SqlWebUrl { get; set; }

        /// <summary>
        ///     The name of the SQL-database to use.
        /// </summary>
        public string SqlDatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-login.
        /// </summary>
        public string SqlUsername { get; set; }

        ///// <summary>
        /////     Sets the language
        ///// </summary>
        //public void SetLanguage()
        //{
        //    string languageFilePath = Path.Combine(Program.LanguagesDirectory,
        //        String.Format("{0}.json", Settings.Default.Language.Name));
        //    if (File.Exists(languageFilePath))
        //        _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
        //    else
        //    {
        //        string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
        //        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        //        {
        //            _lp = Serializer.Deserialize<LocalizationProperties>(stream);
        //        }
        //    }

        //    Text = _lp.NewProjectDialogTitle;
        //    Text = String.Format(Text, _lp.ProductTitle);

        //    cancelButton.Text = _lp.CancelButtonText;
        //    continueButton.Text = _lp.ContinueButtonText;

        //    keyPairHeaderLabel.Text = _lp.PanelSignatureHeader;
        //    keyPairInfoLabel.Text = _lp.PanelSignatureInfoText;
        //    keyPairGenerationLabel.Text = _lp.PanelSignatureWaitText;

        //    generalHeaderLabel.Text = _lp.PanelGeneralHeader;
        //    nameLabel.Text = _lp.PanelGeneralNameText;
        //    nameTextBox.Cue = _lp.PanelGeneralNameWatermarkText;

        //    ftpHeaderLabel.Text = _lp.PanelFtpHeader;
        //    ftpHostLabel.Text = _lp.PanelFtpServerText;
        //    ftpUserLabel.Text = _lp.PanelFtpUserText;
        //    ftpUserTextBox.Cue = _lp.PanelFtpUserWatermarkText;
        //    ftpPasswordLabel.Text = _lp.PanelFtpPasswordText;
        //    ftpPortLabel.Text = _lp.PanelFtpPortText;
        //    ftpPortTextBox.Cue = _lp.PanelFtpPortWatermarkText;
        //}

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            ftpPortTextBox.ShortcutsEnabled = false;
            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;

            //SetLanguage();

            controlPanel1.Visible = false;
            ThreadPool.QueueUserWorkItem(arg => GenerateKeyPair());
        }

        private void NewProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        ///     Provides a new thread that generates a new RSA-key pair.
        /// </summary>
        private void GenerateKeyPair()
        {
            var rsa = new RsaSignature();
            PrivateKey = rsa.PrivateKey;
            PublicKey = rsa.PublicKey;

            Invoke(new Action(() =>
            {
                controlPanel1.Visible = true;
                informationCategoriesTabControl.SelectedTab = generalTabPage;
                _sender = generalTabPage;
            }));

            _allowCancel = true;
        }

        public void SetUiState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                if (!enabled)
                {
                    _allowCancel = false;
                    loadingPanel.Visible = true;
                    loadingPanel.Location = new Point(144, 72);
                    loadingPanel.BringToFront();
                }
                else
                {
                    _allowCancel = true;
                    loadingPanel.Visible = false;
                }
            }));
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (_sender == generalTabPage)
            {
                if (!ValidationManager.ValidatePanel(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (nameTextBox.Text.Contains("%") || localPathTextBox.Text.Contains("%"))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Illegal character specified.",
                        "The name and/or local path mustn't contain the char \"%\".", PopupButtons.Ok);
                    return;
                }

                if (!_generalTabPassed)
                {
                    if (Program.ExisitingProjects.Any(item => item.Key == nameTextBox.Text))
                    {
                        var relatingProject =
                            Program.ExisitingProjects.First(item => item.Key == nameTextBox.Text);
                        if (File.Exists(relatingProject.Value))
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                                String.Format(
                                    "The project \"{0}\" is already existing. Please choose another name for it.",
                                    nameTextBox.Text), PopupButtons.Ok);
                            return;
                        }

                        Program.ExisitingProjects.Remove(relatingProject.Key);
                        try
                        {
                            var projectEntries =
                                Program.ExisitingProjects.Select(
                                    projectEntry => String.Format("{0}%{1}", projectEntry.Key, projectEntry.Value))
                                    .ToList();

                            File.WriteAllText(Program.ProjectsConfigFilePath, String.Join("\n", projectEntries));
                        }
                        catch (Exception ex)
                        {
                            Popup.ShowPopup(this, SystemIcons.Error,
                                "Error while editing the project confiuration file. Please choose another name for the project.",
                                ex,
                                PopupButtons.Ok);
                            return;
                        }
                    }
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid adress.", "The given Update-URL is invalid.",
                        PopupButtons.Ok);
                    return;
                }

                if (!Path.IsPathRooted(localPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }

                try
                {
                    Path.GetFullPath(localPathTextBox.Text);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }

                _sender = ftpTabPage;
                backButton.Enabled = true;
                informationCategoriesTabControl.SelectedTab = ftpTabPage;
            }
            else if (_sender == ftpTabPage)
            {
                if (!ValidationManager.ValidatePanel(ftpPanel) || String.IsNullOrEmpty(ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                _ftp.Host = ftpHostTextBox.Text;
                _ftp.Port = int.Parse(ftpPortTextBox.Text);
                _ftp.Username = ftpUserTextBox.Text;
                _ftp.Directory = ftpDirectoryTextBox.Text;

                var ftpPassword = new SecureString();
                foreach (var c in ftpPasswordTextBox.Text)
                {
                    ftpPassword.AppendChar(c);
                }
                _ftp.Password = ftpPassword; // Same instance that FtpManager will automatically dispose

                _ftp.UsePassiveMode = ftpModeComboBox.SelectedIndex == 0;
                _ftp.Protocol = (FtpSecurityProtocol) ftpProtocolComboBox.SelectedIndex;

                if (!backButton.Enabled) // If the back-button was disabled, enabled it again
                    backButton.Enabled = true;

                _sender = statisticsServerTabPage;
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (_sender == statisticsServerTabPage)
            {
                if (useStatisticsServerRadioButton.Checked)
                {
                    if (!ValidationManager.ValidatePanel(statisticsServerTabPage))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                        return;
                    }
                }

                _sender = proxyTabPage;
                informationCategoriesTabControl.SelectedTab = proxyTabPage;
            }
            else if (_sender == proxyTabPage)
            {
                if (useProxyRadioButton.Checked)
                {
                    if (!ValidationManager.ValidatePanel(proxyTabPage) && !String.IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !String.IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                        return;
                    }
                }

                try
                {
                    using (File.Create(localPathTextBox.Text))
                    {
                    }
                    _projectFileCreated = true;
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.Ok);
                    Close();
                }

                var usePassive = ftpModeComboBox.SelectedIndex == 0;

                WebProxy proxy = null;
                string proxyUsername = null;
                string proxyPassword = null;
                if (!String.IsNullOrEmpty(proxyHostTextBox.Text))
                {
                    proxy = new WebProxy(proxyHostTextBox.Text);
                    if (!String.IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !String.IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        proxyUsername = proxyUserTextBox.Text;
                        proxyPassword =
                            Convert.ToBase64String(AesManager.Encrypt(proxyPasswordTextBox.Text,
                                ftpPasswordTextBox.Text,
                                ftpUserTextBox.Text));
                    }
                }

                string sqlPassword = null;
                if (useStatisticsServerRadioButton.Checked)
                {
                    sqlPassword =
                        Convert.ToBase64String(AesManager.Encrypt(sqlPasswordTextBox.Text, ftpPasswordTextBox.Text,
                            ftpUserTextBox.Text));
                }

                Settings.Default.ApplicationID += 1;
                Settings.Default.Save();
                Settings.Default.Reload();

                // Create a new package...
                var project = new UpdateProject
                {
                    Path = localPathTextBox.Text,
                    Name = nameTextBox.Text,
                    Guid = Guid.NewGuid().ToString(),
                    ApplicationId = Settings.Default.ApplicationID,
                    UpdateUrl = updateUrlTextBox.Text,
                    NewestPackage = null,
                    Packages = null,
                    ReleasedPackages = 0,
                    FtpHost = ftpHostTextBox.Text,
                    FtpPort = int.Parse(ftpPortTextBox.Text),
                    FtpUsername = ftpUserTextBox.Text,
                    FtpPassword =
                        Convert.ToBase64String(AesManager.Encrypt(ftpPasswordTextBox.Text, ftpPasswordTextBox.Text,
                            ftpUserTextBox.Text)),
                    FtpDirectory = ftpDirectoryTextBox.Text,
                    FtpProtocol = ftpProtocolComboBox.SelectedIndex,
                    FtpUsePassiveMode = usePassive,
                    Proxy = proxy,
                    ProxyUsername = proxyUsername,
                    ProxyPassword = proxyPassword,
                    UseStatistics = useStatisticsServerRadioButton.Checked,
                    SqlDatabaseName = SqlDatabaseName,
                    SqlWebUrl = SqlWebUrl,
                    SqlUsername = SqlUsername,
                    SqlPassword = sqlPassword,
                    PrivateKey = PrivateKey,
                    PublicKey = PublicKey,
                    Log = null,
                };

                try
                {
                    ApplicationInstance.SaveProject(localPathTextBox.Text, project); // ... and save it
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to save project file.", ex, PopupButtons.Ok);
                    Reset();
                    Close();
                }

                Program.ExisitingProjects.Add(project.Name, project.Path);
                try
                {
                    var projectEntries =
                        Program.ExisitingProjects.Select(
                            projectEntry => String.Format("{0}%{1}", projectEntry.Key, projectEntry.Value)).ToList();

                    File.WriteAllText(Program.ProjectsConfigFilePath, String.Join(Environment.NewLine, projectEntries));
                    _projectConfigurationEdited = true;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create the project-entry.", ex,
                        PopupButtons.Ok);
                    Reset();
                    Close();
                }

                if (useStatisticsServerRadioButton.Checked)
                {
                    string phpFilePath = Path.Combine(Program.Path, "Projects", nameTextBox.Text, "statistics.php");
                    try
                    {
                        File.WriteAllBytes(phpFilePath, Resources.statistics);

                        string phpFileContent = File.ReadAllText(phpFilePath);
                        phpFileContent = phpFileContent.Replace("_DBURL", SqlWebUrl);
                        phpFileContent = phpFileContent.Replace("_DBUSER", SqlUsername);
                        phpFileContent = phpFileContent.Replace("_DBNAME", SqlDatabaseName);
                        phpFileContent = phpFileContent.Replace("_DBPASS", sqlPasswordTextBox.Text);
                        File.WriteAllText(phpFilePath, phpFileContent);
                        _phpFileCreated = true;
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Failed to initialize the project-files.", ex,
                            PopupButtons.Ok);
                        Reset();
                        Close();
                    }
                }

                _generalTabPassed = true;
                ThreadPool.QueueUserWorkItem(arg => InitializeProject());
            }
        }


        /// <summary>
        ///     Provides a new thread that sets up the project data.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:SQL-Abfragen auf Sicherheitsrisiken überprüfen")]
        private void InitializeProject()
        {
            SetUiState(false);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Testing connection to the FTP-server..."));

            try
            {
                _ftp.TestConnection();
            }
            catch (FtpAuthenticationException ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while authenticating the certificate.",
                                ex.InnerException ?? ex, PopupButtons.Ok)));
                SetUiState(true);
                Reset();
                return;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while testing the FTP-data.",
                                ex.InnerException ?? ex, PopupButtons.Ok)));
                SetUiState(true);
                Reset();
                return;
            }

            /*
             *  Setup the "statistics.php" if necessary.
             */

            bool useStatistics = false;
            BeginInvoke(new Action(() => useStatistics = useStatisticsServerRadioButton.Checked));

            string name = null;
            if (useStatistics)
            {
                try
                {
                    Invoke(
                        new Action(
                            () =>
                                name = nameTextBox.Text));

                    string phpFilePath = Path.Combine(Program.Path, "Projects", name, "statistics.php");
                    _ftp.UploadFile(phpFilePath);
                    _phpFileUploaded = true;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the PHP-file.",
                                    ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                /*
             *  Setup the SQL-server and database.
             */

                Invoke(
                    new Action(
                        () =>
                            name = nameTextBox.Text));

                #region "Setup-String"

                string setupString = @"CREATE DATABASE IF NOT EXISTS _DBNAME;
USE _DBNAME;

CREATE TABLE IF NOT EXISTS `_DBNAME`.`Application` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `_DBNAME`.`Version` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Version` VARCHAR(40) NOT NULL,
  `Application_ID` INT NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Version_Application_idx` (`Application_ID` ASC),
  CONSTRAINT `fk_Version_Application`
    FOREIGN KEY (`Application_ID`)
    REFERENCES `_DBNAME`.`Application` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `_DBNAME`.`Download` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Version_ID` INT NOT NULL,
  `DownloadDate` DATETIME NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Download_Version1_idx` (`Version_ID` ASC),
  CONSTRAINT `fk_Download_Version1`
    FOREIGN KEY (`Version_ID`)
    REFERENCES `_DBNAME`.`Version` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

INSERT INTO Application (`ID`, `Name`) VALUES (_APPID, '_APPNAME');";

                #endregion

                setupString = setupString.Replace("_DBNAME", SqlDatabaseName);
                setupString = setupString.Replace("_APPNAME", name);
                setupString = setupString.Replace("_APPID",
                    Settings.Default.ApplicationID.ToString(CultureInfo.InvariantCulture));

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Connecting to SQL-server..."));

                MySqlConnection myConnection;
                try
                {
                    string myConnectionString = null;
                    Invoke(new Action(() =>
                    {
                        myConnectionString = String.Format("SERVER={0};" +
                                                           "DATABASE={1};" +
                                                           "UID={2};" +
                                                           "PASSWORD={3};", SqlWebUrl, SqlDatabaseName,
                            SqlUsername, sqlPasswordTextBox.Text);
                    }));

                    myConnection = new MySqlConnection(myConnectionString);
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    SetUiState(true);
                    Reset();
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    SetUiState(true);
                    Reset();
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Executing setup commands..."));

                MySqlCommand command = myConnection.CreateCommand();
                command.CommandText = setupString;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                    ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }
            }

            SetUiState(true);
            Invoke(new Action(
                Close));
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (_sender == ftpTabPage)
            {
                informationCategoriesTabControl.SelectedTab = generalTabPage;
                backButton.Enabled = false;
                _sender = generalTabPage;

                if (_generalTabPassed)
                    backButton.Enabled = false;
            }
            else if (_sender == statisticsServerTabPage)
            {
                informationCategoriesTabControl.SelectedTab = ftpTabPage;
                _sender = ftpTabPage;
            }
            else if (_sender == proxyTabPage)
            {
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
                _sender = statisticsServerTabPage;
            }
        }

        private void ftpPortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) < 0)
                e.Handled = true;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidatePanelWithIgnoring(ftpPanel, ftpDirectoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.", PopupButtons.Ok);
                return;
            }

            // FtpProtocol protocol = Equals(ftpModeComboBox.SelectedIndex, 0) ? FtpProtocol.FTP : FtpProtocol.FTPS;
            // TODO: Protocol

            var securePwd = new SecureString();
            foreach (char sign in ftpPasswordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchDialog = new DirectorySearchDialog
            {
                ProjectName = nameTextBox.Text,
                Host = ftpHostTextBox.Text,
                Port = int.Parse(ftpPortTextBox.Text),
                UsePassiveMode = ftpModeComboBox.SelectedIndex.Equals(0),
                Username = ftpUserTextBox.Text,
                Password = securePwd,
                Protocol = ftpProtocolComboBox.SelectedIndex,
            };

            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;

            searchDialog.Close();
        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog {Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj", CheckFileExists = false};
            if (fileDialog.ShowDialog() == DialogResult.OK)
                localPathTextBox.Text = fileDialog.FileName;
        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Management of sensible data.",
                "All your passwords will be encrypted with AES 256. The key and initializing vector is your FTP-username and password, so you have to enter them each time you open a project.",
                PopupButtons.Ok);
        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            statisticsInfoPanel.Enabled = useStatisticsServerRadioButton.Checked;
            selectServerButton.Enabled = useStatisticsServerRadioButton.Checked;
        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    UpdateProject importProject = ApplicationInstance.LoadProject(fileDialog.FileName);
                    ftpHostTextBox.Text = importProject.FtpHost;
                    ftpPortTextBox.Text = importProject.FtpPort.ToString(CultureInfo.InvariantCulture);
                    ftpUserTextBox.Text = importProject.FtpUsername;
                    ftpProtocolComboBox.SelectedIndex = importProject.FtpProtocol;
                    ftpModeComboBox.SelectedIndex = importProject.FtpUsePassiveMode ? 0 : 1;
                    ftpDirectoryTextBox.Text = importProject.FtpDirectory;
                    ftpPasswordTextBox.Focus();
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while importing project data.", ex,
                        PopupButtons.Ok);
                }
            }
        }

        private void selectServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerDialog = new StatisticsServerDialog {ReactsOnKeyDown = true};
            if (statisticsServerDialog.ShowDialog() != DialogResult.OK) return;

            SqlDatabaseName = statisticsServerDialog.SqlDatabaseName;
            SqlWebUrl = statisticsServerDialog.SqlWebUrl;
            SqlUsername = statisticsServerDialog.SqlUsername;
            string sqlNameString = SqlDatabaseName;
            databaseNameLabel.Text = sqlNameString;
        }

        private void doNotUseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            proxyPanel.Enabled = doNotUseProxyRadioButton.Checked;
        }

        public void Reset()
        {
            Invoke(new Action(
                () =>
                    loadingLabel.Text = "Resetting data..."));

            if (_projectFileCreated)
            {
                try
                {
                    string localPath = null;
                    Invoke(new Action(() => localPath = localPathTextBox.Text));
                    File.Delete(localPath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while deleting the project file.",
                                    ex, PopupButtons.Ok)));
                }
            }

            if (_projectConfigurationEdited)
            {
                string name = null;
                Invoke(new Action(() => name = nameTextBox.Text));
                Program.ExisitingProjects.Remove(name);

                try
                {
                    var projectEntries =
                        Program.ExisitingProjects.Select(
                            projectEntry => String.Format("{0}%{1}", projectEntry.Key, projectEntry.Value)).ToList();

                    File.WriteAllText(Program.ProjectsConfigFilePath, String.Join(Environment.NewLine, projectEntries));
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while deleting the project file.",
                                    ex, PopupButtons.Ok)));
                }
            }

            if (_phpFileCreated)
            {
                string phpFilePath = Path.Combine(Program.Path, "Projects", nameTextBox.Text, "statistics.php");
                try
                {
                    File.Delete(phpFilePath);
                    _phpFileCreated = false;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while deleting \"statistics.php\" again.",
                                    ex, PopupButtons.Ok)));
                }
            }

            if (_phpFileUploaded)
            {
                try
                {
                    _ftp.DeleteFile("statistics.php");
                    _phpFileUploaded = false;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while deleting \"statistics.php\" on the server again.",
                                    ex, PopupButtons.Ok)));
                }
            }

            Settings.Default.ApplicationID -= 1;
            Settings.Default.Save();
            Settings.Default.Reload();
        }
    }
}