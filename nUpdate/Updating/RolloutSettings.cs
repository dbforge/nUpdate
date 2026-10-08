namespace nUpdate.Updating;

/// <summary>How the positive rollout conditions of a package are combined.</summary>
public enum RolloutConditionMode
{
    /// <summary>The client must match at least one positive condition.</summary>
    Any,

    /// <summary>The client must match every positive condition.</summary>
    All,
}

/// <summary>A key/value pair a client must (or, when negated, must not) report in order to receive a package.</summary>
public sealed class RolloutCondition
{
    public RolloutCondition()
    {
    }

    public RolloutCondition(string key, string value, bool negated = false)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Negated = negated;
    }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool Negated { get; set; }
}

/// <summary>The rollout conditions of a package and how they combine.</summary>
public sealed class RolloutSettings
{
    public RolloutConditionMode Mode { get; set; } = RolloutConditionMode.Any;

    public List<RolloutCondition> Conditions { get; set; } = [];
}
