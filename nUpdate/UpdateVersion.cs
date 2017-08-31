// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace nUpdate
{
    public class UpdateVersion
    {
        // <Major>.<Minor>.<Revision>[[-]SemVerSuffix|.][Build][+BuildMetaData]
        private const string RegexString =
                @"^(?<major>\d+)(?:\.(?<minor>\d+)(?:\.(?<build>\d+)|)|)(?:(?:\-?(?<semver>[a-zA-Z]+)|\.)(?:(?<revision>\d+)|)|)(?:\+(?:(?<meta>.+)|)|)$"
            ;

        public UpdateVersion(string versionString)
        {
            var match = Regex.Match(versionString, RegexString, RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ArgumentException("The specified version is not valid.");

            Major = int.Parse(match.Groups["major"].Captures[0].Value);
            if (match.Groups["minor"].Success)
            {
                int minor;
                if (!int.TryParse(match.Groups["minor"].Captures[0].Value, out minor))
                    throw new ArgumentException("Minor is not a valid version number.");
                Minor = minor;
            }

            if (match.Groups["build"].Success)
            {
                int build;
                if (!int.TryParse(match.Groups["build"].Captures[0].Value, out build))
                    throw new ArgumentException("Build is not a valid version number.");
                Build = build;
            }

            if (match.Groups["semver"].Success)
                SemVerSuffix = match.Groups["semver"].Captures[0].Value;

            if (match.Groups["revision"].Success)
            {
                int revision;
                if (!int.TryParse(match.Groups["revision"].Captures[0].Value, out revision))
                    throw new ArgumentException("Revision is not a valid version number.");
                Revision = revision;
            }

            if (match.Groups["meta"].Success)
                BuildMetadata = match.Groups["meta"].Captures[0].Value;
        }

        public UpdateVersion(int major, int minor, int revision, string semVerSuffix = "", int build = 0,
            string buildMetaData = "")
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException(nameof(major), "Index must be 0 or higher");

            if (minor < 0)
                throw new ArgumentOutOfRangeException(nameof(minor), "Index must be 0 or higher");

            if (build < 0)
                throw new ArgumentOutOfRangeException(nameof(build), "Index must be 0 or higher");

            if (revision < 0)
                throw new ArgumentOutOfRangeException(nameof(revision), "Index must be 0 or higher");

            Major = major;
            Minor = minor;
            Revision = revision;
            SemVerSuffix = semVerSuffix;
            Build = build;
            BuildMetadata = buildMetaData;
        }

        /// <summary>
        ///     Gets or sets the build/patch version.
        /// </summary>
        public int Build { get; }

        /// <summary>
        ///     Gets or sets the build metadata (SemVer).
        /// </summary>
        public string BuildMetadata { get; set; }

        /// <summary>
        ///     Gets or sets the major version.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets or sets the minor version.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets or sets the revision version.
        /// </summary>
        public int Revision { get; }

        /// <summary>
        ///     Gets or sets the semantic version suffix representing the update channel.
        /// </summary>
        public string SemVerSuffix { get; set; }

        /// <summary>
        ///     Gets the <see cref="UpdateChannel" /> that this <see cref="UpdateVersion" /> is associated with.
        /// </summary>
        /// <param name="masterChannel">The master channel containing all available channels.</param>
        /// <returns>The <see cref="UpdateChannel" /> associated with this <see cref="UpdateVersion" />.</returns>
        public UpdateChannel GetUpdateChannel(IEnumerable<UpdateChannel> masterChannel)
        {
            return masterChannel.First(x => x.SupportedSuffixes.Contains(SemVerSuffix.ToLowerInvariant()));
        }

        /// <summary>
        ///     Determines whether the specified update version is valid.
        /// </summary>
        /// <param name="updateVersion">The update version to check.</param>
        public static bool IsValid(UpdateVersion updateVersion)
        {
            return IsValid(updateVersion.ToString());
        }

        /// <summary>
        ///     Determines whether the specified version string is valid.
        /// </summary>
        /// <param name="versionString">The version string to check.</param>
        public static bool IsValid(string versionString)
        {
            var regex = new Regex(RegexString, RegexOptions.IgnoreCase);
            return regex.IsMatch(versionString);
        }

        /// <summary>
        ///     Tries to get the <see cref="UpdateChannel" /> that this <see cref="UpdateVersion" /> is associated with.
        /// </summary>
        /// <param name="masterChannel">The master channel containing all available channels.</param>
        /// <param name="updateChannel">The <see cref="UpdateChannel" /> object that should be initialized.</param>
        /// <returns><c>true</c>, if the <see cref="UpdateChannel" /> could be determined, otherwise <c>false</c>.</returns>
        public bool TryGetUpdateChannel(IEnumerable<UpdateChannel> masterChannel, out UpdateChannel updateChannel)
        {
            updateChannel =
                masterChannel.FirstOrDefault(x => x.SupportedSuffixes.Contains(SemVerSuffix.ToLowerInvariant()));
            return updateChannel != null;
        }
    }
}