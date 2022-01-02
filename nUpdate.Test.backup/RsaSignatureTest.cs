﻿// RsaSignatureTest.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nUpdate.Test
{
    [TestClass]
    public class RsaSignatureTest
    {
        private static byte[] _data;
        private static string _desktopPath;
        private static string _keyFile;
        private static string _signatureFile;

        /// <summary>
        ///     Checks if the signing of data works.
        /// </summary>
        [TestMethod]
        public void CanSignAndValidateData()
        {
            var rsa = new RsaManager();
            var signature = rsa.SignData(_data);

            Assert.IsTrue(rsa.VerifyData(_data, signature));
        }

        /// <summary>
        ///     Checks if the signing of data works when saving the keys.
        /// </summary>
        [TestMethod]
        public void CanSignAndValidateDataWithSaving()
        {
            var rsa = new RsaManager();
            var signature = rsa.SignData(_data);

            File.WriteAllBytes(_signatureFile, signature);
            File.WriteAllText(_keyFile, rsa.PublicKey);

            var givenRsa = new RsaManager(File.ReadAllText(_keyFile));
            var givenSignature = File.ReadAllBytes(_signatureFile);

            Assert.IsTrue(givenRsa.VerifyData(_data, givenSignature));
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            File.Delete(_keyFile);
            File.Delete(_signatureFile);
        }

        /// <summary>
        ///     Initializes the members.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _data = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            _keyFile = Path.Combine(_desktopPath, "keys.txt");
            _signatureFile = Path.Combine(_desktopPath, "Signature.txt");

            var keyStream = File.Create(_keyFile);
            keyStream.Flush();
            keyStream.Close();

            var signatureStream = File.Create(_signatureFile);
            signatureStream.Flush();
            signatureStream.Close();
        }
    }
}