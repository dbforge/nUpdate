// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using nUpdate.Administration.BusinessLogic;
using nUpdate.Administration.PluginBase;
using nUpdate.Administration.PluginBase.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class NewProjectBase : WizardViewModelBase
    {
        private IUpdateProviderPlugin _currentUpdateProviderPlugin;

        public NewProjectBase(INewProjectProvider newProjectProvider)
        {
            var pages = new List<WizardPageViewModelBase>
            {
                new GenerateKeyPairPageViewModel(this),
                new GeneralDataPageViewModel(this, newProjectProvider),
                new UpdateProviderSelectionPageViewModel(ProjectCreationData,
                    GlobalSession.UpdateProviderPlugins.ToDictionary(p => p.Value.Identifier, p => p.Value.Description))
            };

            pages.AddRange(from plugin in GlobalSession.UpdateProviderPlugins
                from type in plugin.Value.WizardViewModelViewAssociations.Keys
                select (UpdateProviderWizardPageViewModelBase) Activator.CreateInstance(type, this, ProjectCreationData));

            pages.Add(new FinishPageViewModel());
            InitializePages(pages);

            newProjectProvider.SetFinishAction(out var f);
            FinishingAction = f;
        }

        public ProjectCreationData ProjectCreationData { get; } = new ProjectCreationData();

        protected override Task<bool> Finish()
        {
            return Task.Run(() =>
            {
                string projectDirectory = Path.Combine(ProjectCreationData.Location, ProjectCreationData.Project.Name);
                if (!Directory.Exists(projectDirectory))
                    Directory.CreateDirectory(projectDirectory);
                KeyManager.Instance[ProjectCreationData.Project.Identifier] = ProjectCreationData.PrivateKey;
                new UpdateProjectBl(ProjectCreationData.Project).Save(Path.Combine(projectDirectory,
                    ProjectCreationData.Project.Name, ".nupdproj"));
                return true;
            });
        }

        protected override void GoBack()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateBack(this);
            switch (oldPageViewModel)
            {
                case IFirstUpdateProviderBase _:
                    CurrentPageViewModel =
                        PageViewModels.First(x => x.GetType() == typeof(UpdateProviderSelectionPageViewModel));
                    break;
                case UpdateProviderWizardPageViewModelBase _:
                    CurrentPageViewModel =
                        _currentUpdateProviderPlugin.GetPreviousPageViewModel(this, CurrentPageViewModel, ProjectCreationData) ??
                            PageViewModels[PageViewModels.IndexOf(PageViewModels.First(p => p is UpdateProviderSelectionPageViewModel))];
                    break;
                default:
                    CurrentPageViewModel = PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
                    break;
            }
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }

        protected override async void GoForward()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateForward(this);

            switch (oldPageViewModel)
            {
                case UpdateProviderSelectionPageViewModel _:
                    _currentUpdateProviderPlugin = GlobalSession.UpdateProviderPlugins.First(p =>
                        p.Value.Identifier.Equals(ProjectCreationData.Project.UpdateProviderIdentifier)).Value;
                    CurrentPageViewModel = _currentUpdateProviderPlugin.GetNextPageViewModel(this, null, ProjectCreationData);
                    break;
                case UpdateProviderWizardPageViewModelBase _:
                    CurrentPageViewModel = _currentUpdateProviderPlugin.GetNextPageViewModel(this, CurrentPageViewModel, ProjectCreationData) ?? PageViewModels.Last();
                    break;
                case FinishPageViewModel _:
                {
                    // If no errors occured and everything worked, we can now close the window
                    if (await Finish())
                        FinishingAction.Invoke();
                    return;
                }

                default:
                    CurrentPageViewModel =
                        PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
                    break;
            }
            
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }
    }
}