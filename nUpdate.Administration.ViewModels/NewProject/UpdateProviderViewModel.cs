// UpdateProviderViewModel.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.Administration.Infrastructure;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class UpdateProviderViewModel : NotifyPropertyChangedBase
    {
        private string _description;
        private Guid _identifier;
        private bool _isSelected;

        public UpdateProviderViewModel(Guid identifier, string description, bool isSelected)
        {
            _identifier = identifier;
            _description = description;
            _isSelected = isSelected;
        }

        public string Description
        {
            get => _description;
            set => SetProperty(value, ref _description);
        }

        public Guid Identifier
        {
            get => _identifier;
            set => SetProperty(value, ref _identifier);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(value, ref _isSelected);
        }
    }
}