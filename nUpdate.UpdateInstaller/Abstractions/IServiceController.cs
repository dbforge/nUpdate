namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Controls Windows services.</summary>
public interface IServiceController
{
    /// <summary>Starts the service, or restarts it when it is already running.</summary>
    void StartService(string serviceName, string[] arguments);

    void StopService(string serviceName);
}
