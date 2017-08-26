// Copyright © Dominic Beger 2017

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels
{
    public class FtpBrowseViewModel : WindowViewModel
    {
        private readonly List<TreeViewItem> _cachedTreeViewItems = new List<TreeViewItem>();
        private string _directory = "/";
        private ActionCommand<RoutedPropertyChangedEventArgs<object>> _selectedItemChangedCommand;
        private RelayCommand _loadCommand;

        public FtpBrowseViewModel()
        {
            _loadCommand = new RelayCommand(OnLoad);
            _selectedItemChangedCommand =
                new ActionCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectedItemChanged);
        }

        private async void OnLoad()
        {
            await AddDirectoryContent("/", ServerItem[0]);
        }

        public string Directory
        {
            get => _directory;
            set => SetProperty(value, ref _directory, nameof(Directory));
        }

        public RelayCommand LoadCommand
        {
            get => _loadCommand;
            set => SetProperty(value, ref _loadCommand, nameof(LoadCommand));
        }

        public ActionCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand
        {
            get => _selectedItemChangedCommand;
            set => SetProperty(value, ref _selectedItemChangedCommand, nameof(SelectedItemChangedCommand));
        }
        
        public List<TreeViewItem> ServerItem => new List<TreeViewItem> {new TreeViewItem {Header = "/"}};

        public TransferManager TransferManager { get; set; }

        private async Task AddDirectoryContent(string path, ItemsControl current)
        {
            var serverItems = await TransferManager.List(path, false);
            foreach (var serverItem in serverItems.Where(x => x.ItemType == ServerItemType.Directory))
                current.Items.Add(new TreeViewItem {Header = serverItem.Name});
        }

        private async void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = (TreeViewItem) e.NewValue;

            // Update the path
            UpdatePath(selectedItem);

            // If the content of the selected item has already been listed, we can ignore it
            if (_cachedTreeViewItems.Contains(selectedItem))
                return;

            _cachedTreeViewItems.Add(selectedItem);
            await AddDirectoryContent(Directory, selectedItem);
        }

        private void UpdatePath(TreeViewItem current)
        {
            var itemStack = new Stack<TreeViewItem>();
            var currentItem = current;
            while (currentItem != null)
            {
                itemStack.Push(currentItem);
                currentItem = (TreeViewItem) currentItem.Parent;
            }

            Directory = string.Join("/", itemStack.Select(x => x.Header.ToString()));
        }
    }
}