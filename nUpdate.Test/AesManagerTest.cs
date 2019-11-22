// Author: Dominic Beger (Trade/ProgTrade)

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
            byte[] plain = Encoding.UTF8.GetBytes("test");
            var cipherText = AesCryptoProvider.Encrypt(plain, "master");
            byte[] decrypted = AesCryptoProvider.Decrypt(cipherText, "master");
            Assert.AreEqual(Encoding.UTF8.GetString(decrypted).TrimEnd('\0'), "test");
        }
    }
}