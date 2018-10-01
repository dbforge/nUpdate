using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nUpdate.Test
{
    [TestClass]
    public class UpdaterTest
    {
        private Updater _updater;

        [TestInitialize]
        public void Initialize()
        {
            // TODO: Rewrite test
        }

        [TestMethod]
        public async void CanSearchForUpdates()
        {
            var searchCancellationToken = new CancellationToken();
            bool updatesFound = await _updater.SearchForUpdates(searchCancellationToken);
            Assert.AreEqual(false, updatesFound);
        }
    }
}
