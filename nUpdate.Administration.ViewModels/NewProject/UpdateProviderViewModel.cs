using System;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderViewModel : NotifyPropertyChangedBase
    {
        private Guid _identifier;
        private string _description;
        private bool _isSelected;

        public UpdateProviderViewModel(Guid identifier, string description, bool isSelected)
        {
            _identifier = identifier;
            _description = description;
            _isSelected = isSelected;
        }

        public Guid Identifier
        {
            get => _identifier;
            set => SetProperty(value, ref _identifier);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(value, ref _description);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(value, ref _isSelected);
        }
    }
}
