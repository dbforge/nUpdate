namespace nUpdate.Administration
{
    internal struct UpdateFactoryPackage
    {
        public UpdateFactoryPackage(string archivePath, UpdatePackage packageData)
        {
            ArchivePath = archivePath;
            PackageData = packageData;
        }

        internal string ArchivePath { get; set; }
        internal UpdatePackage PackageData { get; set; }
    }
}