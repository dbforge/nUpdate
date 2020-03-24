// AesCryptoProvider.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System.IO;
using System.Security.Cryptography;

namespace nUpdate.Administration.BusinessLogic
{
// ReSharper disable once InconsistentNaming
    public class AesCryptoProvider
    {
        private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();


        public static byte[] Decrypt(byte[] cipherText, string password)
        {
            var plain = new byte[cipherText.Length - 32];
            using (var ms = new MemoryStream(cipherText, false))
            {
                var salt = new byte[16];
                var iv = new byte[16];

                ms.Read(salt, 0, salt.Length);
                ms.Read(iv, 0, iv.Length);

                // Derive the key from the password and salt that has been read from the cipher text.
                var key = new Rfc2898DeriveBytes(password, salt).GetBytes(32);
                using (var aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.Key = key;
                    aes.IV = iv;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        cs.Read(plain, 0, plain.Length);
                    }
                }
            }

            return plain;
        }

        public static byte[] Encrypt(byte[] plain, string password)
        {
            var salt = new byte[16];
            var iv = new byte[16];

            // Generate random salts.
            _rng.GetBytes(salt);
            _rng.GetBytes(iv);

            // Derive the key from the password using the randomly generated salt.
            var key = new Rfc2898DeriveBytes(password, salt).GetBytes(32);

            // The cipher text length is always a multiple of the block size (16 B).
            // Reserve additional space for salt and IV (32 B).
            var cipherText = new byte[(plain.Length / 16 + 1) * 16 + 32];

            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(cipherText))
                {
                    ms.Write(salt, 0, salt.Length);
                    ms.Write(iv, 0, iv.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plain, 0, plain.Length);
                    }
                }
            }

            return cipherText;
        }
    }
}