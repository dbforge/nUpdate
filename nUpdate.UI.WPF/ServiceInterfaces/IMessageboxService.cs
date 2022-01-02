// IMessageboxService.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

// ReSharper disable once CheckNamespace

namespace nUpdate.UI.WPF.ServiceInterfaces
{
    public interface IMessageboxService
    {
        EnuMessageBoxResult Show(string message, string title, EnuMessageBoxButton buttons = 0,
            EnuMessageBoxImage image = 0);
    }

    public enum EnuMessageBoxResult
    {
        None = 0,
        Ok = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public enum EnuMessageBoxButton
    {
        Ok = 0,
        OkCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }

    public enum EnuMessageBoxImage
    {
        None = 0,
        Error = 16,
        Question = 32,
        Warning = 48,
        Information = 64
    }
}