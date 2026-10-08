namespace nUpdate.Tests.Installer.Support;

/// <summary>Waits for what another thread does.</summary>
public static class Polling
{
    /// <summary>Waits until the condition holds and fails after ten seconds.</summary>
    public static void WaitUntil(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
                throw new TimeoutException();
            Thread.Sleep(5);
        }
    }
}
