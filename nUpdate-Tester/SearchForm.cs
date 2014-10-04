using System;
using System.Windows.Forms;

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

        public void Finished(bool found)
        {
            DialogResult = DialogResult.OK;
        }
    }
}