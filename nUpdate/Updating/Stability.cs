namespace nUpdate.Updating;

/// <summary>
///     The least stable versions a client installs; each level includes the more stable ones above it, so
///     <see cref="Beta" /> also installs release candidates and releases.
/// </summary>
public enum Stability
{
    /// <summary>Releases only.</summary>
    Release,

    /// <summary>Release candidates (<c>rc</c>) and releases.</summary>
    ReleaseCandidate,

    /// <summary>Betas, release candidates and releases.</summary>
    Beta,

    /// <summary>Everything, including alphas and labels nUpdate does not know.</summary>
    Any,
}

/// <summary>The stage nUpdate recognises in the first identifier of a pre-release label.</summary>
internal enum PreReleaseStage
{
    None,
    ReleaseCandidate,
    Beta,
    Alpha,
    Other,
}
