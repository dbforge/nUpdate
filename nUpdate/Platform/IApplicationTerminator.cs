namespace nUpdate.Platform;

/// <summary>Ends the host application so the installer can replace its files.</summary>
public interface IApplicationTerminator
{
    void Terminate();
}
