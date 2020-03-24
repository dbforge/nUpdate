// NoUpdateFoundDialog.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Properties;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal partial class NoUpdateFoundDialog : BaseDialog
    {
        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = UpdateProvider.LanguageCulture;

            closeButton.Text = strings.CloseButtonText;
            headerLabel.Text = strings.NoUpdateDialogHeader;
            infoLabel.Text = strings.NoUpdateDialogInfoText;

            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
        }
    }
}