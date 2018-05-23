// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FtpAssemblyInputDialog : BaseDialog
    {
        public FtpAssemblyInputDialog()
        {
            InitializeComponent();
        }

        public string AssemblyPath
        {
            get => assemblyFilePathTextBox.Text;
            set => assemblyFilePathTextBox.Text = value;
        }

        private void assemblyFilePathTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Executable files (*.exe)|*.exe|Dynamic link libraries (*.dll)|*.dll";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                assemblyFilePathTextBox.Text = fileDialog.FileName;
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "All fields need to have a value.", PopupButtons.Ok);
                return;
            }

            try
            {
                var assembly = Assembly.LoadFrom(assemblyFilePathTextBox.Text);
                if (ServiceProviderHelper.CreateServiceProvider(assembly) == null)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while creating a service provider.",
                        "The selected assembly doesn't contain any types that are marked with the TransferServiceProvider-attribute.",
                        PopupButtons.Ok);
                    return;
                }
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating a service provider.",
                    ex, PopupButtons.Ok);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void FtpAssemblyInputDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            assemblyFilePathTextBox.Initialize();
        }
    }
}