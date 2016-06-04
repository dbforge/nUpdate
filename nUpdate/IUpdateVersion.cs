using System;

namespace nUpdate
{
    public interface IUpdateVersion : IComparable<IUpdateVersion>
    {
        /// <summary>
        ///     Gets or sets the major of this <see cref="UpdateVersion"/>.
        /// </summary>
        int Major { get; set; }

        /// <summary>
        ///     Gets or sets the minor of this <see cref="UpdateVersion"/>.
        /// </summary>
        int Minor { get; set; }

        /// <summary>
        ///     Gets or sets the build of this <see cref="UpdateVersion"/>.
        /// </summary>
        int Build { get; set; }

        /// <summary>
        ///     Gets or sets the revision of this <see cref="UpdateVersion"/>.
        /// </summary>
        int Revision { get; set; }

        /// <summary>
        ///     Gets or sets the developmental stage of this <see cref="UpdateVersion"/>.
        /// </summary>
        DevelopmentalStage DevelopmentalStage { get; set; }

        /// <summary>
        ///     Gets or sets the development build of this <see cref="UpdateVersion"/>.
        /// </summary>
        int DevelopmentBuild { get; set; }

        /// <summary>
        ///     Gets the full description text of this <see cref="UpdateVersion" />.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets the current <see cref="UpdateVersion" /> without the developmental stage and development build.
        /// </summary>
        string BasicVersion { get; }

        //int CompareTo(UpdateVersion version);

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        string ToString();

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        int GetHashCode();

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        bool Equals(object obj);

        /// <summary>
        ///     Determines whether this version is equal to the specified <see cref="IUpdateVersion"/>.
        /// </summary>
        /// <param name="version">The version to compare this one with.</param>
        /// <returns>Returns <c>true</c>, if the versions are equal, otherwise <c>false</c>.</returns>
        bool IsEqualTo(IUpdateVersion version);

        /// <summary>
        ///     Determines whether this version is newer than the specified <see cref="IUpdateVersion"/>.
        /// </summary>
        /// <param name="version">The version to compare this one with.</param>
        /// <returns>Returns <c>true</c>, if this version is newer, otherwise <c>false</c>.</returns>
        bool IsNewerThan(IUpdateVersion version);

        /// <summary>
        ///     Determines whether this version is older than the specified <see cref="IUpdateVersion"/>.
        /// </summary>
        /// <param name="version">The version to compare this one with.</param>
        /// <returns>Returns <c>true</c>, if this version is older, otherwise <c>false</c>.</returns>
        bool IsOlderThan(IUpdateVersion version);

        /// <summary>
        ///     Determines whether this version is newer than the specified <see cref="IUpdateVersion"/> or equal to it.
        /// </summary>
        /// <param name="version">The version to compare this one with.</param>
        /// <returns>Returns <c>true</c>, if this version is newer or equal, otherwise <c>false</c>.</returns>
        bool IsNewerOrEqualTo(IUpdateVersion version);

        /// <summary>
        ///     Determines whether this version is older than the specified <see cref="IUpdateVersion"/> or equal to it.
        /// </summary>
        /// <param name="version">The version to compare this one with.</param>
        /// <returns>Returns <c>true</c>, if this version is older or equal, otherwise <c>false</c>.</returns>
        bool IsOlderOrEqualTo(IUpdateVersion version);
    }
}