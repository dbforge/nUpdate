namespace nUpdate.UpdateInstaller;

/// <summary>Counts finished tasks and turns them into a percentage.</summary>
public sealed class ProgressTracker
{
    public int Total { get; private set; }

    public int Done { get; private set; }

    public float Percentage => Total <= 0 ? 100f : Math.Min(100f, 100f * Done / Total);

    public void AddTasks(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        Total += count;
    }

    /// <summary>Marks one task as done and returns the new percentage.</summary>
    public float Advance()
    {
        Done++;
        return Percentage;
    }
}
