namespace nUpdate.Administration.ViewModels.NewProject
{
    public class ProtocolSelectionPageViewModel : PageViewModel
    {
        private readonly NewProjectViewModel _newProjectViewModel;
        private TransferProtocol _transferProtocol;

        public ProtocolSelectionPageViewModel(NewProjectViewModel viewModel)
        {
            _newProjectViewModel = viewModel;
            CanGoBack = true;
            CanGoForward = true;

            _transferProtocol = TransferProtocol.HTTP;
        }

        public TransferProtocol TransferProtocol
        {
            get => _transferProtocol;
            set
            {
                SetProperty(value, ref _transferProtocol, nameof(TransferProtocol));
                _newProjectViewModel.ProjectCreationData.TransferProtocol = value;
            }
        }
    }
}
