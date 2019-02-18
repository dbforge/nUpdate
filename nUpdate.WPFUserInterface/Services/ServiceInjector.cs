using nUpdate.WPFUserInterface.ServiceInterfaces;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.Services
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