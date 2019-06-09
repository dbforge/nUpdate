using System.Windows.Input;

namespace nUpdate.Administration.Common
{
    public class MainMenuItem
    {
        public MainMenuItem(string header, string description, MainMenuGroup category, /*ImageSource imageSource,*/
            ICommand command)
        {
            Header = header;
            Description = description;
            Category = category;
            //Image = imageSource;
            Command = command;
        }

        public MainMenuGroup Category { get; }
        public ICommand Command { get; }
        public string Description { get; }
        public string Header { get; }
        //public ImageSource Image { get; }
    }
}