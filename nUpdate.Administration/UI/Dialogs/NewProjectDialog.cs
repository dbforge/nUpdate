// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private readonly FtpManager _ftp = new FtpManager();
        private bool _allowCancel;
        private string _ftpAssemblyPath;
        private bool _generalTabPassed;
        private bool _isSetByUser = true;
        private int _lastSelectedIndex;
        private bool _mustClose;
        //private LocalizationProperties _lp = new LocalizationProperties();
        private bool _phpFileCreated;
        private bool _phpFileUploaded;
        private List<ProjectConfiguration> _projectConfiguration;
        private bool _projectConfigurationEdited;
        private bool _projectFileCreated;
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

        public void Reset()
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Resetting data..."));

            if (_projectFileCreated)
            {
                try
                {
                    string localPath = null;
                    Invoke(new Action(() => localPath = localPathTextBox.Text));
                    File.Delete(localPath);
                    _projectFileCreated = false;
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
                Invoke(
                    new Action(
                        () =>
                            name = nameTextBox.Text));
                _projectConfiguration.RemoveAt(_projectConfiguration.FindIndex(item => item.Name == name));

                try
                {
                    File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(_projectConfiguration));
                    _projectConfigurationEdited = false;
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
                var phpFilePath = Path.Combine(Program.Path, "Projects", nameTextBox.Text, "statistics.php");
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

            bool useStatistics = false;
            Invoke(new Action(() => useStatistics = useStatisticsServerRadioButton.Checked));
            if (useStatistics)
            {
                Settings.Default.ApplicationID -= 1;
                Settings.Default.Save();
            }

            SetUiState(true);
            if (_mustClose)
                Invoke(new Action(Close));
        }

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
            if (!ConnectionManager.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection was found. In order to create a project a network connection is required in order to communicate with the server.",
                    PopupButtons.Ok);
                Close();
                return;
            }

            ftpPortTextBox.ShortcutsEnabled = false;
            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;
            ipVersionComboBox.SelectedIndex = 0;

            //SetLanguage();
            Text = string.Format(Text, Program.VersionString);
            localPathTextBox.ButtonClicked += BrowsePathButtonClick;
            localPathTextBox.Initialize();
            controlPanel1.Visible = false;
            GenerateKeyPair();

            _isSetByUser = true;
        }

        private void NewProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private async void GenerateKeyPair()
        {
            await Task.Factory.StartNew(() =>
            {
                var rsa = new RsaManager();
                PrivateKey = rsa.PrivateKey;
                PublicKey = rsa.PublicKey;

                Invoke(new Action(() =>
                {
                    controlPanel1.Visible = true;
                    informationCategoriesTabControl.SelectedTab = generalTabPage;
                    _sender = generalTabPage;
                }));

                _allowCancel = true;
            });
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (_sender == generalTabPage)
            {
                if (!ValidationManager.Validate(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!_generalTabPassed)
                {
                    _projectConfiguration =
                        ProjectConfiguration.Load().ToList();
                    if (_projectConfiguration != null)
                    {
                        if (_projectConfiguration.Any(item => item.Name == nameTextBox.Text))
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                                $"The project \"{nameTextBox.Text}\" is already existing.", PopupButtons.Ok);
                            return;
                        }
                    }
                    else
                    {
                        _projectConfiguration = new List<ProjectConfiguration>();
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

                _sender = httpAuthenticationTabPage;
                backButton.Enabled = true;
                informationCategoriesTabControl.SelectedTab = httpAuthenticationTabPage;
            }
            else if (_sender == httpAuthenticationTabPage)
            {
                if (httpAuthenticationCheckBox.Checked && !ValidationManager.Validate(httpAuthenticationPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                _sender = ftpTabPage;
                informationCategoriesTabControl.SelectedTab = ftpTabPage;
            }
            else if (_sender == ftpTabPage)
            {
                if (!ValidationManager.Validate(ftpPanel) || string.IsNullOrEmpty(ftpPasswordTextBox.Text))
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
                _ftp.Protocol = (FtpsSecurityProtocol) ftpProtocolComboBox.SelectedIndex;
                _ftp.NetworkVersion = (NetworkVersion) ipVersionComboBox.SelectedIndex;

                if (!backButton.Enabled) // If the back-button was disabled, enabled it again
                    backButton.Enabled = true;

                _sender = statisticsServerTabPage;
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (_sender == statisticsServerTabPage)
            {
                if (useStatisticsServerRadioButton.Checked)
                {
                    if (SqlDatabaseName == null || string.IsNullOrWhiteSpace(sqlPasswordTextBox.Text))
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
                if (useProxyRadioButton.Checked &&
                    !ValidationManager.ValidateAndIgnore(proxyTabPage, new[] {proxyUserTextBox, proxyPasswordTextBox}))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
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
                if (!string.IsNullOrEmpty(proxyHostTextBox.Text))
                {
                    proxy = new WebProxy(proxyHostTextBox.Text);
                    if (!string.IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !string.IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        proxyUsername = proxyUserTextBox.Text;
                        if (!saveCredentialsCheckBox.Checked)
                            proxyPassword = Convert.ToBase64String(AesManager.Encrypt(proxyPasswordTextBox.Text,
                                ftpPasswordTextBox.Text,
                                ftpUserTextBox.Text));
                        else
                            proxyPassword =
                                Convert.ToBase64String(AesManager.Encrypt(proxyPasswordTextBox.Text,
                                    Program.AesKeyPassword,
                                    Program.AesIvPassword));
                    }
                }

                string sqlPassword = null;
                if (useStatisticsServerRadioButton.Checked)
                {
                    if (!saveCredentialsCheckBox.Checked)
                        sqlPassword = Convert.ToBase64String(AesManager.Encrypt(sqlPasswordTextBox.Text,
                            ftpPasswordTextBox.Text,
                            ftpUserTextBox.Text));
                    else
                        sqlPassword =
                            Convert.ToBase64String(AesManager.Encrypt(sqlPasswordTextBox.Text, Program.AesKeyPassword,
                                Program.AesIvPassword));
                }

                Settings.Default.ApplicationID += 1;
                Settings.Default.Save();
                Settings.Default.Reload();

                string ftpPassword;
                if (!saveCredentialsCheckBox.Checked)
                    ftpPassword = Convert.ToBase64String(AesManager.Encrypt(ftpPasswordTextBox.Text,
                        ftpPasswordTextBox.Text,
                        ftpUserTextBox.Text));
                else
                    ftpPassword =
                        Convert.ToBase64String(AesManager.Encrypt(ftpPasswordTextBox.Text, Program.AesKeyPassword,
                            Program.AesIvPassword));

                // Create a new package...
                var project = new UpdateProject
                {
                    Path = localPathTextBox.Text,
                    Name = nameTextBox.Text,
                    Guid = Guid.NewGuid().ToString(),
                    ApplicationId = Settings.Default.ApplicationID,
                    UpdateUrl = updateUrlTextBox.Text,
                    Packages = null,
                    SaveCredentials = saveCredentialsCheckBox.Checked,
                    HttpAuthenticationCredentials = httpAuthenticationCheckBox.Checked
                        ? new NetworkCredential(httpAuthenticationUserTextBox.Text,
                            httpAuthenticationPasswordTextBox.Text)
                        : null,
                    FtpHost = ftpHostTextBox.Text,
                    FtpPort = int.Parse(ftpPortTextBox.Text),
                    FtpUsername = ftpUserTextBox.Text,
                    FtpPassword = ftpPassword,
                    FtpDirectory = ftpDirectoryTextBox.Text,
                    FtpProtocol = ftpProtocolComboBox.SelectedIndex,
                    FtpUsePassiveMode = usePassive,
                    FtpTransferAssemblyFilePath = _ftpAssemblyPath,
                    FtpNetworkVersion = (NetworkVersion) ipVersionComboBox.SelectedIndex,
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
                    Log = null
                };

                try
                {
                    UpdateProject.SaveProject(localPathTextBox.Text, project); // ... and save it
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the project file.", ex,
                        PopupButtons.Ok);
                    _mustClose = true;
                    Reset();
                }

                try
                {
                    var projectDirectoryPath = Path.Combine(Program.Path, "Projects", nameTextBox.Text);
                    Directory.CreateDirectory(projectDirectoryPath);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the project'S directory.", ex,
                        PopupButtons.Ok);
                    _mustClose = true;
                    Reset();
                }

                try
                {
                    _projectConfiguration.Add(new ProjectConfiguration(nameTextBox.Text, localPathTextBox.Text));
                    File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(_projectConfiguration));
                    _projectConfigurationEdited = true;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while editing the project confiuration file. Please choose another name for the project.",
                        ex,
                        PopupButtons.Ok);
                    _mustClose = true;
                    Reset();
                }

                if (useStatisticsServerRadioButton.Checked)
                {
                    var phpFilePath = Path.Combine(Program.Path, "Projects", nameTextBox.Text, "statistics.php");
                    try
                    {
                        File.WriteAllBytes(phpFilePath, Resources.statistics);

                        var phpFileContent = File.ReadAllText(phpFilePath);
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
                        _mustClose = true;
                        Reset();
                    }
                }

                _generalTabPassed = true;
                InitializeProject();
            }
        }

        /// <summary>
        ///     Provides a new thread that sets up the project data.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:SQL-Abfragen auf Sicherheitsrisiken überprüfen")]
        private async void InitializeProject()
        {
            await Task.Factory.StartNew(() =>
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
                catch (FtpsAuthenticationException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while authenticating the certificate.",
                                    ex.InnerException ?? ex, PopupButtons.Ok)));
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
                    Reset();
                    return;
                }

                /*
             *  Setup the "statistics.php" if necessary.
             */

                var useStatistics = false;
                Invoke(new Action(() => useStatistics = useStatisticsServerRadioButton.Checked));

                string name = null;
                if (useStatistics)
                {
                    try
                    {
                        Invoke(
                            new Action(
                                () =>
                                    name = nameTextBox.Text));

                        var phpFilePath = Path.Combine(Program.Path, "Projects", name, "statistics.php");
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

                    var setupString = @"CREATE DATABASE IF NOT EXISTS _DBNAME;
USE _DBNAME;

CREATE TABLE IF NOT EXISTS `_DBNAME`.`Application` (
  `ID` INT NOT NULL,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `_DBNAME`.`Version` (
  `ID` INT NOT NULL,
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
  `OperatingSystem` VARCHAR(20) NOT NULL,
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
                            myConnectionString = $"SERVER='{SqlWebUrl}';" + $"DATABASE='{SqlDatabaseName}';" +
                                                 $"UID='{SqlUsername}';" +
                                                 $"PASSWORD='{sqlPasswordTextBox.Text}';";
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
                        Reset();
                        return;
                    }

                    Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Executing setup commands..."));

                    var command = myConnection.CreateCommand();
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
                        return;
                    }
                }

                SetUiState(true);
                Invoke(new Action(
                    Close));
            });
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (_sender == httpAuthenticationTabPage)
            {
                informationCategoriesTabControl.SelectedTab = generalTabPage;
                backButton.Enabled = false;
                _sender = generalTabPage;

                if (_generalTabPassed)
                    backButton.Enabled = false;
            }
            else if (_sender == ftpTabPage)
            {
                informationCategoriesTabControl.SelectedTab = httpAuthenticationTabPage;
                _sender = httpAuthenticationTabPage;
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
            if (!ValidationManager.ValidateAndIgnore(ftpPanel, new [] { ftpDirectoryTextBox }))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.", PopupButtons.Ok);
                return;
            }

            var securePwd = new SecureString();
            foreach (var sign in ftpPasswordTextBox.Text)
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
                NetworkVersion = (NetworkVersion)ipVersionComboBox.SelectedIndex
            };

            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;

            securePwd.Dispose();
            searchDialog.Close();
        }

        private void BrowsePathButtonClick(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.CheckFileExists = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    localPathTextBox.Text = fileDialog.FileName;
            }
        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Management of sensible data.",
                "All your passwords will be encrypted with AES 256. The key and initializing vector are your FTP-username and password, so you have to enter them each time you open the project.",
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
                    var importProject = UpdateProject.LoadProject(fileDialog.FileName);
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
            if (statisticsServerDialog.ShowDialog() != DialogResult.OK)
                return;

            SqlDatabaseName = statisticsServerDialog.SqlDatabaseName;
            SqlWebUrl = statisticsServerDialog.SqlWebUrl;
            SqlUsername = statisticsServerDialog.SqlUsername;
            var sqlNameString = SqlDatabaseName;
            databaseNameLabel.Text = sqlNameString;
        }

        private void doNotUseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            proxyPanel.Enabled = !doNotUseProxyRadioButton.Checked;
        }

        private void ftpProtocolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isSetByUser || (ftpProtocolComboBox.SelectedIndex != ftpProtocolComboBox.Items.Count - 1))
                return;
            var ftpAssemblyInputDialog = new FtpAssemblyInputDialog();
            if (ftpAssemblyInputDialog.ShowDialog() == DialogResult.Cancel)
                ftpProtocolComboBox.SelectedIndex = _lastSelectedIndex;
            else
                _ftpAssemblyPath = ftpAssemblyInputDialog.AssemblyPath;

            _lastSelectedIndex = ftpProtocolComboBox.SelectedIndex;
        }

        private void httpAuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            httpAuthenticationPanel.Enabled = httpAuthenticationCheckBox.Checked;
        }
    }
}