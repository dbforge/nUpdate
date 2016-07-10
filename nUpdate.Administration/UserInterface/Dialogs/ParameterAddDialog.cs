using System.Windows.Forms;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    public partial class ParameterAddDialog : Form
    {
        public ParameterAddDialog()
        {
            InitializeComponent();
        }

        public string Parameter => parameterTextBox.Text;
    }
}
