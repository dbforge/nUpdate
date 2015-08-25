// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Windows.Forms;
using nUpdate.UpdateEventArgs;

namespace nUpdate_Tester
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
        }

        public void Fail(object sender, FailedEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        public void Finished(object sender, UpdateSearchFinishedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}