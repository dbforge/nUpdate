﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using nUpdate.Core;

namespace nUpdate.Internal
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
            var regex = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+([a-b][0-9]+|)", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(version))
                throw new ArgumentException("Version is not valid.");

            string[] versionParts = version.Split(new[] {'.'});

            Major = int.Parse(versionParts[0]);
            Minor = int.Parse(versionParts[1]);
            Build = int.Parse(versionParts[2]);

            string[] parts;
            if (versionParts[3].Contains("a"))
            {
                DevelopmentalStage = DevelopmentalStage.Alpha;
                parts = versionParts[3].Split(new[] {'a'});
                Revision = int.Parse(parts[0]);
                DevelopmentBuild = int.Parse(parts[1]);
            }
            else if (versionParts[3].Contains("b"))
            {
                DevelopmentalStage = DevelopmentalStage.Beta;
                parts = versionParts[3].Split(new[] {'b'});
                Revision = int.Parse(parts[0]);
                DevelopmentBuild = int.Parse(parts[1]);
            }
            else
            {
                DevelopmentalStage = DevelopmentalStage.Release;
                Revision = int.Parse(versionParts[3]);
            }

            if (Major < 0)
                throw new ArgumentOutOfRangeException("major", "Index must be 0 or higher");

            if (Minor < 0)
                throw new ArgumentOutOfRangeException("minor", "Index must be 0 or higher");

            if (Build < 0)
                throw new ArgumentOutOfRangeException("build", "Index must be 0 or higher");

            if (Revision < 0)
                throw new ArgumentOutOfRangeException("revision", "Index must be 0 or higher");
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="build">The build version.</param>
        /// <param name="revision">The revision version.</param>
        public UpdateVersion(int major, int minor, int build, int revision)
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
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateVersion" />-class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="build">The build version.</param>
        /// <param name="revision">The revision version.</param>
        /// <param name="devStage">The developmental stage.</param>
        /// <param name="developmentStage">The pre-release version.</param>
        public UpdateVersion(int major, int minor, int build, int revision, DevelopmentalStage devStage,
            int developmentStage)
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
            DevelopmentBuild = developmentStage;
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
        ///     Returns the full description text for the update version.
        /// </summary>
        public string FullText
        {
            get
            {
                if (DevelopmentalStage != DevelopmentalStage.Release)
                    return String.Format("{0} {1} {2}", LiteralUpdateVersion, DevelopmentalStage, DevelopmentBuild);
                return LiteralUpdateVersion;
            }
        }

        /// <summary>
        ///     Returns the current version without the developmental stage and development build.
        /// </summary>
        public string LiteralUpdateVersion
        {
            get { return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision); }
        }

        // Overwritten Instance Methods
        public override string ToString()
        {
            if (DevelopmentalStage != DevelopmentalStage.Release)
            {
                return String.Format("{0}.{1}.{2}.{3}{4}{5}", Major, Minor, Build, Revision,
                    DevelopmentalStage.ToString().Substring(0, 1).ToLower(), DevelopmentBuild);
            }
            return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
        }

        public override int GetHashCode()
        {
            int accumulator = 0;

            accumulator |= (Major & 0x0000000F) << 28;
            accumulator |= (Minor & 0x000000FF) << 20;
            accumulator |= (Build & 0x000000FF) << 12;
            accumulator |= (Revision & 0x00000FFF);

            return accumulator;
        }

        // Operators
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

            if (left.DevelopmentBuild > right.DevelopmentBuild)
                return true;
            if (left.DevelopmentBuild < right.DevelopmentBuild)
                return false;

            // Versions are exactly equal
            return false;
        }

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

            if (left.DevelopmentBuild < right.DevelopmentBuild)
                return true;
            if (left.DevelopmentBuild > right.DevelopmentBuild)
                return false;

            // Versions are exactly equal
            return false;
        }

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
            if (left.DevelopmentBuild > right.DevelopmentBuild)
                return false;

            // Versions are exactly equal
            return true;
        }

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
            if (left.DevelopmentBuild < right.DevelopmentBuild)
                return false;

            // Versions are exactly equal
            return true;
        }

        public static bool operator ==(UpdateVersion left, UpdateVersion right)
        {
            return left != null && (right != null && (left.ToString() == right.ToString()));
        }

        public static bool operator !=(UpdateVersion left, UpdateVersion right)
        {
            return left != null && (right != null && (left.ToString() != right.ToString()));
        }

        /// <summary>
        ///     Retuns the highest version in the given collection.
        /// </summary>
        /// <param name="updateVersions">The collection of versions to check.</param>
        /// <returns>Returns the highest version found.</returns>
        public static UpdateVersion GetHighestUpdateVersion(IEnumerable<UpdateVersion> updateVersions)
        {
            UpdateVersion[] newestVersion = {new UpdateVersion()};
            foreach (UpdateVersion i in updateVersions.Where(i => i > newestVersion[0]))
            {
                newestVersion[0] = i;
            }

            return newestVersion[0];
        }
    }
}