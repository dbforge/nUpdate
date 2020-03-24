// SemanticVersion.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace nUpdate
{
    public class SemanticVersion : IVersion<SemanticVersion>
    {
        // https://semver.org
        // https://github.com/semver/semver/issues/232#issuecomment-405596809
        private const string RegexString =
            @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

        public SemanticVersion(string versionString)
        {
            var match = Regex.Match(versionString, RegexString, RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ArgumentException("The specified version is not valid.");

            Major = int.Parse(match.Groups["major"].Captures[0].Value);
            Minor = int.Parse(match.Groups["minor"].Captures[0].Value);
            Patch = int.Parse(match.Groups["patch"].Captures[0].Value);

            if (match.Groups["prerelease"].Success)
                PreRelease = match.Groups["prerelease"].Captures[0].Value;

            if (match.Groups["buildmetadata"].Success)
                BuildMetadata = match.Groups["buildmetadata"].Captures[0].Value;
        }

        /// <summary>
        ///     Gets the build metadata.
        /// </summary>
        public string BuildMetadata { get; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether build metadata is available, or not.
        /// </summary>
        public bool HasBuildMetadata => !BuildMetadata.Equals(string.Empty);

        /// <summary>
        ///     Gets the major version.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets the minor version.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets the patch version.
        /// </summary>
        public int Patch { get; }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(SemanticVersion))
                throw new InvalidOperationException();

            return CompareTo((SemanticVersion) obj);
        }

        // https://semver.org/#spec-item-11
        public int CompareTo(SemanticVersion other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (other is null)
                return 1;

            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
                return majorComparison;
            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
                return minorComparison;
            var patchComparison = Patch.CompareTo(other.Patch);
            if (patchComparison != 0)
                return patchComparison;

            if (!HasPreRelease && !other.HasPreRelease)
                return 0;
            if (!HasPreRelease)
                return 1;
            if (!other.HasPreRelease)
                return -1;

            var preReleaseElements = PreRelease.Split('.');
            var otherPreReleaseElements = other.PreRelease.Split('.');
            var minElements = Math.Min(preReleaseElements.Length, otherPreReleaseElements.Length);
            for (var i = 0; i < minElements; ++i)
            {
                int c;
                var elementIsNumber = int.TryParse(preReleaseElements[i], out var elementNumber);
                var otherElementIsNumber = int.TryParse(otherPreReleaseElements[i], out var otherElementNumber);

                if (elementIsNumber && otherElementIsNumber)
                {
                    if ((c = elementNumber.CompareTo(otherElementNumber)) != 0)
                        return c;
                }
                else
                {
                    if (elementIsNumber)
                        return -1;
                    if (otherElementIsNumber)
                        return 1;
                    c = Math.Sign(string.CompareOrdinal(preReleaseElements[i], otherPreReleaseElements[i]));
                    if (c != 0)
                        return c;
                }
            }

            return preReleaseElements.Length.CompareTo(otherPreReleaseElements.Length);
        }

        bool IEquatable<SemanticVersion>.Equals(SemanticVersion other)
        {
            return Equals(other);
        }

        /// <summary>
        ///     Gets a value indicating whether pre-release information is available, or not.
        /// </summary>
        public bool HasPreRelease => !PreRelease.Equals(string.Empty);

        public bool IsValid()
        {
            var regex = new Regex(RegexString, RegexOptions.IgnoreCase);
            return regex.IsMatch(ToString());
        }

        /// <summary>
        ///     Gets the pre-release information.
        /// </summary>
        public string PreRelease { get; } = string.Empty;

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        protected bool Equals(SemanticVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch &&
                   string.Equals(PreRelease, other.PreRelease) && string.Equals(BuildMetadata, other.BuildMetadata);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ (PreRelease != null ? PreRelease.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BuildMetadata != null ? BuildMetadata.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        ///     Determines whether the specified update version is valid.
        /// </summary>
        /// <param name="semanticVersion">The update version to check.</param>
        public static bool IsValid(SemanticVersion semanticVersion)
        {
            return semanticVersion.IsValid();
        }

        /// <summary>
        ///     Determines whether the specified version string is valid.
        /// </summary>
        /// <param name="versionString">The version string to check.</param>
        public static bool IsValid(string versionString)
        {
            return new SemanticVersion(versionString).IsValid();
        }

        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            return version1 != null && version1.CompareTo(version2).Equals(0);
        }

        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            return version1.CompareTo(version2).Equals(1);
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
        {
            return !(version1 == version2);
        }

        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            return version1.CompareTo(version2).Equals(-1);
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"{Major}.{Minor}.{Patch}");
            if (HasPreRelease)
                builder.Append($"-{PreRelease}");
            if (HasBuildMetadata)
                builder.Append($"+{BuildMetadata}");
            return builder.ToString();
        }
    }
}