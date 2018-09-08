// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using nUpdate.Administration.ViewModels.NewProject;
using nUpdate.Administration.Views;

namespace nUpdate.Administration.ViewModels
{
    public class NewProjectViewModel : PagedWindowViewModel
    {
        public NewProjectViewModel()
        {
            InitializePages(new List<PageViewModel>
            {
                new GenerateKeyPairPageViewModel(this),
                new GeneralDataPageViewModel(this),
                new ProtocolSelectionPageViewModel(this),
                new FtpDataPageViewModel(this),
                new HttpDataPageViewModel(this)
            });
        }

        public ProjectCreationData ProjectCreationData { get; } = new ProjectCreationData();

        protected override Task<bool> Finish()
        {
            return new Task<bool>(() =>
            {
                ProjectCreationData.Project.PrivateKey = ProjectCreationData.PrivateKey;
                ProjectCreationData.Project.Save(Path.Combine(ProjectCreationData.Location,
                    ProjectCreationData.Project.Name + ".nupdproj"));
                return true;
            });
        }

        protected override void GoBack()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateBack(this);
            CurrentPageViewModel = oldPageViewModel is IProtocolPageViewModel
                ? PageViewModels.First(x => x.GetType() == typeof(ProtocolSelectionPageViewModel))
                : PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) - 1];
            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }

        protected override async Task GoForward()
        {
            var oldPageViewModel = CurrentPageViewModel;
            oldPageViewModel.OnNavigateForward(this);

            // TODO: Check the attribute implementation
            /*if (AttributeHelper.Compare<DescriptionAttribute, string>(oldPageViewModel,
                typeof(ProtocolSelectionPageViewModel), d => d.Description))*/
            if (oldPageViewModel.GetType() == typeof(ProtocolSelectionPageViewModel))
            {
                switch (ProjectCreationData.TransferProtocol)
                {
                    case TransferProtocol.FTP:
                        CurrentPageViewModel = PageViewModels.First(x => x.GetType() == typeof(FtpDataPageViewModel));
                        break;
                    case TransferProtocol.HTTP:
                        CurrentPageViewModel = PageViewModels.First(x => x.GetType() == typeof(HttpDataPageViewModel));
                        break;
                    case TransferProtocol.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (oldPageViewModel is IProtocolPageViewModel)
            {
                // TODO: Add page after protocol-specific pages
                // If no errors occured and everything worked, we can now close the window
                var t = Finish();
                t.RunSynchronously();
                if (t.Result)
                    WindowManager.GetCurrentWindow().RequestClose();
                return;
            }
            else
            {
                // There is no further page which means we are finished with the wizard
                if (PageViewModels.IndexOf(CurrentPageViewModel) == PageViewModels.Count - 1)
                {
                    // If no errors occured and everything worked, we can now close the window
                    var t = Finish();
                    t.RunSynchronously();
                    if (t.Result)
                        WindowManager.GetCurrentWindow().RequestClose();
                    return;
                }

                CurrentPageViewModel =
                    PageViewModels[PageViewModels.IndexOf(CurrentPageViewModel) + 1];
            }

            CurrentPageViewModel.OnNavigated(oldPageViewModel, this);
        }
    }
}