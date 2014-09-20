// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog
    {
        private readonly FtpManager _ftp = new FtpManager();
        private bool _allowCancel = true;
        private bool _generalTabPassed;

        private LocalizationProperties _lp = new LocalizationProperties();
        private bool _phpFileUploaded;
        private bool _projectDataAlreadyInitialized;
        private Sql _sql = new Sql();

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
        ///     Sets the current tabpage.
        /// </summary>
        public TabPage Sender { get; set; }

        /// <summary>
        ///     Sets the language
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            if (File.Exists(languageFilePath))
                _lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    _lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = _lp.NewProjectDialogTitle;
            Text = String.Format(Text, _lp.ProductTitle);

            cancelButton.Text = _lp.CancelButtonText;
            continueButton.Text = _lp.ContinueButtonText;

            keyPairHeaderLabel.Text = _lp.PanelSignatureHeader;
            keyPairInfoLabel.Text = _lp.PanelSignatureInfoText;
            keyPairGenerationLabel.Text = _lp.PanelSignatureWaitText;

            generalHeaderLabel.Text = _lp.PanelGeneralHeader;
            nameLabel.Text = _lp.PanelGeneralNameText;
            nameTextBox.Cue = _lp.PanelGeneralNameWatermarkText;

            ftpHeaderLabel.Text = _lp.PanelFtpHeader;
            ftpHostLabel.Text = _lp.PanelFtpServerText;
            ftpUserLabel.Text = _lp.PanelFtpUserText;
            ftpUserTextBox.Cue = _lp.PanelFtpUserWatermarkText;
            ftpPasswordLabel.Text = _lp.PanelFtpPasswordText;
            ftpPortLabel.Text = _lp.PanelFtpPortText;
            ftpPortTextBox.Cue = _lp.PanelFtpPortWatermarkText;
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            ftpPortTextBox.ShortcutsEnabled = false;

            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;

            //SetLanguage();

            controlPanel1.Visible = false;
            _allowCancel = false;

            ThreadPool.QueueUserWorkItem(delegate { GenerateKeyPair(); }, null);
        }

        private void NewProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        ///     Generates the key pair in an own thread.
        /// </summary>
        private void GenerateKeyPair()
        {
            // Create a new instance of the RsaSignature-class
            var rsa = new RsaSignature();

            // Initialize the properties with the keys
            PrivateKey = rsa.PrivateKey;
            PublicKey = rsa.PublicKey;

            Invoke(new Action(() =>
            {
                controlPanel1.Visible = true;
                tablessTabControl1.SelectedTab = generalTabPage;
                Sender = generalTabPage;
            }));

            _allowCancel = true;
        }

        private void SetUiState(bool enabled)
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
            if (Sender == generalTabPage)
            {
                if (!ValidationManager.ValidatePanel(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                var entriesToDelete = new List<string>();
                foreach (var existingProject in Program.ExisitingProjects)
                {
                    if (existingProject.Key == nameTextBox.Text && File.Exists(existingProject.Value))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                            String.Format(
                                "The project \"{0}\" is already existing. Please choose another name for it.",
                                nameTextBox.Text), PopupButtons.Ok);
                        return;
                    }
                    if (!File.Exists(existingProject.Value))
                        entriesToDelete.Add(existingProject.Key);
                }

                foreach (string entryToDelete in entriesToDelete)
                {
                    Program.ExisitingProjects.Remove(entryToDelete);
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

                Sender = ftpTabPage;
                backButton.Enabled = true;
                tablessTabControl1.SelectedTab = ftpTabPage;
            }
            else if (Sender == ftpTabPage)
            {
                if (!ValidationManager.ValidatePanel(ftpPanel) || String.IsNullOrEmpty(ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                _ftp.Host = ftpHostTextBox.Text;
                _ftp.Port = int.Parse(ftpPortTextBox.Text);
                _ftp.UserName = ftpUserTextBox.Text;
                _ftp.Directory = ftpDirectoryTextBox.Text;

                var ftpPassword = new SecureString();
                foreach (Char c in ftpPasswordTextBox.Text)
                {
                    ftpPassword.AppendChar(c);
                }
                _ftp.Password = ftpPassword;

                _ftp.UsePassiveMode = ftpModeComboBox.SelectedIndex == 0;
                _ftp.Protocol = (FtpSecurityProtocol)ftpProtocolComboBox.SelectedIndex;

                if (!backButton.Enabled) // If the back-button was disabled, enabled it again
                    backButton.Enabled = true;

                Sender = statisticsServerTabPage;
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
            }
            else if (Sender == statisticsServerTabPage)
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

                Sender = proxyTabPage;
                tablessTabControl1.SelectedTab = proxyTabPage;
            }
            else if (Sender == proxyTabPage)
            {
                if (!String.IsNullOrEmpty(proxyUserTextBox.Text) && !ValidationManager.ValidatePanel(proxyTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }
                if (!String.IsNullOrEmpty(proxyPasswordTextBox.Text) && !ValidationManager.ValidatePanel(proxyTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!_projectDataAlreadyInitialized)
                {
                    try
                    {
                        using (File.Create(localPathTextBox.Text))
                        {
                        }
                    }
                    catch (IOException ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.Ok);
                        Close();
                    }

                    bool usePassive = ftpModeComboBox.SelectedIndex == 0;

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
                                Convert.ToBase64String(AesManager.Encrypt(proxyPasswordTextBox.Text, ftpPasswordTextBox.Text,
                                    ftpUserTextBox.Text));
                        }
                    }

                    string sqlPassword = null;
                    if (useStatisticsServerRadioButton.Checked)
                    {
                        sqlPassword =
                            Convert.ToBase64String(AesManager.Encrypt(sqlPasswordTextBox.Text, ftpPasswordTextBox.Text,
                                    ftpUserTextBox.Text));
                        _sql.Password = sqlPassword;
                    }

                    Settings.Default.ApplicationID += 1;
                    Settings.Default.Save();
                    Settings.Default.Reload(); // TODO: Check if it is necessary...

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
                        SqlSettings = _sql,
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
                        Close();
                    }

                    Program.ExisitingProjects.Add(project.Name, project.Path);
                    try
                    {
                        var builder = new StringBuilder();
                        foreach (var dictionaryEntry in Program.ExisitingProjects)
                        {
                            builder.AppendLine(String.Format("{0}-{1}", dictionaryEntry.Key, dictionaryEntry.Value));
                        }
                        File.WriteAllText(Program.ProjectsConfigFilePath, builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Failed to create the project-entry.", ex,
                            PopupButtons.Ok);
                        Close();
                    }

                    string projectDirectory = Path.Combine(Program.Path, "Projects", nameTextBox.Text);
                    string phpFilePath = Path.Combine(projectDirectory, "getfile.php");
                    try
                    {
                        Directory.CreateDirectory(projectDirectory);
                        File.WriteAllBytes(phpFilePath, Resources.getfile);

                        if (useStatisticsServerRadioButton.Checked)
                        {
                            string phpFileContent = File.ReadAllText(phpFilePath);
                            phpFileContent = phpFileContent.Replace("_DBURL", _sql.WebUrl);
                            phpFileContent = phpFileContent.Replace("_DBUSER", _sql.Username);
                            phpFileContent = phpFileContent.Replace("_DBNAME", _sql.DatabaseName);
                            phpFileContent = phpFileContent.Replace("_DBPASS", sqlPasswordTextBox.Text);
                            File.WriteAllText(phpFilePath, phpFileContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Failed to initialize the project-files.", ex,
                            PopupButtons.Ok);
                        Directory.Delete(projectDirectory);
                        Close();
                    }
                }

                _projectDataAlreadyInitialized = true; // The data is now created.
                _generalTabPassed = true;

                SetUiState(false);
                if (useStatisticsServerRadioButton.Checked)
                    ThreadPool.QueueUserWorkItem(arg => InitializeRemoteData(true));
                else
                    ThreadPool.QueueUserWorkItem(arg => InitializeRemoteData(false));
            }
        }


        /// <summary>
        ///     Provides a new thread that sets up the PHP-file and optinally the statistics server.
        /// </summary>
        private void InitializeRemoteData(bool setupStatisticsServer)
        {
            /*
             *  Setup the "getfile.php".
             */

            if (!_phpFileUploaded)
            {
                try
                {
                    string name = null;
                    Invoke(
                        new Action(
                            () =>
                                name = nameTextBox.Text));

                    string projectDirectory = Path.Combine(Program.Path, "Projects", name);
                    string phpFilePath = Path.Combine(projectDirectory, "getfile.php");
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
                    SetUiState(true);
                    return;
                }
            }

            /*
             *  Setup the SQL-server and database.
             */

            if (setupStatisticsServer)
            {
                string name = null;
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
    REFERENCES `vstradesql2`.`Application` (`ID`)
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
    REFERENCES `vstradesql2`.`Version` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

INSERT INTO Application (`ÌD`, `Name`) VALUES (_APPID, '_APPNAME');";

                #endregion

                setupString = setupString.Replace("_DBNAME", _sql.DatabaseName);
                setupString = setupString.Replace("_APPNAME", name);
                setupString = setupString.Replace("_APPID", Settings.Default.ApplicationID.ToString());

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
                                                           "PASSWORD={3};", _sql.WebUrl, _sql.DatabaseName,
                            _sql.Username, sqlPasswordTextBox.Text);
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
            if (Sender == ftpTabPage)
            {
                tablessTabControl1.SelectedTab = generalTabPage;
                backButton.Enabled = false;
                Sender = generalTabPage;
            }
            else if (Sender == statisticsServerTabPage)
            {
                tablessTabControl1.SelectedTab = ftpTabPage;
                Sender = ftpTabPage;

                if (_generalTabPassed)
                    backButton.Enabled = false;
            }
            else if (Sender == proxyTabPage)
            {
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
                Sender = statisticsServerTabPage;
            }
        }

        private void ftpPortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString(), StringComparison.Ordinal) < 0)
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

            FtpProtocol protocol = Equals(ftpModeComboBox.SelectedIndex, 0) ? FtpProtocol.FTP : FtpProtocol.FTPS;

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
                Protocol = protocol,
            };

            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;

            searchDialog.Close();
        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog {Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj"};
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
            panel1.Enabled = useStatisticsServerRadioButton.Checked;
            selectServerButton.Enabled = useStatisticsServerRadioButton.Checked;
        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        UpdateProject importProject = ApplicationInstance.LoadProject(fileDialog.FileName);
                        ftpHostTextBox.Text = importProject.FtpHost;
                        ftpPortTextBox.Text = importProject.FtpPort.ToString();
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
        }

        private void selectServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerDialog = new StatisticsServerDialog();
            statisticsServerDialog.ReactsOnKeyDown = true;
            if (statisticsServerDialog.ShowDialog() == DialogResult.OK)
            {
                _sql = statisticsServerDialog.SqlSettings;
                string sqlNameString = _sql.DatabaseName;
                databaseNameLabel.Text = sqlNameString;
            }
        }
    }
}