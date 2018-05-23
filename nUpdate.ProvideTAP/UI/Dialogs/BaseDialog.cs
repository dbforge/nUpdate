// Copyright © Dominic Beger 2018

using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    internal class BaseDialog : Form
    {
        internal UpdateManager UpdateManager { get; set; }
    }
}