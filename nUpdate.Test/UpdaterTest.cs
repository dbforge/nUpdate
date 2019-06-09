using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nUpdate.Test
{
    [TestClass]
    public class UpdaterTest
    {
        private HttpUpdateProvider _updater;

        [TestInitialize]
        public void Initialize()
        {
            // TODO: Rewrite test
        }

        [TestMethod]
        public async void CanSearchForUpdates()
        {
            bool updatesFound = (await _updater.BeginUpdateCheck()).UpdatesFound;
            Assert.AreEqual(false, updatesFound);
        }
    }
}
