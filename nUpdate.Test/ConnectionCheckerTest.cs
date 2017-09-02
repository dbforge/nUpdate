// Author: Dominic Beger (Trade/ProgTrade) 2016

using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class ConnectionCheckerTest
    {
        /// <summary>
        ///     Checks if the "wininet.dll" returns the right stats.
        /// </summary>
        [TestMethod]
        public void CanReturnConnectionStatus()
        {
            Assert.IsTrue(ConnectionManager.IsConnectionAvailable());
        }
    }
}