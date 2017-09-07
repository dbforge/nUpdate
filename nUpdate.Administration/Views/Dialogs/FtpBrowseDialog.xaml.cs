// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TaskDialogInterop;

namespace nUpdate.Administration.Views.Dialogs
{
    public partial class FtpBrowseDialog
    {
        private const string ServerItemTag = "server";
        private readonly List<TreeViewItem> _cachedTreeViewItems = new List<TreeViewItem>();
        private readonly TransferManager _transferManager;
        private string _directory = "/";

        public FtpBrowseDialog(ITransferData data)
        {
            InitializeComponent();
            _transferManager = new TransferManager(TransferProtocol.FTP, data);
            ServerTreeView.Items.Add(new TreeViewItem {Header = "/", Tag = ServerItemTag});
        }

        public string Directory
        {
            get => _directory;
            set
            {
                _directory = value;
                DirectoryTextBox.Text = value;
            }
        }

        private async Task AddDirectoryContent(string path, ItemsControl current)
        {
            var serverItems = await _transferManager.List(path, false);
            foreach (var serverItem in serverItems.Where(x => x.ItemType == ServerItemType.Directory))
                current.Items.Add(new TreeViewItem {Header = serverItem.Name});
        }

        private async void FtpBrowseWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await AddDirectoryContent("/", (TreeViewItem) ServerTreeView.Items[0]);
        }

        private async void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = (TreeViewItem) e.NewValue;
            var parentAdorner = AdornerLayer.GetAdornerLayer(MainPanel);
            var loadingAdorner = new LoadingAdorner(MainPanel)
            {
                Text = "Loading directories...",
                Width = MainPanel.Width,
                Height = MainPanel.Height
            };
            parentAdorner.Add(loadingAdorner);

            // Update the path
            UpdatePath(selectedItem);

            // If the content of the selected item has already been listed, we can ignore it
            if (_cachedTreeViewItems.Contains(selectedItem))
                return;

            ServerTreeView.IsEnabled = false;
            _cachedTreeViewItems.Add(selectedItem);

            try
            {
                await AddDirectoryContent(Directory, selectedItem);
            }
            catch (Exception ex)
            {
                var taskDialog = new TaskDialogOptions
                {
                    MainIcon = VistaTaskDialogIcon.Error,
                    MainInstruction = ex.Message,
                    ExpandedInfo = ex.StackTrace
                };
                TaskDialog.Show(taskDialog);
            }
            finally
            {
                parentAdorner.Remove(loadingAdorner);
                ServerTreeView.IsEnabled = true;
            }
        }

        private void UpdatePath(TreeViewItem current)
        {
            var itemStack = new Stack<TreeViewItem>();
            var currentItem = current;
            while (currentItem != null)
            {
                itemStack.Push(currentItem);
                if (currentItem.Parent == null
                    || currentItem.Parent.GetType() != typeof(TreeViewItem)
                    || ((TreeViewItem) currentItem.Parent).Tag.ToString() == ServerItemTag)
                    break;
                currentItem = (TreeViewItem) currentItem.Parent;
            }

            Directory = string.Join("/", itemStack.Select(x => x.Header.ToString()));
        }
    }
}