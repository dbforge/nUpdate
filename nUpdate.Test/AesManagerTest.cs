// AesManagerTest.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Administration.BusinessLogic;

namespace nUpdate.Test
{
    [TestClass]
    public class AesManagerTest
    {
        [TestMethod]
        public void CanEncryptAndDecrypt()
        {
            var plain = Encoding.UTF8.GetBytes("test");
            var cipherText = AesCryptoProvider.Encrypt(plain, "master");
            var decrypted = AesCryptoProvider.Decrypt(cipherText, "master");
            Assert.AreEqual(Encoding.UTF8.GetString(decrypted).TrimEnd('\0'), "test");
        }
    }
}