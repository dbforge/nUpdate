namespace nUpdate.Administration
{
    internal struct UpdateFactoryPackage
    {
        public UpdateFactoryPackage(string archivePath, DefaultUpdatePackage packageData)
        {
            ArchivePath = archivePath;
            PackageData = packageData;
        }

        internal string ArchivePath { get; set; }
        internal DefaultUpdatePackage PackageData { get; set; }
    }
}