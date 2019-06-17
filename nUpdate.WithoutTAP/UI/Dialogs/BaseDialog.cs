// BaseDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    internal class BaseDialog : Form
    {
        internal UpdateManager UpdateManager { get; set; }

        internal void ShowDialog(DialogResultWrapper d)
        {
            d.DialogResult = ShowDialog();
        }
    }
}