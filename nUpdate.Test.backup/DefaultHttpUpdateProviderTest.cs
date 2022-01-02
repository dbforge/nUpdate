using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace nUpdate.Test
{
    [TestClass]
    public class DefaultHttpUpdateProviderTest
    {
        private DefaultHttpUpdateProvider _defaultHttpUpdateProvider;

        [TestInitialize]
        public void Initialize()
        {
            _defaultHttpUpdateProvider = new DefaultHttpUpdateProvider(new Uri("https://www.example.com"), "PUBLICKEY",
                Mock.Of<IVersion>(v => v.CompareTo(It.IsAny<IVersion>()) == -1), UpdateChannelFilter.None);
        }

        [TestMethod]
        public void CanCheckForUpdatesWithoutUpdateChannelFilter()
        {
            var testPackages = new List<UpdatePackage> {
                new UpdatePackage {Guid = Guid.NewGuid()}, 
                new UpdatePackage {Guid = Guid.NewGuid()}
            };

            string premadePackageResponse = JsonSerializer.Serialize(testPackages);
            var webClientMock = new Mock<ICustomWebClient>();
            webClientMock.Setup(w => w.DownloadStringFrom(_defaultHttpUpdateProvider.PackageDataFile))
                .ReturnsAsync(premadePackageResponse);

            var updateCheckResult = _defaultHttpUpdateProvider.CheckForUpdates(new CancellationToken());
            Assert.AreEqual(testPackages, updateCheckResult.Result.Packages);
        }
    }
}
