using nUpdate.Administration.Core.Application;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The update project in the current state.
        /// </summary>
        internal UpdateProject Project { get; set; }

        /// <summary>
        /// Shows a new dialog.
        /// </summary>
        /// <typeparam name="T">The dialog to show.</typeparam>
        /// <returns>Returns the returned <see cref="DialogResult"/>.</returns>
        internal BaseFormResult ShowDialog<T>() where T : BaseForm
        {
            return this.ShowDialog<T>(null, null);
        }

        /// <summary>
        /// Shows a new dialog.
        /// </summary>
        /// <typeparam name="T">The dialog to show.</typeparam>
        /// <param name="owner">The owner of the dialog.</param>
        /// <returns>Returns the returned <see cref="DialogResult"/>.</returns>
        internal BaseFormResult ShowDialog<T>(IWin32Window owner) where T : BaseForm
        {
            return this.ShowDialog<T>(owner, null);
        }

        /// <summary>
        /// Shows a new dialog.
        /// </summary>
        /// <typeparam name="T">The dialog to show.</typeparam>
        /// <param name="owner">The owner of the dialog.</param>
        /// <param name="args">The project to handle over.</param>
        /// <returns>Returns the returned <see cref="DialogResult"/>.</returns>
        internal BaseFormResult ShowDialog<T>(IWin32Window owner, UpdateProject project) where T : BaseForm
        {
            // Create the instance of the form
            var dialogInstance = Activator.CreateInstance<T>();
            dialogInstance.Project = project;

            var result = new BaseFormResult();
            result.DialogResult = dialogInstance.ShowDialog(owner);
            result.UpdateProject = dialogInstance.Project;

            return result;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "BaseForm";
            this.BackColor = SystemColors.Window;
            this.Font = new Font("SeogeUI", 8);
            this.ResumeLayout(false);
        }
    }
}
