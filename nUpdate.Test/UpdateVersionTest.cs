// Author: Dominic Beger (Trade/ProgTrade)

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Updating;

namespace nUpdate.Test
{
    [TestClass]
    public class UpdateVersionTest
    {
        [TestMethod]
        public void CanCheckVersionValidity()
        {
            var alphaVersion = new UpdateVersion("1.0.0.0a1");
            var betaVersion = new UpdateVersion("1.0.0.0b1");
            var normalVersion = new UpdateVersion("1.0.0.0");
            // var firstInvalidVersion = new UpdateVersion("1.0.0.0b"); // Invalid
            // var secondInvalidVersion = new UpdateVersion("1.0.0.0c1"); // Invalid
        }

        [TestMethod]
        public void CanGetHighestVersion()
        {
            var versionArray = new[]
            {"1.0.0.0", "1.1.0.0", "1.2.0.0a1", "1.3.0.0b1", "1.2.0.0b3", "1.3.0.0b3", "1.3.0.1b97", "1.1.1.0"};
            var versions = versionArray.Select(entry => new UpdateVersion(entry)).ToList();

            Assert.AreEqual("1.3.0.1b97", UpdateVersion.GetHighestUpdateVersion(versions).ToString());
        }
    }
}