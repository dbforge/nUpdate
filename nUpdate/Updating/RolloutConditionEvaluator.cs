namespace nUpdate.Updating;

/// <summary>Decides whether a client's conditions allow a package to be rolled out to it.</summary>
internal static class RolloutConditionEvaluator
{
    /// <summary>
    ///     Returns <c>true</c> when the client may receive the package. Values compare case-insensitively.
    ///     A package without conditions is available to everyone. A client without conditions only receives packages
    ///     that have no positive conditions. A matching negated condition always excludes the client.
    /// </summary>
    public static bool Matches(RolloutSettings rollout, IReadOnlyDictionary<string, string> clientConditions)
    {
        if (rollout is null)
            throw new ArgumentNullException(nameof(rollout));
        if (clientConditions is null)
            throw new ArgumentNullException(nameof(clientConditions));

        if (rollout.Conditions.Count == 0)
            return true;

        var positive = rollout.Conditions.Where(c => !c.Negated).ToList();
        if (clientConditions.Count == 0)
            return positive.Count == 0;

        if (rollout.Conditions.Any(c => c.Negated && Matches(c, clientConditions)))
            return false;

        return rollout.Mode switch
        {
            RolloutConditionMode.Any => positive.Count == 0 || positive.Any(c => Matches(c, clientConditions)),
            RolloutConditionMode.All => positive.All(c => Matches(c, clientConditions)),
            _ => throw new ArgumentOutOfRangeException(nameof(rollout), "Invalid rollout condition mode."),
        };
    }

    private static bool Matches(RolloutCondition condition, IReadOnlyDictionary<string, string> client) =>
        client.TryGetValue(condition.Key, out var value) && string.Equals(condition.Value, value, StringComparison.OrdinalIgnoreCase);
}
