// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Core;

namespace nUpdate.Test
{
    [TestClass]
    public class RsaSignatureTest
    {
        private static byte[] data;
        private static string desktopPath;
        private static string keyFile;
        private static string signatureFile;

        /// <summary>
        ///     Initializes the members.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            data = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            keyFile = Path.Combine(desktopPath, "keys.txt");
            signatureFile = Path.Combine(desktopPath, "Signature.txt");

            FileStream keyStream = File.Create(keyFile);
            keyStream.Flush();
            keyStream.Close();

            FileStream signatureStream = File.Create(signatureFile);
            signatureStream.Flush();
            signatureStream.Close();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            File.Delete(keyFile);
            File.Delete(signatureFile);
        }

        /// <summary>
        ///     Checks if the signing of data works.
        /// </summary>
        [TestMethod]
        public void CanSignAndValidateData()
        {
            var rsa = new RsaManager();
            byte[] signature = rsa.SignData(data);

            Assert.IsTrue(rsa.VerifyData(data, signature));
        }

        /// <summary>
        ///     Checks if the signing of data works when saving the keys.
        /// </summary>
        [TestMethod]
        public void CanSignAndValidateDataWithSaving()
        {
            var rsa = new RsaManager();
            byte[] signature = rsa.SignData(data);

            File.WriteAllBytes(signatureFile, signature);
            File.WriteAllText(keyFile, rsa.PublicKey);

            var givenRsa = new RsaManager(File.ReadAllText(keyFile));
            byte[] givenSignature = File.ReadAllBytes(signatureFile);

            Assert.IsTrue(givenRsa.VerifyData(data, givenSignature));
        }
    }
}