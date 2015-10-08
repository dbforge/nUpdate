// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using nUpdate.Administration.Application;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.Ftp.Exceptions;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using static System.String;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private readonly FTPManager _ftp = new FTPManager();
        private bool _allowCancel;
        private string _ftpAssemblyPath;
        private bool _generalTabPassed;
        private bool _isSetByUser = true;
        private int _lastSelectedIndex;
        private bool _mustClose;
        private bool _phpFileCreated;
        private bool _phpFileUploaded;
        private List<ProjectConfiguration> _projectConfiguration;
        private bool _projectConfigurationEdited;
        private bool _projectFileCreated;
        private string _sqlDatabaseName;
        private string _sqlWebUrl;
        private string _sqlUsername;
        private readonly BindingList<string> _parametersBindingList = new BindingList<string>(); 

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        public void SetUIState(bool enabled)
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

            SetUIState(true);
            if (_mustClose)
                Invoke(new Action(Close));
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            if (!WebConnection.IsAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection was found. In order to create a project a network connection is required in order to communicate with the server.",
                    PopupButtons.Ok);
                Close();
                return;
            }

            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;
            parametersListBox.DataSource = _parametersBindingList;
            Text = Format(Text, Program.VersionString);
            localPathTextBox.ButtonClicked += BrowseLocalPathButtonClick;
            localPathTextBox.Initialize();

            _projectConfiguration = ProjectConfiguration.Load().ToList();
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
                }));

                _allowCancel = true;
            });
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (informationCategoriesTabControl.SelectedTab == generalTabPage)
            {
                if (!ValidationManager.Validate(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid URL specified.", "The given Update-URL is invalid.",
                        PopupButtons.Ok);
                    return;
                }

                if (!Path.IsPathRooted(localPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }

                if (Path.GetInvalidFileNameChars().Any(item => nameTextBox.Text.Contains(item))) // TODO: Check if this is necessary as the project name is no longer used for directories
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid project name.",
                        "The given project name contains invalid chars.", PopupButtons.Ok);
                    return;
                }

                if (Path.GetInvalidPathChars().Any(item => localPathTextBox.Text.Contains(item)))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid project name.",
                        "The given project file path contains invalid chars.", PopupButtons.Ok);
                    return;
                }

                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    Path.GetFullPath(localPathTextBox.Text);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }
                
                backButton.Enabled = true;
                informationCategoriesTabControl.SelectedTab = ftpTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == ftpTabPage)
            {
                if (!ValidationManager.Validate(ftpPanel) || IsNullOrEmpty(ftpPasswordTextBox.Text))
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

                informationCategoriesTabControl.SelectedTab = ftpProtocolComboBox.SelectedIndex == ftpProtocolComboBox.Items.Count - 1 ? ftpTabPage1 : statisticsServerTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == ftpTabPage1)
            {
                if (!ValidationManager.ValidateTabPage(ftpTabPage1))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == statisticsServerTabPage)
            {
                if (useStatisticsServerRadioButton.Checked)
                {
                    if (_sqlDatabaseName == null || IsNullOrWhiteSpace(sqlPasswordTextBox.Text))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                        return;
                    }
                }
                
                informationCategoriesTabControl.SelectedTab = proxyTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == proxyTabPage)
            {
                if (useProxyRadioButton.Checked)
                {
                    if (!ValidationManager.ValidateTabPage(proxyTabPage) && !IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !IsNullOrEmpty(proxyPasswordTextBox.Text))
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
                if (!IsNullOrEmpty(proxyHostTextBox.Text))
                {
                    proxy = new WebProxy(proxyHostTextBox.Text);
                    if (!IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        proxyUsername = proxyUserTextBox.Text;
                        if (!saveCredentialsCheckBox.Checked)
                            proxyPassword = Convert.ToBase64String(AESManager.Encrypt(proxyPasswordTextBox.Text,
                                ftpPasswordTextBox.Text,
                                ftpUserTextBox.Text));
                        else
                            proxyPassword =
                                Convert.ToBase64String(AESManager.Encrypt(proxyPasswordTextBox.Text,
                                    Program.AesKeyPassword,
                                    Program.AesIvPassword));
                    }
                }

                string sqlPassword = null;
                if (useStatisticsServerRadioButton.Checked)
                {
                    if (!saveCredentialsCheckBox.Checked)
                        sqlPassword = Convert.ToBase64String(AESManager.Encrypt(sqlPasswordTextBox.Text,
                            ftpPasswordTextBox.Text,
                            ftpUserTextBox.Text));
                    else
                        sqlPassword =
                            Convert.ToBase64String(AESManager.Encrypt(sqlPasswordTextBox.Text, Program.AesKeyPassword,
                                Program.AesIvPassword));
                }

                Settings.Default.ApplicationID += 1;
                Settings.Default.Save();
                Settings.Default.Reload();

                string ftpPassword;
                if (!saveCredentialsCheckBox.Checked)
                    ftpPassword = Convert.ToBase64String(AESManager.Encrypt(ftpPasswordTextBox.Text,
                        ftpPasswordTextBox.Text,
                        ftpUserTextBox.Text));
                else
                    ftpPassword =
                        Convert.ToBase64String(AESManager.Encrypt(ftpPasswordTextBox.Text, Program.AesKeyPassword,
                            Program.AesIvPassword));

                // Create a new package...
                var project = new UpdateProject
                {
                    Path = localPathTextBox.Text,
                    Name = nameTextBox.Text,
                    Guid = Guid.NewGuid(),
                    ApplicationId = Settings.Default.ApplicationID,
                    UpdateUrl = updateUrlTextBox.Text,
                    Packages = null,
                    SaveCredentials = saveCredentialsCheckBox.Checked,
                    FtpHost = ftpHostTextBox.Text,
                    FtpPort = int.Parse(ftpPortTextBox.Text),
                    FtpUsername = ftpUserTextBox.Text,
                    FtpPassword = ftpPassword,
                    FtpDirectory = ftpDirectoryTextBox.Text,
                    FtpProtocol = ftpProtocolComboBox.SelectedIndex,
                    FtpUsePassiveMode = usePassive,
                    FtpTransferAssemblyFilePath = _ftpAssemblyPath,
                    Proxy = proxy,
                    ProxyUsername = proxyUsername,
                    ProxyPassword = proxyPassword,
                    UseStatistics = useStatisticsServerRadioButton.Checked,
                    SqlDatabaseName = _sqlDatabaseName,
                    SqlWebUrl = _sqlWebUrl,
                    SqlUsername = _sqlUsername,
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
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the project file.", ex, PopupButtons.Ok);
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
                        phpFileContent = phpFileContent.Replace("_DBURL", _sqlWebUrl);
                        phpFileContent = phpFileContent.Replace("_DBUSER", _sqlUsername);
                        phpFileContent = phpFileContent.Replace("_DBNAME", _sqlDatabaseName);
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

        [SuppressMessage("Microsoft.Security", "CA2100:SQL-Abfragen auf Sicherheitsrisiken überprüfen")]
        private async void InitializeProject()
        {
            await Task.Factory.StartNew(() =>
            {
                SetUIState(false);
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

                    setupString = setupString.Replace("_DBNAME", _sqlDatabaseName);
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
                            myConnectionString = $"SERVER={_sqlWebUrl};" + $"DATABASE={_sqlDatabaseName};" +
                                                 $"UID={_sqlUsername};" + $"PASSWORD={sqlPasswordTextBox.Text};";
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

                SetUIState(true);
                Invoke(new Action(Close));
            });
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (informationCategoriesTabControl.SelectedTab == ftpTabPage)
            {
                backButton.Enabled = false;
                informationCategoriesTabControl.SelectedTab = generalTabPage;

                if (_generalTabPassed)
                    backButton.Enabled = false;
            }
            else if (informationCategoriesTabControl.SelectedTab == statisticsServerTabPage)
                informationCategoriesTabControl.SelectedTab = ftpTabPage;
            else if (informationCategoriesTabControl.SelectedTab == proxyTabPage)
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
        }

        private void ftpPortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) < 0)
                e.Handled = true;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateWithIgnoring(ftpPanel, ftpDirectoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.", PopupButtons.Ok);
                return;
            }

            var securePassword = new SecureString();
            foreach (var sign in ftpPasswordTextBox.Text)
                securePassword.AppendChar(sign);

            var searchDialog = new DirectorySearchDialog
            {
                ProjectName = nameTextBox.Text,
                Host = ftpHostTextBox.Text,
                Port = int.Parse(ftpPortTextBox.Text),
                UsePassiveMode = ftpModeComboBox.SelectedIndex.Equals(0),
                Username = ftpUserTextBox.Text,
                Password = securePassword,
                Protocol = ftpProtocolComboBox.SelectedIndex
            };

            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;

            securePassword.Dispose();
            searchDialog.Close();
        }

        private void BrowseLocalPathButtonClick(object sender, EventArgs e)
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
                saveCredentialsCheckBox.Checked
                    ? "All your passwords will be encrypted with AES 256 by using a hardcoded key and initialization vector."
                    : "All your passwords will be encrypted with AES 256. The key and initialization vector are your FTP-username and password, so you have to enter them each time you open the project.",
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

            _sqlDatabaseName = statisticsServerDialog.SqlDatabaseName;
            _sqlWebUrl = statisticsServerDialog.SqlWebUrl;
            _sqlUsername = statisticsServerDialog.SqlUsername;
            var sqlNameString = _sqlDatabaseName;
            databaseNameLabel.Text = sqlNameString;
        }

        private void doNotUseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            proxyPanel.Enabled = doNotUseProxyRadioButton.Checked;
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

        private void addParameterButton_Click(object sender, EventArgs e)
        {
            var parameterAddDialog = new ParameterAddDialog();
            if (parameterAddDialog.ShowDialog() == DialogResult.OK)
                _parametersBindingList.Add(parameterAddDialog.Parameter);
        }

        private void parametersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeParameterButton.Enabled = parametersListBox.SelectedIndex >= 0;
        }
    }
}