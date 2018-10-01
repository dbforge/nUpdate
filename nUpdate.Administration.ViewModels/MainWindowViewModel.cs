using System.Collections.Generic;
using System.Windows.Input;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Providers;

// ReSharper disable MemberCanBeMadeStatic.Local

namespace nUpdate.Administration.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private ICommand _loadCommand;

        public MainWindowViewModel(IMainViewActionProvider mainWindowActionProvider)
        {
            LoadCommand = new RelayCommand(mainWindowActionProvider.Load);
            CollectionView = mainWindowActionProvider.GetCollectionView();
        }

        public List<MainMenuItem> CollectionView { get; }
        
        public ICommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand);
        }
    }
}