// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Providers;
using nUpdate.Administration.ViewModels.NewProject;

namespace nUpdate.Administration.ViewModels
{
    public class NewProjectViewModel : WizardViewModel
    {
        public NewProjectViewModel(INewProjectProvider newProjectProvider)
        {
            InitializePages(new List<WizardPageViewModel>
            {
                new GenerateKeyPairPageViewModel(this),
                new GeneralDataPageViewModel(this, newProjectProvider),
                new UpdateProviderSelectionPageViewModel(this),
                new FtpDataPageViewModel(this, newProjectProvider),
                new HttpDataPageViewModel(this)
            });

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
                ProjectCreationData.Project.PrivateKey = ProjectCreationData.PrivateKey;
                ProjectCreationData.Project.Save(Path.Combine(projectDirectory,
                    ProjectCreationData.Project.Name + ".nupdproj"));
                return true;
            });
        }

        protected override void GoBack()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateBack(this);
            CurrentPageViewModel = oldPageViewModel is IUpdateProviderPageViewModel
                ? PageViewModels.First(x => x.GetType() == typeof(UpdateProviderSelectionPageViewModel))
                : PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }

        protected override async void GoForward()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateForward(this);

            if (oldPageViewModel.GetType() == typeof(UpdateProviderSelectionPageViewModel))
            {
                switch (ProjectCreationData.UpdateProviderType)
                {
                    case UpdateProviderType.ServerOverFtp:
                        CurrentPageViewModel =
                            PageViewModels.First(x => x.GetType() == typeof(FtpDataPageViewModel));
                        break;
                    case UpdateProviderType.ServerOverHttp:
                        CurrentPageViewModel =
                            PageViewModels.First(x => x.GetType() == typeof(HttpDataPageViewModel));
                        break;
                    // TODO: Implement
                    case UpdateProviderType.ServerOverSsh:
                    case UpdateProviderType.GitHub:
                    case UpdateProviderType.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (oldPageViewModel is IUpdateProviderPageViewModel)
            {
                // TODO: Add page after protocol-specific pages
                // If no errors occured and everything worked, we can now close the window
                if (await Finish())
                    FinishingAction.Invoke();
                return;
            }
            else
            {
                CurrentPageViewModel =
                    PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
            }
            
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }
    }
}