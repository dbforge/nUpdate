// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using nUpdate.Core;

namespace nUpdate.Updating
{
    public class UpdateVersion
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
            if (!IsValid(version))
                throw new ArgumentException("The specified version is not valid.");

            var versionParts = version.Split('.');

            Major = int.Parse(versionParts[0]);
            Minor = int.Parse(versionParts[1]);
            Build = int.Parse(versionParts[2]);

            string[] parts;
            if (versionParts[3].Contains("a"))
            {
                DevelopmentalStage = DevelopmentalStage.Alpha;
                parts = versionParts[3].Split('a');
                Revision = int.Parse(parts[0]);
                DevelopmentBuild = int.Parse(parts[1]);
            }
            else if (versionParts[3].Contains("b"))
            {
                DevelopmentalStage = DevelopmentalStage.Beta;
                parts = versionParts[3].Split('b');
                Revision = int.Parse(parts[0]);
                DevelopmentBuild = int.Parse(parts[1]);
            }
            else
            {
                DevelopmentalStage = DevelopmentalStage.Release;
                Revision = int.Parse(versionParts[3]);
            }
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
                throw new ArgumentOutOfRangeException("major", "Index must be 0 or higher");

            if (minor < 0)
                throw new ArgumentOutOfRangeException("minor", "Index must be 0 or higher");

            if (build < 0)
                throw new ArgumentOutOfRangeException("build", "Index must be 0 or higher");

            if (revision < 0)
                throw new ArgumentOutOfRangeException("revision", "Index must be 0 or higher");

            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
            DevelopmentalStage = devStage;
            DevelopmentBuild = developmentBuild;
        }

        /// <summary>
        ///     The major of the version.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        ///     The minor of the version.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        ///     The build of the version.
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        ///     The revision of the version.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        ///     The developmental stage of the version.
        /// </summary>
        public DevelopmentalStage DevelopmentalStage { get; set; }

        /// <summary>
        ///     The build version of the alpha or beta.
        /// </summary>
        public int DevelopmentBuild { get; set; }

        /// <summary>
        ///     Returns the full description text for the current <see cref="UpdateVersion"/>.
        /// </summary>
        public string FullText
        {
            get
            {
                return DevelopmentalStage != DevelopmentalStage.Release
                    ? String.Format("{0} {1} {2}", BasicVersion, DevelopmentalStage, DevelopmentBuild)
                    : BasicVersion;
            }
        }

        /// <summary>
        ///     Returns the current <see cref="UpdateVersion"/> without the developmental stage and development build.
        /// </summary>
        public string BasicVersion
        {
            get { return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision); }
        }

        // Overwritten Instance Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            if (DevelopmentalStage != DevelopmentalStage.Release)
            {
                return String.Format("{0}.{1}.{2}.{3}{4}{5}", Major, Minor, Build, Revision,
                    DevelopmentalStage.ToString().Substring(0, 1).ToLower(), DevelopmentBuild);
            }
            return BasicVersion;
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
            accumulator |= (Revision & 0x00000FFF);

            return accumulator;
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
        ///     Retuns the highest version in the given collection.
        /// </summary>
        /// <param name="updateVersions">The collection of versions to check.</param>
        /// <returns>Returns the highest version found.</returns>
        public static UpdateVersion GetHighestUpdateVersion(IEnumerable<UpdateVersion> updateVersions)
        {
            var newestVersion = new UpdateVersion();
            foreach (var i in updateVersions)
            {
                if (i > newestVersion)
                    newestVersion = i;
            }

            return newestVersion;
        }

        /// <summary>
        ///     Returns a new <see cref="UpdateVersion"/> from the given full text.
        /// </summary>
        /// <param name="fullText">The full text containing the version information.</param>
        /// <returns>Returns a new <see cref="UpdateVersion"/> from the given full text.</returns>
        /// <exception cref="System.ArgumentException">fullText</exception>
        public static UpdateVersion FromFullText(string fullText)
        {
            var versionSections = fullText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (versionSections.Length != 1 && versionSections.Length != 3)
                throw new ArgumentException("fullText");

            var versionParts = versionSections[0].Split('.');
            int major = int.Parse(versionParts[0]);
            int minor = int.Parse(versionParts[1]);
            int build = int.Parse(versionParts[2]);
            int revision = int.Parse(versionParts[3]);

            if (versionSections.Length == 1)
                return new UpdateVersion(major, minor, build, revision);

            DevelopmentalStage devStage = DevelopmentalStage.Release;
            switch (versionSections[1])
            {
                case "Alpha":
                    devStage = DevelopmentalStage.Alpha;
                    break;
                case "Beta:":
                    devStage = DevelopmentalStage.Beta;
                    break;
            }

            int developmentBuild = int.Parse(versionSections[2]);
            return new UpdateVersion(major, minor, build, revision, devStage, developmentBuild);
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
            var regex = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+([a-b][0-9]+|)", RegexOptions.IgnoreCase);
            return regex.IsMatch(versionString);
        }
    }
}