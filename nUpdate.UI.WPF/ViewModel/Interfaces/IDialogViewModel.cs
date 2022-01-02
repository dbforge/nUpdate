// IDialogViewModel.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019



// ReSharper disable once CheckNamespace

using System.Windows.Threading;

namespace nUpdate.UI.WPF.ViewModel.Interfaces
{
    public interface IDialogViewModel
    {
        bool DialogResult { get; set; }
        string WindowTitle { get; set; }
        Dispatcher CurrentDispatcher { get; set; }
        void DialogLoaded();
        void DialogClosing();
    }
}