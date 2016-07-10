// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Windows.Forms;
using nUpdate.Administration.UserInterface.Popups;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class ProjectEditDialog : BaseDialog
    {
        private bool _allowCancel = true;
        private TransferManager _ftp;
        private string _ftpAssemblyPath;
        private bool _ftpDirectoryChanged;
        private bool _generalTabPassed;
        private bool _isSetByUser;
        private int _lastSelectedIndex;
        private string _localPath;
        //private LocalizationProperties _lp = new LocalizationProperties();
        private string _name;
        private IEnumerable<UpdatePackage> _newUpdateConfiguration;
        private IEnumerable<UpdatePackage> _oldUpdateConfiguration;
        private IEnumerable<UpdateVersion> _packageVersionsToAffect;
        private bool _phpFileCreated;
        private bool _phpFileDeleted;
        private bool _phpFileOnlineDeleted;
        private bool _phpFileUploaded;
        private WebProxy _proxy;
        private TabPage _sender;
        private bool _sqlDataDeleted;
        private bool _sqlDataInitialized;
        private bool _updateConfigurationSaved;
        private Uri _updateUri;
        private bool _updateUrlChanged;
        private bool _useStatistics;

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
        private void ProjectEditDialog_Load(object sender, EventArgs e)
        {
            if (!WebConnection.IsAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection was found. In order to edit a project a network connection is required in order to communicate with the server.",
                    PopupButtons.Ok);
                Close();
                return;
            }

            Text = string.Format(Text, Session.ActiveProject.Name, Program.VersionString);
            ftpPortTextBox.ShortcutsEnabled = false;
            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;

            nameTextBox.Text = Session.ActiveProject.Name;
            updateUriTextBox.Text = Session.ActiveProject.UpdateDirectoryUri.ToString();

            localPathTextBox.ButtonClicked += BrowsePathButtonClick;
            localPathTextBox.Text = Session.ProjectFilePath;
            localPathTextBox.Initialize();
            
            _sender = generalTabPage;
            _isSetByUser = true;
        }

        private void ProjectEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
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

                if (nameTextBox.Text != Path.GetFileNameWithoutExtension(localPathTextBox.Text))
                {
                    if (Popup.ShowPopup(this, SystemIcons.Warning,
                        "Possible incomplete change of project-data found.",
                        "The current name and local path don't fit in with each other. Are you sure that you want to continue?",
                        PopupButtons.YesNo) == DialogResult.No)
                        return;
                }

                if (!_generalTabPassed)
                {
                    //_projectConfiguration =
                    //    ProjectConfiguration.Load().ToList();
                    //if (_projectConfiguration != null)
                    //{
                    //    if (_projectConfiguration.Any(item => item.Name == nameTextBox.Text) &&
                    //        Session.ActiveProject.Name != nameTextBox.Text)
                    //    {
                    //        Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                    //            $"The project \"{nameTextBox.Text}\" is already existing. Please choose another name for it.", PopupButtons.Ok);
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    _projectConfiguration = new List<ProjectConfiguration>();
                    //}
                }

                if (!Uri.IsWellFormedUriString(updateUriTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid address.", "The given Update-URL is invalid.",
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

                if (Path.GetInvalidFileNameChars().Any(item => localPathTextBox.Text.Contains(item)))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid project name.",
                        "The given project file path contains invalid chars.", PopupButtons.Ok);
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
                if (!ValidationManager.Validate(ftpPanel) || string.IsNullOrEmpty(ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }
                
                if (!backButton.Enabled) // If the back-button was disabled, enable it again
                    backButton.Enabled = true;

                if (!ftpDirectoryTextBox.Text.StartsWith("/"))
                    ftpDirectoryTextBox.Text = $"/{ftpDirectoryTextBox.Text}";

                //if (ftpDirectoryTextBox.Text != Project.FtpDirectory && updateUriTextBox.Text == Project.UpdateUri.ToString())
                //{
                //    if (Popup.ShowPopup(this, SystemIcons.Warning,
                //        "Possible incomplete change of project-data found.",
                //        "The current update-URL hasn't changed although the FTP-directory has. Are you sure that you want to continue?",
                //        PopupButtons.YesNo) == DialogResult.No)
                //        return;
                //}

                _sender = statisticsServerTabPage;
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
            }
            //else if (_sender == statisticsServerTabPage)
            //{
            //    if (useStatisticsServerRadioButton.Checked)
            //    {
            //        if ((Project.SqlDatabaseName == null && SqlDatabaseName == null) ||
            //            string.IsNullOrWhiteSpace(sqlPasswordTextBox.Text))
            //        {
            //            Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
            //                "All fields need to have a value.", PopupButtons.Ok);
            //            return;
            //        }
            //    }

            //    _sender = proxyTabPage;
            //    tablessTabControl1.SelectedTab = proxyTabPage;
            //}
            else if (_sender == proxyTabPage)
            {
                if (useProxyRadioButton.Checked)
                {
                    if (!ValidationManager.ValidateTabPage(proxyTabPage) && !string.IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !string.IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                        return;
                    }
                }

                _generalTabPassed = true;
                InitializeData();
            }
        }

        /// <summary>
        ///     Provides a new thread that sets up the project.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:SQL-Abfragen auf Sicherheitsrisiken überprüfen")]
        private void InitializeData()
        {
//            await Task.Factory.StartNew(() =>
//            {
//                SetUserInterfaceState(false);
//                Invoke(
//                    new Action(
//                        () =>
//                            loadingLabel.Text = "Testing connection to the FTP-server..."));

//                try
//                {
//                    _ftp.TestConnection();
//                }
//                catch (FtpAuthenticationException ex)
//                {
//                    Invoke(
//                        new Action(
//                            () =>
//                                Popup.ShowPopup(this, SystemIcons.Error, "Error while authenticating the certificate.",
//                                    ex.InnerException ?? ex, PopupButtons.Ok)));
//                    SetUserInterfaceState(true);
//                    return;
//                }
//                catch (Exception ex)
//                {
//                    Invoke(
//                        new Action(
//                            () =>
//                                Popup.ShowPopup(this, SystemIcons.Error, "Error while testing the FTP-data.",
//                                    ex.InnerException ?? ex, PopupButtons.Ok)));
//                    SetUserInterfaceState(true);
//                    return;
//                }
//                Invoke(new Action(() =>
//                {
//                    if (useProxyRadioButton.Checked)
//                    {
//                        _proxy = new WebProxy(proxyHostTextBox.Text);
//                        if (!string.IsNullOrEmpty(proxyPasswordTextBox.Text))
//                            _proxy.Credentials = new NetworkCredential(proxyUserTextBox.Text, proxyPasswordTextBox.Text);
//                        else
//                            _proxy.UseDefaultCredentials = true;
//                    }
//                }));

//                //_ftp.Proxy = _proxy;

//                Invoke(new Action(() =>
//                {
//                    _name = nameTextBox.Text;
//                    _localPath = localPathTextBox.Text;
//                    _updateUri = new Uri(updateUriTextBox.Text);
//                    _useStatistics = useStatisticsServerRadioButton.Checked;
//                }));

//                //if (Project.Name != _name)
//                //{
//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Initializing new name..."));

//                    var projectConfiguration =
//                        Serializer.Deserialize<List<ProjectConfiguration>>(
//                            File.ReadAllText(Program.ProjectsConfigFilePath));
//                    projectConfiguration[projectConfiguration.FindIndex(item => item.Name == Project.Name)] =
//                        new ProjectConfiguration(_name, _localPath);

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Editing name in project configuration..."));

//                    try
//                    {
//                        File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(projectConfiguration));
//                        Session.ActiveProjectConfigurationEdited = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                            "Error while editing the project confiuration file.",
//                            ex,
//                            PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    var projectDirectory = Path.Combine(Program.Path, "Projects", Project.Name);
//                    if (Directory.Exists(projectDirectory))
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Renaming project directory..."));

//                        try
//                        {
//                            Directory.Move(projectDirectory, Path.Combine(Program.Path, "Projects", _name));
//                            Session.ActiveProjectDirectoryMoved = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                                "Error while renaming the project directory.",
//                                ex,
//                                PopupButtons.Ok)));
//                            Reset();
//                            return;
//                        }
//                    }
//                //}

//                if (Project.Path != _localPath)
//                {
//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Moving project file..."));

//                    try
//                    {
//                        File.Move(Project.Path, _localPath);
//                        Session.ActiveProjectFileMoved = true;
//                    }
//                    catch (IOException ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while moving the project file.", ex,
//                                        PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }
//                }

//                string ftpDirectory = null;
//                Invoke(new Action(() => ftpDirectory = ftpDirectoryTextBox.Text));
//                if (Project.FtpDirectory != ftpDirectory &&
//                    Project.FtpDirectory != ftpDirectory.Remove(ftpDirectory.Length - 1) &&
//                    Project.FtpDirectory.Substring(1) != ftpDirectory)
//                {
//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text =
//                                    $"Moving old directory content to \"{ftpDirectory}\"..."));

//                    try
//                    {
//                        _ftpDirectoryChanged = true;
//                        _ftp.MoveContent(ftpDirectory);
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while moving the directory content.",
//                                        ex,
//                                        PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }
//                }

//                if (Project.UpdateUri != _updateUri)
//                {
//                    if (_newUpdateConfiguration == null && !LoadConfiguration())
//                    {
//                        Reset();
//                        return;
//                    }

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Editing update-url in configuration..."));

//                    try
//                    {
//                        _newUpdateConfiguration.ForEach(
//                            item =>
//                            {
//                                item.UpdatePackageUri = new Uri(_updateUri, $"{Project.Guid}.zip");
//                                item.UpdatePhpFileUri = new Uri(_updateUri, "statistics.php");
//                            });
//                        _updateUrlChanged = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                            "Error while editing the update configuration's update url.", ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    if (Project.UseStatistics == _useStatistics) // We don't need the configuration later
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Uploading new configuration..."));

//                        try
//                        {
//                            var localConfigurationPath = Path.Combine(Program.Path, "updates.json");
//                            File.WriteAllText(localConfigurationPath, Serializer.Serialize(_newUpdateConfiguration));
//                            _updateConfigurationSaved = true;

//                            _ftp.UploadFile(localConfigurationPath);
//                            File.WriteAllText(localConfigurationPath, string.Empty);
//                            _updateConfigurationSaved = false;
//                            _updateUrlChanged = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                                "Error while saving and uploading the new configuration.", ex, PopupButtons.Ok)));
//                            Reset();
//                            return;
//                        }
//                    }
//                }

//                var phpFilePath = Path.Combine(Program.Path, "Projects", _name, "statistics.php");
//                if (!Project.UseStatistics && useStatisticsServerRadioButton.Checked)
//                {
//                    if (_newUpdateConfiguration == null && !LoadConfiguration())
//                    {
//                        Reset();
//                        return;
//                    }

//                    /*
//                    *  Setup the "statistics.php".
//                    */

//                    try
//                    {
//                        if (File.Exists(phpFilePath))
//                            File.Delete(phpFilePath);

//                        // Create the file
//                        File.WriteAllBytes(phpFilePath, Resources.statistics);

//                        var phpFileContent = File.ReadAllText(phpFilePath);
//                        phpFileContent = phpFileContent.Replace("_DBURL", SqlWebUrl);
//                        phpFileContent = phpFileContent.Replace("_DBUSER", SqlUsername);
//                        phpFileContent = phpFileContent.Replace("_DBNAME", SqlDatabaseName);
//                        Invoke(
//                            new Action(
//                                () =>
//                                    phpFileContent = phpFileContent.Replace("_DBPASS", sqlPasswordTextBox.Text)));
//                        File.WriteAllText(phpFilePath, phpFileContent);

//                        _phpFileCreated = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error,
//                                        "Error while creating and editing the PHP-file.",
//                                        ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    try
//                    {
//                        _ftp.UploadFile(phpFilePath);
//                        _phpFileUploaded = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the PHP-file.",
//                                        ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    /*
//                *  Setup the SQL-data.
//                */

//                    #region "Setup-String"

//                    var setupString = @"CREATE DATABASE IF NOT EXISTS _DBNAME;
//USE _DBNAME;

//CREATE TABLE IF NOT EXISTS `_DBNAME`.`Application` (
//  `ID` INT NOT NULL,
//  `Name` VARCHAR(200) NOT NULL,
//  PRIMARY KEY (`ID`))
//ENGINE = InnoDB;

//CREATE TABLE IF NOT EXISTS `_DBNAME`.`Version` (
//  `ID` INT NOT NULL,
//  `Version` VARCHAR(40) NOT NULL,
//  `Application_ID` INT NOT NULL,
//  PRIMARY KEY (`ID`),
//  INDEX `fk_Version_Application_idx` (`Application_ID` ASC),
//  CONSTRAINT `fk_Version_Application`
//    FOREIGN KEY (`Application_ID`)
//    REFERENCES `_DBNAME`.`Application` (`ID`)
//    ON DELETE NO ACTION
//    ON UPDATE NO ACTION)
//ENGINE = InnoDB;

//CREATE TABLE IF NOT EXISTS `_DBNAME`.`Download` (
//  `ID` INT NOT NULL AUTO_INCREMENT,
//  `Version_ID` INT NOT NULL,
//  `DownloadDate` DATETIME NOT NULL,
//  `OperatingSystem` VARCHAR(20) NOT NULL,
//  PRIMARY KEY (`ID`),
//  INDEX `fk_Download_Version1_idx` (`Version_ID` ASC),
//  CONSTRAINT `fk_Download_Version1`
//    FOREIGN KEY (`Version_ID`)
//    REFERENCES `_DBNAME`.`Version` (`ID`)
//    ON DELETE NO ACTION
//    ON UPDATE NO ACTION)
//ENGINE = InnoDB;

//INSERT INTO Application (`ID`, `Name`) VALUES (_APPID, '_APPNAME');";

//                    #endregion

//                    setupString = setupString.Replace("_DBNAME", SqlDatabaseName);
//                    setupString = setupString.Replace("_APPNAME", _name);
//                    setupString = setupString.Replace("_APPID",
//                        Project.ApplicationId.ToString(CultureInfo.InvariantCulture));

//                    if (_newUpdateConfiguration != null)
//                    {
//                        foreach (
//                            var updateConfig in
//                                _newUpdateConfiguration.Where(
//                                    updateConfig =>
//                                        _packageVersionsToAffect.Any(
//                                            item => item.ToString() == updateConfig.LiteralVersion))
//                            )
//                        {
//                            updateConfig.UseStatistics = true;
//                            setupString +=
//                                $"\nINSERT INTO Version (`ID`, `Version`, `Application_ID`) VALUES ({updateConfig.VersionId}, \"{updateConfig.LiteralVersion}\", {Project.ApplicationId});";
//                        }

//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Uploading new configuration..."));

//                        try
//                        {
//                            var localConfigurationPath = Path.Combine(Program.Path, "updates.json");
//                            File.WriteAllText(localConfigurationPath, Serializer.Serialize(_newUpdateConfiguration));
//                            _updateConfigurationSaved = true;

//                            _ftp.UploadFile(localConfigurationPath);
//                            File.WriteAllText(localConfigurationPath, string.Empty);
//                            _updateConfigurationSaved = false;
//                            _updateUrlChanged = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                                "Error while saving and uploading the new configuration.", ex, PopupButtons.Ok)));
//                            Reset();
//                            return;
//                        }
//                    }

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Connecting to SQL-server..."));

//                    string myConnectionString = null;
//                    Invoke(new Action(() =>
//                    {
//                        myConnectionString = $"SERVER={SqlWebUrl};" + $"DATABASE={SqlDatabaseName};" +
//                                             $"UID={SqlUsername};" + $"PASSWORD={sqlPasswordTextBox.Text};";
//                    }));

//                    var myConnection = new MySqlConnection(myConnectionString);
//                    try
//                    {
//                        myConnection.Open();
//                    }
//                    catch (MySqlException ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
//                                        ex, PopupButtons.Ok)));
//                        myConnection.Close();
//                        Reset();
//                        return;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
//                                        ex, PopupButtons.Ok)));
//                        myConnection.Close();
//                        Reset();
//                        return;
//                    }

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Executing setup commands..."));

//                    var command = myConnection.CreateCommand();
//                    command.CommandText = setupString;

//                    try
//                    {
//                        command.ExecuteNonQuery();
//                        _sqlDataInitialized = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
//                                        ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }
//                    finally
//                    {
//                        myConnection.Close();
//                        command.Dispose();
//                    }
//                }
//                else if (Project.UseStatistics && !_useStatistics)
//                {
//                    if (_newUpdateConfiguration == null && !LoadConfiguration())
//                    {
//                        Reset();
//                        return;
//                    }

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Deleting file \"statistics.php\"..."));

//                    try
//                    {
//                        if (File.Exists(phpFilePath))
//                            File.Delete(phpFilePath);
//                        _phpFileDeleted = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error,
//                                        "Error while deleting the PHP-file.",
//                                        ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    Invoke(
//                        new Action(
//                            () =>
//                                loadingLabel.Text = "Deleting file \"statistics.php\" on the server..."));

//                    try
//                    {
//                        if (_ftp.IsExisting("statistics.php"))
//                            _ftp.DeleteFile("statistics.php");
//                        _phpFileOnlineDeleted = true;
//                    }
//                    catch (Exception ex)
//                    {
//                        Invoke(
//                            new Action(
//                                () =>
//                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the PHP-file.",
//                                        ex, PopupButtons.Ok)));
//                        Reset();
//                        return;
//                    }

//                    if (_newUpdateConfiguration != null)
//                    {
//                        foreach (var updateConfig in _newUpdateConfiguration)
//                        {
//                            updateConfig.UseStatistics = false;
//                        }

//                        /*
//                        *  Setup the SQL-server and database.
//                        */

//                        var setupString = @"USE _DBNAME;";
//                        setupString = _newUpdateConfiguration.Aggregate(setupString,
//                            (current, configuration) =>
//                                current +
//                                $"DELETE FROM Download WHERE `Version_ID` = {configuration.VersionId};");

//                        setupString += @"DELETE FROM Version WHERE `Application_ID` = _APPID;
//DELETE FROM Application WHERE `ID` = _APPID;";
//                        setupString = setupString.Replace("_DBNAME", Project.SqlDatabaseName);
//                        setupString = setupString.Replace("_APPID",
//                            Project.ApplicationId.ToString(CultureInfo.InvariantCulture));

//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Uploading new configuration..."));

//                        try
//                        {
//                            var localConfigurationPath = Path.Combine(Program.Path, "updates.json");
//                            File.WriteAllText(localConfigurationPath, Serializer.Serialize(_newUpdateConfiguration));
//                            _updateConfigurationSaved = true;

//                            _ftp.UploadFile(localConfigurationPath);
//                            File.WriteAllText(localConfigurationPath, string.Empty);
//                            _updateConfigurationSaved = false;
//                            _updateUrlChanged = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error,
//                                "Error while saving and uploading the new configuration.", ex, PopupButtons.Ok)));
//                            Reset();
//                            return;
//                        }

//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Connecting to SQL-server..."));

//                        MySqlConnection myConnection = null;
//                        try
//                        {
//                            string myConnectionString = null;
//                            Invoke(new Action(() =>
//                            {
//                                myConnectionString = $"SERVER={Project.SqlWebUrl};" +
//                                                     $"DATABASE={Project.SqlDatabaseName};" +
//                                                     $"UID={Project.SqlUsername};" +
//                                                     $"PASSWORD={Project.RuntimeSqlPassword.ConvertToInsecureString()};";
//                            }));

//                            myConnection = new MySqlConnection(myConnectionString);
//                            myConnection.Open();
//                        }
//                        catch (MySqlException ex)
//                        {
//                            Invoke(
//                                new Action(
//                                    () =>
//                                        Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
//                                            ex, PopupButtons.Ok)));
//                            myConnection?.Close();
//                            Reset();
//                            return;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(
//                                new Action(
//                                    () =>
//                                        Popup.ShowPopup(this, SystemIcons.Error,
//                                            "Error while connecting to the database.",
//                                            ex, PopupButtons.Ok)));
//                            myConnection?.Close();
//                            Reset();
//                            return;
//                        }

//                        Invoke(
//                            new Action(
//                                () =>
//                                    loadingLabel.Text = "Executing setup commands..."));

//                        var command = myConnection.CreateCommand();
//                        command.CommandText = setupString;

//                        try
//                        {
//                            command.ExecuteNonQuery();
//                            _sqlDataDeleted = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            Invoke(
//                                new Action(
//                                    () =>
//                                        Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
//                                            ex, PopupButtons.Ok)));
//                            Reset();
//                            return;
//                        }
//                        finally
//                        {
//                            myConnection.Close();
//                            command.Dispose();
//                        }
//                    }
//                }

//                Project.Name = _name;
//                Project.Path = _localPath;
//                Project.UpdateUri = _updateUri;
//                Invoke(new Action(() =>
//                {
//                    Project.SaveCredentials = saveCredentialsCheckBox.Checked;
//                    Project.FtpHost = ftpHostTextBox.Text;
//                    Project.FtpPort = int.Parse(ftpPortTextBox.Text);
//                    Project.FtpUsername = ftpUserTextBox.Text;
//                    if (!saveCredentialsCheckBox.Checked)
//                        Project.FtpPassword = Convert.ToBase64String(AESManager.Encrypt(ftpPasswordTextBox.Text,
//                            ftpPasswordTextBox.Text,
//                            ftpUserTextBox.Text));
//                    else
//                        Project.FtpPassword =
//                            Convert.ToBase64String(AESManager.Encrypt(ftpPasswordTextBox.Text, Program.AesKeyPassword,
//                                Program.AesIvPassword));
//                    Project.FtpUsePassiveMode = ftpModeComboBox.SelectedIndex == 0;
//                    Project.FtpProtocol = ftpProtocolComboBox.SelectedIndex;
//                    Project.FtpDirectory = ftpDirectoryTextBox.Text;
//                    Project.FtpTransferAssemblyFilePath = ftpProtocolComboBox.SelectedIndex ==
//                                                          ftpProtocolComboBox.Items.Count - 1
//                        ? _ftpAssemblyPath
//                        : null;
//                    if (useProxyRadioButton.Checked)
//                    {
//                        Project.Proxy = new WebProxy(new Uri(proxyHostTextBox.Text));
//                        if (!string.IsNullOrEmpty(proxyPasswordTextBox.Text))
//                        {
//                            Project.ProxyUsername = proxyUserTextBox.Text;
//                            if (!saveCredentialsCheckBox.Checked)
//                                Project.ProxyPassword =
//                                    Convert.ToBase64String(AESManager.Encrypt(proxyPasswordTextBox.Text,
//                                        ftpPasswordTextBox.Text,
//                                        ftpUserTextBox.Text));
//                            else
//                                Project.ProxyPassword =
//                                    Convert.ToBase64String(AESManager.Encrypt(proxyPasswordTextBox.Text,
//                                        Program.AesKeyPassword,
//                                        Program.AesIvPassword));
//                        }
//                        else
//                        {
//                            Project.ProxyUsername = null;
//                            Project.ProxyPassword = null;
//                        }
//                    }
//                    else
//                    {
//                        Project.Proxy = null;
//                        Project.ProxyUsername = null;
//                        Project.ProxyPassword = null;
//                    }

//                    if (useStatisticsServerRadioButton.Checked)
//                    {
//                        Project.UseStatistics = true;
//                        Project.SqlDatabaseName = SqlDatabaseName ?? Project.SqlDatabaseName;
//                        Project.SqlWebUrl = SqlWebUrl ?? Project.SqlWebUrl;
//                        Project.SqlUsername = SqlUsername ?? Project.SqlUsername;
//                        if (!saveCredentialsCheckBox.Checked)
//                            Project.SqlPassword = Convert.ToBase64String(AESManager.Encrypt(sqlPasswordTextBox.Text,
//                                ftpPasswordTextBox.Text,
//                                ftpUserTextBox.Text));
//                        else
//                            Project.SqlPassword =
//                                Convert.ToBase64String(AESManager.Encrypt(sqlPasswordTextBox.Text,
//                                    Program.AesKeyPassword,
//                                    Program.AesIvPassword));
//                    }
//                    else
//                    {
//                        Project.UseStatistics = false;
//                        Project.SqlDatabaseName = null;
//                        Project.SqlWebUrl = null;
//                        Project.SqlUsername = null;
//                        Project.SqlPassword = null;
//                    }
//                }));

//                try
//                {
//                    Project.Save();
//                }
//                catch (Exception ex)
//                {
//                    Invoke(
//                        new Action(
//                            () =>
//                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new project-data.",
//                                    ex, PopupButtons.Ok)));
//                    Reset();
//                    return;
//                }

//                SetUserInterfaceState(true);
//                Invoke(new Action(Close));
//            });
        }

        private bool LoadConfiguration()
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Getting old configuration..."));

            try
            {
                //_oldUpdateConfiguration =
                //    UpdateData.Download(new Uri(_updateUri, "updates.json"), Project.Proxy);
                //if (_oldUpdateConfiguration != null)
                //    _newUpdateConfiguration = _oldUpdateConfiguration.ToArray();
                //else
                //{
                //    _oldUpdateConfiguration = new List<UpdateData>();
                //    _newUpdateConfiguration = new List<UpdateData>();
                //}

                return true;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error,
                                "Error while downloading the old configuration.", ex,
                                PopupButtons.Ok)));
                return false;
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (_sender == ftpTabPage)
            {
                tablessTabControl1.SelectedTab = generalTabPage;
                backButton.Enabled = false;
                _sender = generalTabPage;

                if (_generalTabPassed)
                    backButton.Enabled = false;
            }
            else if (_sender == statisticsServerTabPage)
            {
                tablessTabControl1.SelectedTab = ftpTabPage;
                _sender = ftpTabPage;
            }
            else if (_sender == proxyTabPage)
            {
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
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
            if (!ValidationManager.ValidateDialogWithIgnoring(ftpPanel, new [] { ftpDirectoryTextBox }))
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

            var searchDialog = new DirectorySearchDialog(Session.TransferManager, nameTextBox.Text);
            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;
            searchDialog.Close();
        }

        private void BrowsePathButtonClick(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj",
                CheckFileExists = false
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
                localPathTextBox.Text = fileDialog.FileName;
        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Management of sensible data.",
                "All your passwords will be encrypted with AES 256. The key and initializing vector are your FTP-username and password, so you have to enter them each time you open the project.",
                PopupButtons.Ok);
        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSetByUser)
            {
                var packagesToAffectDialog = new PackagesToAffectDialog();
                //if (!_useStatistics && useStatisticsServerRadioButton.Checked &&
                //    (Project.Packages != null && Project.Packages.Count != 0))
                //{
                //    foreach (var existingPackage in Project.Packages)
                //    {
                //        packagesToAffectDialog.AvailablePackageVersions.Add(new UpdateVersion(existingPackage.Version));
                //    }

                //    if (packagesToAffectDialog.ShowDialog() != DialogResult.OK)
                //        return;
                //}

                _packageVersionsToAffect = packagesToAffectDialog.PackageVersionsToAffect;
            }

            statisticsInfoPanel.Enabled = useStatisticsServerRadioButton.Checked;
            selectServerButton.Enabled = useStatisticsServerRadioButton.Checked;
        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {
            //using (var fileDialog = new OpenFileDialog())
            //{
            //    fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
            //    fileDialog.Multiselect = false;
            //    if (fileDialog.ShowDialog() != DialogResult.OK)
            //        return;

            //    try
            //    {
            //        var importProject = UpdateProject.LoadProject(fileDialog.FileName);
            //        ftpHostTextBox.Text = importProject.FtpHost;
            //        ftpPortTextBox.Text = importProject.FtpPort.ToString(CultureInfo.InvariantCulture);
            //        ftpUserTextBox.Text = importProject.FtpUsername;
            //        ftpProtocolComboBox.SelectedIndex = importProject.FtpProtocol;
            //        ftpModeComboBox.SelectedIndex = importProject.FtpUsePassiveMode ? 0 : 1;
            //        ftpDirectoryTextBox.Text = importProject.FtpDirectory;
            //        ftpPasswordTextBox.Focus();
            //    }
            //    catch (Exception ex)
            //    {
            //        Popup.ShowPopup(this, SystemIcons.Error, "Error while importing project data.", ex,
            //            PopupButtons.Ok);
            //    }
            //}
        }

        private void selectServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerDialog = new StatisticalServerDialog { ReactsOnKeyDown = true };
            if (statisticsServerDialog.ShowDialog() != DialogResult.OK)
                return;

            SqlDatabaseName = statisticsServerDialog.DatabaseName;
            SqlWebUrl = statisticsServerDialog.WebUri.ToString();
            SqlUsername = statisticsServerDialog.Username;
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
    }
}