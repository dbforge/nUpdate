// ServiceInjector.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using nUpdate.UI.WPF.ServiceInterfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.UI.WPF.Services
{
    public static class ServiceInjector
    {
        // Loads service objects into the ServiceContainer on startup.
        public static void InjectServices()
        {
            ServiceContainer.Instance.AddService<IMessageboxService>(
                new MessageBoxService());
            ServiceContainer.Instance.AddService<IDialogWindowService>(
                new DialogWindowService());
        }
    }
}