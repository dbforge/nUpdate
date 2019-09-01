using System;

namespace nUpdate
{
    public class DotNetVersion : IVersion<DotNetVersion>
    {
        public DotNetVersion(int major, int minor, int build, int revision)
         : this(new Version(major, minor, build, revision))
        { }

        public DotNetVersion(Version version)
        {
            Version = version;
        }

        public Version Version { get; }
        public int Major => Version.Major;
        public short MajorRevision => Version.MajorRevision;
        public int Minor => Version.Minor;
        public short MinorRevision => Version.MinorRevision;
        public int Build => Version.Build;
        public int Revision => Version.Revision;

        public int CompareTo(DotNetVersion other)
        {
            return Version.CompareTo(other.Version);
        }

        public bool Equals(DotNetVersion other)
        {
            return other != null && Version.Equals(other.Version);
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool HasPreRelease => false;
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DotNetVersion))
                throw new InvalidOperationException();

            return CompareTo((DotNetVersion) obj);
        }
    }
}
