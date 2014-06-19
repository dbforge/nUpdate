using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate;
using nUpdate.Administration;
using nUpdate.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class ConnectionCheckerTest
    {
        /// <summary>
        /// Checks if the "wininet.dll" returns the right stats.
        /// </summary>
        [TestMethod]
        public void CanReturnConnectionStatus()
        {
            Assert.IsTrue(ConnectionChecker.IsConnectionAvailable());
        }
    }
}
