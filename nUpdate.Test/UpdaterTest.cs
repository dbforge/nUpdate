using System;
using System.Globalization;
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
            //File.WriteAllText(@"D:\Desktop\en.json", Serializer.Serialize(new LocalizationProperties()));
            _updater = new Updater(new Uri("http://www.nupdate.net/test/"), "Test")
            {
                LanguageCulture = new CultureInfo("de-DE")
            };
        }

        [TestMethod]
        public async void CanSearchForUpdates()
        {
            var searchCancellationToken = new CancellationToken();
            bool updatesFound = await _updater.SearchForUpdatesTask(searchCancellationToken);
            Assert.AreEqual(false, updatesFound);
        }
    }
}
