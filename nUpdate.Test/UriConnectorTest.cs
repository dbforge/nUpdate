// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Internal.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class UriConnectorTest
    {
        private static string _baseUri;
        private static string _baseUriWithoutSlash;
        private static string _endUri;

        /// <summary>
        ///     Initializes the members.
        /// </summary>
        /// <param name="tcx">The additional TestContext.</param>
        [ClassInitialize]
        public static void Initialize(TestContext tcx)
        {
            _baseUri = "http://www.test.de/";
            _baseUriWithoutSlash = "http://www.test.de";
            _endUri = "test";
        }

        /// <summary>
        ///     Should connect the two Uris with a slash at the end of the first Uri.
        /// </summary>
        [TestMethod]
        public void CanConnectUriWithSlash()
        {
            string output = UriConnector.ConnectUri(_baseUri, _endUri).ToString();
            Debug.Print(output);
            Assert.AreEqual("http://www.test.de/test", output);
        }

        /// <summary>
        ///     Should connect the two Uris with no slash at the end of the first Uri.
        /// </summary>
        [TestMethod]
        public void CanConnectUriWithoutSlash()
        {
            string output = UriConnector.ConnectUri(_baseUriWithoutSlash, _endUri).ToString();
            Debug.Write(output);
            Assert.AreEqual("http://www.test.de/test", output);
        }
    }
}