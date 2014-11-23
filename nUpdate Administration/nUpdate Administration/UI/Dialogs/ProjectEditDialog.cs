using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectEditDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private readonly FtpManager _ftp = new FtpManager();
        private bool _allowCancel = true;
        private bool _generalTabPassed;

        private LocalizationProperties _lp = new LocalizationProperties();
        private string _name;
        private string _localPath;
        private string _sqlWebUrl;
        private string _sqlDatabaseName;
        private string _sqlUsername;
        private string _sqlPassword;
        private bool _useStatistics;
        private bool _phpFileCreated;
        private bool _phpFileUploaded;
        private bool _phpFileDeleted;
        private bool _projectDataAlreadyInitialized;
        private bool _projectAddedToConfig;
        private bool _projectDirectoryMoved;
        private bool _projectConfigurationEdited;
        private bool _projectFileMoved;
        private bool _localPathChanged;
        private bool _updateConfigurationSaved;
        private bool _updateUrlChanged;
        private bool _commandsExecuted;
        private TabPage _sender;
        private readonly Sql _sql = new Sql();
        private IEnumerable<UpdateConfiguration> _oldUpdateConfiguration;
        private IEnumerable<UpdateConfiguration> _newUpdateConfiguration; 

        public ProjectEditDialog()
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

            Text = _lp.ProjectEditDialogTitle;
            Text = String.Format(Text, _lp.ProductTitle);

            cancelButton.Text = _lp.CancelButtonText;
            continueButton.Text = _lp.ContinueButtonText;

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

        private void ProjectEditDialog_Load(object sender, EventArgs e)
        {
            ftpPortTextBox.ShortcutsEnabled = false;

            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;

            //SetLanguage();
        }

        private void ProjectEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        public void SetUiState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (Control c in from Control c in Controls where c.Visible select c)
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

                if (!_generalTabPassed)
                {
                    if (Project.Name != nameTextBox.Text &&
                        Program.ExisitingProjects.Any(item => item.Key == nameTextBox.Text))
                    {
                        var assumingProject =
                            Program.ExisitingProjects.First(item => item.Key == nameTextBox.Text);
                        if (File.Exists(assumingProject.Value))
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                                String.Format(
                                    "The project \"{0}\" is already existing. Please choose another name for it.",
                                    nameTextBox.Text), PopupButtons.Ok);
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
                tablessTabControl1.SelectedTab = ftpTabPage;
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
                foreach (Char c in ftpPasswordTextBox.Text)
                {
                    ftpPassword.AppendChar(c);
                }
                _ftp.Password = ftpPassword;

                _ftp.UsePassiveMode = ftpModeComboBox.SelectedIndex == 0;
                _ftp.Protocol = (FtpSecurityProtocol) ftpProtocolComboBox.SelectedIndex;

                if (!backButton.Enabled) // If the back-button was disabled, enabled it again
                    backButton.Enabled = true;

                _sender = statisticsServerTabPage;
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
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
                tablessTabControl1.SelectedTab = proxyTabPage;
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

                if (!_projectDataAlreadyInitialized)
                {
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
                        _sql.Password = sqlPassword;
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
                        bool isFirstEntry = true;
                        var builder = new StringBuilder();
                        foreach (var projectEntry in Program.ExisitingProjects)
                        {
                            if (isFirstEntry)
                            {
                                builder.Append(String.Format("{0}-{1}", projectEntry.Key, projectEntry.Value));
                                isFirstEntry = false;
                            }
                            else
                            {
                                builder.Append(String.Format("\n{0}-{1}", projectEntry.Key, projectEntry.Value));
                            }
                        }
                        File.WriteAllText(Program.ProjectsConfigFilePath, builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Failed to create the project-entry.", ex,
                            PopupButtons.Ok);
                        Close();
                    }

                    string phpFilePath = Path.Combine(Program.Path, "Projects", nameTextBox.Text, "statistics.php");
                    try
                    {
                        if (useStatisticsServerRadioButton.Checked)
                        {
                            File.WriteAllBytes(phpFilePath, Resources.statistics);

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
                        Close();
                    }
                }

                _projectDataAlreadyInitialized = true; // The data is now created.
                _generalTabPassed = true;

                SetUiState(false);
                if (useStatisticsServerRadioButton.Checked)
                    ThreadPool.QueueUserWorkItem(arg => InitializeData());
            }
        }

        /// <summary>
        ///     Provides a new thread that sets up the project.
        /// </summary>
        private void InitializeData()
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Initializing..."));

            // TODO: Proxy first


            Invoke(new Action(() =>
            {
                _name = nameTextBox.Text;
                _localPath = localPathTextBox.Text;
                _useStatistics = useStatisticsServerRadioButton.Checked;
            }));

            if (Project.Name != _name)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Initializing new name..."));

                var assumingProject =
                    Program.ExisitingProjects.First(item => item.Key == Project.Name);
                if (!assumingProject.Equals(default(KeyValuePair<string, string>))) // Key exists
                    Program.ExisitingProjects.Remove(assumingProject.Key);

                    Program.ExisitingProjects.Add(_name, _localPath);
                    _projectAddedToConfig = true;

                    Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Editing name in project configuration..."));

                try
                {
                    var projectEntries =
                        Program.ExisitingProjects.Select(
                            projectEntry => String.Format("{0}-{1}", projectEntry.Key, projectEntry.Value)).ToList();

                    File.WriteAllText(Program.ProjectsConfigFilePath, String.Join("\\", projectEntries));
                    _projectConfigurationEdited = true;
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while editing the project confiuration file.",
                        ex,
                        PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Renaming project directory..."));

                try
                {
                    Directory.Move(Path.Combine(Program.Path, "Projects", Project.Name),
                        Path.Combine(Program.Path, "Projects", _name));
                    _projectDirectoryMoved = true;
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while renaming the project directory.",
                        ex,
                        PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }
            }

            if (Project.Path != _localPath)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Moving project file..."));

                try
                {
                    File.Move(Project.Path, _localPath);
                    _projectFileMoved = true;
                }
                catch (IOException ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while moving the project file.", ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }
            }

            string updateUrl = null;
            Invoke(new Action(() => updateUrl = updateUrlTextBox.Text));
            if (Project.UpdateUrl != updateUrl)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Getting old configuration..."));

                try
                {
                    _oldUpdateConfiguration =
                        UpdateConfiguration.Download(UriConnector.ConnectUri(Project.UpdateUrl, "updates.json"), null);
                    _newUpdateConfiguration = _oldUpdateConfiguration;
                    // TODO: Proxy
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the old configuration.", ex,
                        PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Editing update-url in configuration..."));

                try
                {
                    _newUpdateConfiguration.ForEach(
                        item =>
                            item.UpdatePackageUrl =
                                UriConnector.ConnectUri(updateUrl, String.Format("{0}.zip", Project.Guid)));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while editing the update configuration's update url.", ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                if (Project.UseStatistics == _useStatistics) // We don't need the configuration later
                {
                    Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Uploading new configuration..."));

                    try
                    {
                        string localConfigurationPath = Path.Combine(Program.Path, "updates.json");
                        File.WriteAllText(localConfigurationPath, Serializer.Serialize(_oldUpdateConfiguration));
                        _updateConfigurationSaved = true;

                        _ftp.UploadFile(localConfigurationPath);
                        File.WriteAllText(localConfigurationPath, String.Empty);
                        _updateConfigurationSaved = false;
                        _updateUrlChanged = true;
                    }
                    catch (Exception ex)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while saving and uploading the new configuration.", ex, PopupButtons.Ok)));
                        Reset();
                        SetUiState(true);
                        return;
                    }
                }
            }

            string phpFilePath = Path.Combine(Program.Path, "Projects", _name, "statistics.php");
            if (!Project.UseStatistics && useStatisticsServerRadioButton.Checked)
            {
                /*
                 *  Setup the "statistics.php".
                */

                try
                {
                    if (File.Exists(phpFilePath))
                        File.Delete(phpFilePath);

                    // Create the file
                    File.WriteAllBytes(phpFilePath, Resources.statistics);

                    string phpFileContent = File.ReadAllText(phpFilePath);
                    phpFileContent = phpFileContent.Replace("_DBURL", _sql.WebUrl);
                    phpFileContent = phpFileContent.Replace("_DBUSER", _sql.Username);
                    phpFileContent = phpFileContent.Replace("_DBNAME", _sql.DatabaseName);
                    phpFileContent = phpFileContent.Replace("_DBPASS", sqlPasswordTextBox.Text);
                    File.WriteAllText(phpFilePath, phpFileContent);

                    _phpFileCreated = true;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while creating and editing the PHP-file.",
                                    ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                try
                {
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

                setupString = setupString.Replace("_DBNAME", _sql.DatabaseName);
                setupString = setupString.Replace("_APPNAME", _name);
                setupString = setupString.Replace("_APPID",
                    Project.ApplicationId.ToString(CultureInfo.InvariantCulture));

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
                    Reset();
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
                    Reset();
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
                    command.ExecuteNonQuery(); // Check unnecessary, last task
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
            else if (Project.UseStatistics && !_useStatistics)
            {
                try
                {
                    if (File.Exists(phpFilePath))
                        File.Delete(phpFilePath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while deleting the PHP-file.",
                                    ex, PopupButtons.Ok)));
                    Reset();
                    SetUiState(true);
                    return;
                }

                    try
                    {
                        _ftp.DeleteFile("statistics.php");
                        _phpFileDeleted = true;
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

                /*
                *  Setup the SQL-server and database.
                */

                #region "Setup-String"

                string setupString = @"USE _DBNAME;
DELETE FROM `Application` WHERE `ID` = _APPID;
DELETE FROM `Version` WHERE `Application_ID` = _APPID";
            
                #endregion

                setupString = setupString.Replace("_DBNAME", _sqlDatabaseName);
                setupString = setupString.Replace("_APPID",
                    Project.ApplicationId.ToString(CultureInfo.InvariantCulture));

                setupString = _oldUpdateConfiguration.Aggregate(setupString, (current, configuration) => current + String.Format("\nDELETE FROM `Download` WHERE `Version_ID` = {0}", configuration.VersionId));

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
                                                           "PASSWORD={3};", _sqlWebUrl, _sqlDatabaseName,
                            _sqlUsername, _sqlPassword);
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
                    Reset();
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
                    Reset();
                    SetUiState(true);
                    return;
                }
            }

            SetUiState(true);
            Invoke(new Action(Close));
        }

        private void backButton_Click(object sender, EventArgs e)
        {

        }

        private void ftpPortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {

        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {

        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {

        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {

        }

        private void selectServerButton_Click(object sender, EventArgs e)
        {

        }

        private void doNotUseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void Reset()
        {
            Invoke(
                new Action(
                    () => 
                        loadingLabel.Text = "Undoing changes..."));

            if (_projectConfigurationEdited)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Removing project configuration entry..."));

                try
                {
                    Program.ExisitingProjects.Remove(_name);
                    _projectConfigurationEdited = false;
                }
                catch (Exception ex)
                {
                    SetUiState(true);
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while resetting the project configuration.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    return;
                }
            }

            if (_projectDirectoryMoved)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Undoing the renaming of the project folder..."));

                try
                {
                    Directory.Move(Path.Combine(Program.Path, "Projects", _name),
                        Path.Combine(Program.Path, "Projects", Project.Name));
                    _projectDirectoryMoved = false;
                }
                catch (Exception ex)
                {
                    SetUiState(true);
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while undoing the renaming of the project folder.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    return;
                }
            }

            if (_projectFileMoved)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Undoing the renaming of the project file..."));

                try
                {
                    File.Move(_localPath, Project.Path);
                    _projectFileMoved = false;
                }
                catch (Exception ex)
                {
                    SetUiState(true);
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while undoing the renaming of the project file.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    return;
                }
            }

            if (_updateUrlChanged)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Saving the old update configuration..."));

                try
                {
                    string localConfigurationPath = Path.Combine(Program.Path, "updates.json");
                    File.WriteAllText(localConfigurationPath, Serializer.Serialize(_newUpdateConfiguration));

                    Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Uploading old configuration..."));
                    _ftp.UploadFile(localConfigurationPath);


                    File.WriteAllText(localConfigurationPath, String.Empty);
                    _updateUrlChanged = true;
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while saving and uploading the new configuration.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    SetUiState(true);
                    return;
                }
            }

            if (_phpFileCreated)
            {
                Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Deleting the file \"statistics.php\"..."));

                try
                {
                    File.Delete(Path.Combine(Program.Path, "Projects", _name, "statistics.php"));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while deleting the file \"statistics.php\" again.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    SetUiState(true);
                    return;   
                }
            }

            if (_phpFileUploaded)
            {
                Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text = "Deleting the file \"statistics.php\" on the server..."));

                try
                {
                    _ftp.DeleteFile("statistics.php");
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while deleting the file \"statistics.php\" on the server.",
                            ex, PopupButtons.Ok);
                        Close();
                    }));
                    SetUiState(true);
                    return;
                }   
            }
        }
    }
}