using nUpdate.Administration.Core.Application;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    internal class BaseFormResult
    {
        /// <summary>
        /// The dialog result of the form.
        /// </summary>
        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// The update project of the form.
        /// </summary>
        public UpdateProject UpdateProject { get; set; }
    }
}
