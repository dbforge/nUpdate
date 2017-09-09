// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using nUpdate.Core;

namespace nUpdate.Updating
{
    public class UpdateVersion : IComparable<UpdateVersion>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        public UpdateVersion()
            : this(0, 0, 0, 0, DevelopmentalStage.Release, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        /// <param name="version">The update version.</param>
        public UpdateVersion(string version)
        {
            var match = Regex.Match(version,
                @"^(?<Version>((?<VersionNumber>\d+)\.){0,3}(?<VersionNumber>\d+))((-| )?(?<DevStage>(?<Type>[ab]|rc)(\.?(?<DevBuild>\d+))?))?$");
            if (!match.Success || !match.Groups["Version"].Success)
                throw new ArgumentException("The specified version is not valid.");

            Major = int.Parse(match.Groups["VersionNumber"].Captures[0].Value);
            if (match.Groups["VersionNumber"].Captures.Count > 1)
                Minor = int.Parse(match.Groups["VersionNumber"].Captures[1].Value);
            else
                Build = 0;
            Build = match.Groups["VersionNumber"].Captures.Count > 2
                ? int.Parse(match.Groups["VersionNumber"].Captures[2].Value)
                : 0;
            Revision = match.Groups["VersionNumber"].Captures.Count > 3
                ? int.Parse(match.Groups["VersionNumber"].Captures[3].Value)
                : 0;

            if (!match.Groups["DevStage"].Success)
                return;
            var developmentalStage = match.Groups["Type"].Value;
            switch (developmentalStage)
            {
                case "a":
                    DevelopmentalStage = DevelopmentalStage.Alpha;
                    break;
                case "b":
                    DevelopmentalStage = DevelopmentalStage.Beta;
                    break;
                case "rc":
                    DevelopmentalStage = DevelopmentalStage.ReleaseCandidate;
                    break;
                default:
                    throw new ArgumentException("The specified developmental stage is not valid.");
            }

            DevelopmentBuild = match.Groups["DevBuild"].Success ? int.Parse(match.Groups["DevBuild"].Value) : 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="build">The build version.</param>
        /// <param name="revision">The revision version.</param>
        public UpdateVersion(int major, int minor, int build, int revision)
            : this(major, minor, build, revision, DevelopmentalStage.Release, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="build">The build version.</param>
        /// <param name="revision">The revision version.</param>
        /// <param name="devStage">The developmental stage.</param>
        /// <param name="developmentBuild">The pre-release version.</param>
        public UpdateVersion(int major, int minor, int build, int revision, DevelopmentalStage devStage,
            int developmentBuild)
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
            Build = build;
            Revision = revision;
            DevelopmentalStage = devStage;
            DevelopmentBuild = developmentBuild;
        }

        /// <summary>
        ///     Gets the current <see cref="UpdateVersion" /> without the developmental stage and development build.
        /// </summary>
        public string BasicVersion => $"{Major}.{Minor}.{Build}.{Revision}";

        /// <summary>
        ///     The build of the version.
        /// </summary>
        public int Build { get; }

        /// <summary>
        ///     The developmental stage of the version.
        /// </summary>
        public DevelopmentalStage DevelopmentalStage { get; set; }

        /// <summary>
        ///     The build version of the alpha or beta.
        /// </summary>
        public int DevelopmentBuild { get; set; }

        /// <summary>
        ///     Gets the full description text for the current <see cref="UpdateVersion" />.
        /// </summary>
        public string FullText => DevelopmentalStage != DevelopmentalStage.Release
            ? DevelopmentBuild != 0
                ? $"{BasicVersion} {DevelopmentalStage} {DevelopmentBuild.ToString(CultureInfo.InvariantCulture)}"
                : $"{BasicVersion} {DevelopmentalStage}"
            : BasicVersion;

        /// <summary>
        ///     The major of the version.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     The minor of the version.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     The revision of the version.
        /// </summary>
        public int Revision { get; }

        /// <summary>
        ///     Gets the semantic version string of the current <see cref="UpdateVersion" />.
        /// </summary>
        public string SemanticVersion
        {
            get
            {
                if (DevelopmentalStage != DevelopmentalStage.Release)
                    return
                        $"{Major}.{Minor}.{Build}.{Revision}-{DevelopmentalStage.ToString().Substring(0, 1).ToLower()}.{DevelopmentBuild}"
                            .Replace(".0", string.Empty);
                return BasicVersion;
            }
        }

        public int CompareTo(UpdateVersion version)
        {
            if (this > version)
                return 1;
            return this == version ? 0 : -1;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(UpdateVersion) && ToString() == obj.ToString();
        }

        /// <summary>
        ///     Returns a new <see cref="UpdateVersion" /> from the given full text.
        /// </summary>
        /// <param name="fullText">The full text containing the version information.</param>
        /// <returns>Returns a new <see cref="UpdateVersion" /> from the given full text.</returns>
        /// <exception cref="System.ArgumentException">fullText</exception>
        public static UpdateVersion FromFullText(string fullText)
        {
            var versionSections = fullText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (versionSections.Length > 3)
                throw new ArgumentException("fullText");

            var versionParts = versionSections[0].Split('.');
            var major = int.Parse(versionParts[0]);
            var minor = int.Parse(versionParts[1]);
            var build = int.Parse(versionParts[2]);
            var revision = int.Parse(versionParts[3]);

            if (versionSections.Length == 1)
                return new UpdateVersion(major, minor, build, revision);

            var devStage = DevelopmentalStage.Release;
            switch (versionSections[1])
            {
                case "Alpha":
                    devStage = DevelopmentalStage.Alpha;
                    break;
                case "Beta":
                    devStage = DevelopmentalStage.Beta;
                    break;
                case "ReleaseCandidate":
                    devStage = DevelopmentalStage.ReleaseCandidate;
                    break;
            }

            if (versionSections.Length == 2)
                return new UpdateVersion(major, minor, build, revision, devStage, 0);

            var developmentBuild = int.Parse(versionSections[2]);
            return new UpdateVersion(major, minor, build, revision, devStage, developmentBuild);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var accumulator = 0;

            accumulator |= (Major & 0x0000000F) << 28;
            accumulator |= (Minor & 0x000000FF) << 20;
            accumulator |= (Build & 0x000000FF) << 12;
            accumulator |= Revision & 0x00000FFF;

            return accumulator;
        }

        /// <summary>
        ///     Retuns the highest version in the given collection.
        /// </summary>
        /// <param name="updateVersions">The collection of versions to check.</param>
        /// <returns>Returns the highest version found.</returns>
        public static UpdateVersion GetHighestUpdateVersion(IEnumerable<UpdateVersion> updateVersions)
        {
            var newestVersion = new UpdateVersion();
            foreach (var i in updateVersions)
                if (i > newestVersion)
                    newestVersion = i;

            return newestVersion;
        }

        /// <summary>
        ///     Retuns the lowest version in the given collection.
        /// </summary>
        /// <param name="updateVersions">The collection of versions to check.</param>
        /// <returns>Returns the lowest version found.</returns>
        public static UpdateVersion GetLowestUpdateVersion(IEnumerable<UpdateVersion> updateVersions)
        {
            var enumerable = updateVersions as UpdateVersion[] ?? updateVersions.ToArray();
            var lowestVersion = GetHighestUpdateVersion(enumerable);
            foreach (var i in enumerable)
                if (i < lowestVersion)
                    lowestVersion = i;

            return lowestVersion;
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
            var regex =
                new Regex(
                    @"^(?<Version>((?<VersionNumber>\d+)\.){0,3}(?<VersionNumber>\d+))((-| )?(?<DevStage>(?<Type>[ab]|rc)(\.?(?<DevBuild>\d+))?))?$",
                    RegexOptions.IgnoreCase);
            return regex.IsMatch(versionString);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(UpdateVersion left, UpdateVersion right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);
            return left.ToString() == right.ToString();
        }

        // Operators

        /// <summary>
        ///     Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >(UpdateVersion left, UpdateVersion right)
        {
            if (left.Major > right.Major)
                return true;
            if (left.Major < right.Major)
                return false;

            if (left.Minor > right.Minor)
                return true;
            if (left.Minor < right.Minor)
                return false;

            if (left.Build > right.Build)
                return true;
            if (left.Build < right.Build)
                return false;

            if (left.Revision > right.Revision)
                return true;
            if (left.Revision < right.Revision)
                return false;

            if (left.DevelopmentalStage < right.DevelopmentalStage)
                return true;
            if (left.DevelopmentalStage > right.DevelopmentalStage)
                return false;

            return left.DevelopmentBuild > right.DevelopmentBuild;
        }

        /// <summary>
        ///     Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >=(UpdateVersion left, UpdateVersion right)
        {
            if (left.Major > right.Major)
                return true;
            if (left.Major < right.Major)
                return false;

            if (left.Minor > right.Minor)
                return true;
            if (left.Minor < right.Minor)
                return false;

            if (left.Build > right.Build)
                return true;
            if (left.Build < right.Build)
                return false;

            if (left.Revision > right.Revision)
                return true;
            if (left.Revision < right.Revision)
                return false;

            if (left.DevelopmentalStage < right.DevelopmentalStage)
                return true;
            if (left.DevelopmentalStage > right.DevelopmentalStage)
                return false;

            if (left.DevelopmentBuild > right.DevelopmentBuild)
                return true;
            return !(left.DevelopmentBuild < right.DevelopmentBuild);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(UpdateVersion left, UpdateVersion right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return !ReferenceEquals(left, right);
            return left.ToString() != right.ToString();
        }

        /// <summary>
        ///     Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <(UpdateVersion left, UpdateVersion right)
        {
            if (left.Major < right.Major)
                return true;
            if (left.Major > right.Major)
                return false;

            if (left.Minor < right.Minor)
                return true;
            if (left.Minor > right.Minor)
                return false;

            if (left.Build < right.Build)
                return true;
            if (left.Build > right.Build)
                return false;

            if (left.Revision < right.Revision)
                return true;
            if (left.Revision > right.Revision)
                return false;

            if (left.DevelopmentalStage > right.DevelopmentalStage)
                return true;
            if (left.DevelopmentalStage < right.DevelopmentalStage)
                return false;

            return left.DevelopmentBuild < right.DevelopmentBuild;
        }

        /// <summary>
        ///     Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <=(UpdateVersion left, UpdateVersion right)
        {
            if (left.Major < right.Major)
                return true;
            if (left.Major > right.Major)
                return false;

            if (left.Minor < right.Minor)
                return true;
            if (left.Minor > right.Minor)
                return false;

            if (left.Build < right.Build)
                return true;
            if (left.Build > right.Build)
                return false;

            if (left.Revision < right.Revision)
                return true;
            if (left.Revision > right.Revision)
                return false;

            if (left.DevelopmentalStage > right.DevelopmentalStage)
                return true;
            if (left.DevelopmentalStage < right.DevelopmentalStage)
                return false;

            if (left.DevelopmentBuild < right.DevelopmentBuild)
                return true;
            return !(left.DevelopmentBuild > right.DevelopmentBuild);
        }

        // Overwritten Instance Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            if (DevelopmentalStage != DevelopmentalStage.Release)
            {
                string devStageShortcut = null;
                switch (DevelopmentalStage)
                {
                    case DevelopmentalStage.Alpha:
                        devStageShortcut = "a";
                        break;
                    case DevelopmentalStage.Beta:
                        devStageShortcut = "b";
                        break;
                    case DevelopmentalStage.ReleaseCandidate:
                        devStageShortcut = "rc";
                        break;
                }
                return
                    $"{Major}.{Minor}.{Build}.{Revision}{devStageShortcut}{(DevelopmentBuild != 0 ? DevelopmentBuild.ToString(CultureInfo.InvariantCulture) : string.Empty)}";
            }
            return BasicVersion;
        }
    }
}