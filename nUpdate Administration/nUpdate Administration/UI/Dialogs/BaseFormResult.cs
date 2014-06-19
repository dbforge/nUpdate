using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.Core.Application;

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
