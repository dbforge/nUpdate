using System;

namespace nUpdate
{
    public class ClassicVersion : IVersion<ClassicVersion>
    {
        /// <summary>
        ///     Gets the major version.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets the minor version.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets the revision version.
        /// </summary>
        public int Revision { get; }

        /// <summary>
        ///     Gets the build version.
        /// </summary>
        public int Build { get; }

        public int CompareTo(ClassicVersion other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ClassicVersion other)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool HasPreRelease => false;

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(ClassicVersion))
                throw new InvalidOperationException();

            return CompareTo((ClassicVersion) obj);
        }
    }
}
