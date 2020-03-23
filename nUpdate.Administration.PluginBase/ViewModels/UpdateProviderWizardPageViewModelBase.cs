using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.PluginBase.ViewModels
{
    public abstract class UpdateProviderWizardPageViewModelBase : WizardPageViewModelBase
    {
        public WizardViewModelBase WizardViewModelBase { get; set; }
        public ProjectCreationData ProjectCreationData { get; set; }

        protected UpdateProviderWizardPageViewModelBase(WizardViewModelBase wizardViewModelBase, ProjectCreationData projectCreationData)
        {
            WizardViewModelBase = wizardViewModelBase;
            ProjectCreationData = projectCreationData;
        }
    }
}
