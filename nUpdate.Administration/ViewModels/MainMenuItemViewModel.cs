using System.Windows.Input;
using System.Windows.Media;

namespace nUpdate.Administration.ViewModels
{
    public class MainMenuItemViewModel
    {
        public MainMenuItemViewModel(string header, string description, MainMenuGroup category, ImageSource imageSource,
            ICommand command)
        {
            Header = header;
            Description = description;
            Category = category;
            Image = imageSource;
            Command = command;
        }

        public MainMenuGroup Category { get; }
        public ICommand Command { get; }
        public string Description { get; }
        public string Header { get; }
        public ImageSource Image { get; }
    }
}