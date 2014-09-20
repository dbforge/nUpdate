using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Internal;

namespace nUpdate.Test
{
    [TestClass]
    public class UpdateVersionTest
    {
        [TestMethod]
        public void CanGetHighestVersion()
        {
            var versions = new List<UpdateVersion>();
            var versionArray = new[]
            {"1.0.0.0", "1.1.0.0", "1.2.0.0a1", "1.3.0.0b1", "1.2.0.0b3", "1.3.0.0b3", "1.3.0.1b97", "1.1.1.0"};
            foreach (string entry in versionArray)
            {
                versions.Add(new UpdateVersion(entry));
            }

            Assert.AreEqual("1.3.0.1b97", UpdateVersion.GetHighestUpdateVersion(versions).ToString());
        }
    }
}