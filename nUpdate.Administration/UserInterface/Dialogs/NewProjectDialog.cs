// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.Http;
using nUpdate.Administration.Logging;
using nUpdate.Administration.Sql;
using nUpdate.Administration.UserInterface.Popups;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class NewProjectDialog : BaseDialog
    {
        private RsaManager _rsaManager;
        private ITransferData _transferData;
        private string _transferAssemblyFilePath;
        private ProxyData _proxyData;
        private SqlData _sqlData;
        private readonly BindingList<string> _parametersBindingList = new BindingList<string>();

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        private async void NewProjectDialog_Load(object sender, EventArgs e)
        {
            Text = String.Format(Text, Program.VersionString);
            protocolComboBox.SelectedIndex = 0;
            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;
            parametersListBox.DataSource = _parametersBindingList;
            localPathTextBox.ButtonClicked += BrowseLocalPathButtonClick;
            localPathTextBox.Initialize();
            phpOutputPathTextBox.ButtonClicked += PhpOutputPathButtonClick;
            phpOutputPathTextBox.Initialize();
            
            await GenerateKeyPair();
        }

        private Task GenerateKeyPair()
        {
            AllowCancel = false;
            return Task.Factory.StartNew(() =>
            {
                _rsaManager = new RsaManager();
                Invoke(new Action(() =>
                {
                    controlPanel1.Visible = true;
                    informationCategoriesTabControl.SelectedTab = generalTabPage;
                }));
                
                AllowCancel = true;
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

                if (!Uri.IsWellFormedUriString(updateDirectoryUriTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid URI specified.", "The specified update URI is invalid.",
                        PopupButtons.Ok);
                    updateDirectoryUriTextBox.Focus();
                    return;
                }

                if (!Path.IsPathRooted(localPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    localPathTextBox.Focus();
                    return;
                }

                if (Path.GetInvalidPathChars().Any(item => localPathTextBox.Text.Contains(item)))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid project file path.",
                        "The specified project file path contains invalid chars.", PopupButtons.Ok);
                    localPathTextBox.Focus();
                    return;
                }

                backButton.Enabled = true;

                switch (protocolComboBox.SelectedIndex)
                {
                    case 0:
                        informationCategoriesTabControl.SelectedTab = ftpTabPage;
                        break;
                    case 1:
                        informationCategoriesTabControl.SelectedTab = httpTabPage;
                        break;
                    case 2:
                        informationCategoriesTabControl.SelectedTab = customTabPage;
                        break;
                }
            }
            else if (informationCategoriesTabControl.SelectedTab == httpTabPage)
            {
                if (!ValidationManager.Validate(httpTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!Uri.IsWellFormedUriString(phpScriptUriTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid URI specified.", "The specified PHP-script URI is invalid.",
                        PopupButtons.Ok);
                    phpScriptUriTextBox.Focus();
                    return;
                }

                if (!Path.IsPathRooted(phpOutputPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The specified output path for the PHP-file is invalid.", PopupButtons.Ok);
                    phpOutputPathTextBox.Focus();
                    return;
                }

                if (Path.GetInvalidPathChars().Any(item => phpOutputPathTextBox.Text.Contains(item)))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid output file path.",
                        "The specified output path for the PHP-file contains invalid chars.", PopupButtons.Ok);
                    phpOutputPathTextBox.Focus();
                    return;
                }

                _transferData = new HttpData
                {
                    ScriptUri = new Uri(phpScriptUriTextBox.Text),
                    MustAuthenticate = authenticateCheckBox.Checked,
                    Username = httpUsernameTextBox.Text,
                    Password = authenticateCheckBox.Checked ? Convert.ToBase64String(AesManager.Encrypt(httpPasswordTextBox.Text, Program.AesKeyPassword, Program.AesIvPassword)) : String.Empty
                };
                
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == ftpTabPage)
            {
                if (!ValidationManager.Validate(ftpPanel) || String.IsNullOrEmpty(ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                _transferData = new FtpData
                {
                    Host = ftpHostTextBox.Text,
                    Port = int.Parse(ftpPortTextBox.Text),
                    Directory = ftpDirectoryTextBox.Text,
                    UsePassiveMode = ftpModeComboBox.SelectedIndex == 0,
                    FtpSpecificProtocol = (FtpsSecurityProtocol)ftpProtocolComboBox.SelectedIndex,
                    Username = ftpUserTextBox.Text,
                    Password = Convert.ToBase64String(AesManager.Encrypt(ftpPasswordTextBox.Text, Program.AesKeyPassword, Program.AesIvPassword))
                };

                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == customTabPage)
            {
                if (!ValidationManager.Validate(customTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                _transferAssemblyFilePath = transferAssemblyFilePathTextBox.Text;
                informationCategoriesTabControl.SelectedTab = statisticsServerTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == statisticsServerTabPage)
            {
                informationCategoriesTabControl.SelectedTab = proxyTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == proxyTabPage)
            {
                if (useProxyRadioButton.Checked)
                {
                    if (!ValidationManager.Validate(proxyTabPage) && !String.IsNullOrEmpty(proxyUserTextBox.Text) &&
                        !String.IsNullOrEmpty(proxyPasswordTextBox.Text))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                            "All fields need to have a value.", PopupButtons.Ok);
                        return;
                    }
                }

                AdjustControlsForAction(async () => await Initialize(), true);
                Close();
            }
        }

        private async Task GenerateScriptFile()
        {
            var script = await ResourceHelper.GetEmbeddedResource("upload.php");
            script.Replace("$URL", phpScriptUriTextBox.Text);
            if (authenticateCheckBox.Checked)
            {
                script.Replace("$USERNAME", httpUsernameTextBox.Text);
                script.Replace("$PASSWORD", httpPasswordTextBox.Text);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(script);
            using (var stream = new FileStream(phpOutputPathTextBox.Text, FileMode.OpenOrCreate))
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
            }
        }

        private async Task Initialize()
        {
            loadingLabel.Text = "Initializing project...";
            var project = new UpdateProject
            {
                Guid = Guid.NewGuid(),
                LogData = Enumerable.Empty<PackageActionLogData>().ToList(),
                Name = nameTextBox.Text,
                Packages = Enumerable.Empty<UpdatePackage>().ToList(),
                PrivateKey = _rsaManager.PrivateKey,
                PublicKey = _rsaManager.PublicKey,
                TransferProtocol = (TransferProtocol) protocolComboBox.SelectedIndex,
                TransferData = _transferData,
                UpdateDirectoryUri = new Uri(updateDirectoryUriTextBox.Text),
                UseProxy = useProxyRadioButton.Checked,
                UseStatistics = useStatisticsServerRadioButton.Checked,
                ProxyData = _proxyData,
                SqlData = _sqlData,
                TransferAssemblyFilePath = _transferAssemblyFilePath
            };

            loadingLabel.Text = "Creating project file...";
            try
            {
                project.Save(localPathTextBox.Text);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Creating the project file failed.", ex, PopupButtons.Ok);
                return;
            }

            try
            {
                Session.AvailableLocations.Add(new UpdateProjectLocation(project.Guid, localPathTextBox.Text));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Creating an entry for the project location failed.", ex,
                    PopupButtons.Ok);
                return;
            }

            if (protocolComboBox.SelectedIndex == 1)
            {
                await GenerateScriptFile();
                Popup.ShowPopup(this, SystemIcons.Information, "PHP-script exported.", $"The PHP-script that will be used for data transfers has been successfully created. In order to make it usable, please upload it onto your server, so that it is reachable at the specified URI: {phpScriptUriTextBox.Text}",
                    PopupButtons.Ok);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (informationCategoriesTabControl.SelectedTab == ftpTabPage || informationCategoriesTabControl.SelectedTab == httpTabPage || informationCategoriesTabControl.SelectedTab == customTabPage)
            {
                backButton.Enabled = false;
                informationCategoriesTabControl.SelectedTab = generalTabPage;
            }
            else if (informationCategoriesTabControl.SelectedTab == statisticsServerTabPage)
            {
                switch (protocolComboBox.SelectedIndex)
                {
                    case 0:
                        informationCategoriesTabControl.SelectedTab = ftpTabPage;
                        break;
                    case 1:
                        informationCategoriesTabControl.SelectedTab = httpTabPage;
                        break;
                    case 2:
                        informationCategoriesTabControl.SelectedTab = customTabPage;
                        break;
                }
            }
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
            if (!ValidationManager.ValidateDialogWithIgnoring(ftpPanel, new [] { ftpDirectoryTextBox }))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.", PopupButtons.Ok);
                return;
            }

            var transferData = new FtpData
            {
                Host = ftpHostTextBox.Text,
                Port = int.Parse(ftpPortTextBox.Text),
                UsePassiveMode = ftpModeComboBox.SelectedIndex == 0,
                FtpSpecificProtocol = (FtpsSecurityProtocol)ftpProtocolComboBox.SelectedIndex,
                Username = ftpUserTextBox.Text,
                Password = Convert.ToBase64String(AesManager.Encrypt(ftpPasswordTextBox.Text, Program.AesKeyPassword, Program.AesIvPassword))
            };

            var searchDialog = new DirectorySearchDialog(new TransferManager(TransferProtocol.FTP, transferData), nameTextBox.Text);
            if (searchDialog.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchDialog.SelectedDirectory;
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

        private void PhpOutputPathButtonClick(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "PHP-script files (*.php)|*.php";
                fileDialog.CheckFileExists = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    phpOutputPathTextBox.Text = fileDialog.FileName;
            }
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
                    var importProject = UpdateProject.Load(fileDialog.FileName);
                    if (importProject.TransferProtocol != TransferProtocol.FTP)
                    {
                        Popup.ShowPopup(this, SystemIcons.Warning, "Importing canceled.", "The specified project does not contain transfer data that conforms to the FTP-specification. It uses another protocol instead.",
                            PopupButtons.Ok);
                    }

                    var ftpData = (FtpData) importProject.TransferData;
                    ftpHostTextBox.Text = ftpData.Host;
                    ftpPortTextBox.Text = ftpData.Port.ToString(CultureInfo.InvariantCulture);
                    ftpUserTextBox.Text = ftpData.Username;
                    ftpProtocolComboBox.SelectedIndex = (int)ftpData.FtpSpecificProtocol;
                    ftpModeComboBox.SelectedIndex = ftpData.UsePassiveMode ? 0 : 1;
                    ftpDirectoryTextBox.Text = ftpData.Directory;
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
            var statisticsServerDialog = new StatisticalServerDialog { ReactsOnKeyDown = true };
            if (statisticsServerDialog.ShowDialog() != DialogResult.OK)
                return;

            _sqlData.DatabaseName = statisticsServerDialog.DatabaseName;
            _sqlData.WebUri = statisticsServerDialog.WebUri;
            _sqlData.Username = statisticsServerDialog.Username;
            databaseNameLabel.Text = statisticsServerDialog.DatabaseName;
        }

        private void doNotUseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            proxyPanel.Enabled = !doNotUseProxyRadioButton.Checked;
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

        private void authenticateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            httpAuthenticatePanel.Enabled = authenticateCheckBox.Checked;
        }
    }
}