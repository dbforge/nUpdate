using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public partial class ChangelogPanel : UserControl
    {
        public ChangelogPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Returns the changelog set.
        /// </summary>
        public string Changelog
        {
            get { return changelogTextBox.Text; }
            set { changelogTextBox.Text = value; }
        }
    }
}
