// IDialogWindowService.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using nUpdate.UI.WPF.ViewModel.Interfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.ServiceInterfaces
{
    public interface IDialogWindowService
    {
        void ShowDialog(string windowname, IDialogViewModel datacontext);
        void CloseDialog();
    }
}