using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate;
using nUpdate.Administration;
using nUpdate.Core;
using System.Diagnostics;

namespace nUpdate.Test
{
    [TestClass]
    public class UriConnecterTest
    {
        static string baseUri;
        static string baseUriWithoutSlash;
        static string endUri;


        /// <summary>
        /// Initializes the members.
        /// </summary>
        /// <param name="tcx">The additional TestContext.</param>
        [ClassInitialize]
        public static void Initialize(TestContext tcx)
        {
            baseUri = "http://www.test.de/";
            baseUriWithoutSlash = "http://www.test.de";
            endUri = "test";
        }

        /// <summary>
        /// Should connect the two Uris with a slash at the end of the first Uri.
        /// </summary>
        [TestMethod]
        public void CanConnectUriWithSlash()
        {
            UriConnecter uriConnecter = new UriConnecter();
            string output = uriConnecter.ConnectUri(baseUri, endUri).ToString();
            Debug.Print(output);
            Assert.AreEqual("http://www.test.de/test", output);
        }

        /// <summary>
        /// Should connect the two Uris with no slash at the end of the first Uri.
        /// </summary>
        [TestMethod]
        public void CanConnectUriWithoutSlash()
        {
            UriConnecter uriConnecter = new UriConnecter();
            string output = uriConnecter.ConnectUri(baseUriWithoutSlash, endUri).ToString();
            Debug.Write(output);
            Assert.AreEqual("http://www.test.de/test", output);
        }
    }
}
