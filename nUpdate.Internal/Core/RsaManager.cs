// Copyright © Dominic Beger 2017

using System;
using System.IO;
using System.Security.Cryptography;

namespace nUpdate.Internal.Core
{
    /// <summary>
    ///     Provides methods and properties to sign and verify data with the RSACryptoServiceProvider.
    /// </summary>
    public class RsaManager : IDisposable
    {
        /// <summary>
        ///     The default key size in bits.
        /// </summary>
        public const int DefaultKeySize = 8192;

        private readonly RSACryptoServiceProvider _rsa;
        private bool _disposed;

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" />-class.
        /// </summary>
        /// <param name="rsaKey">The public key to use.</param>
        public RsaManager(string rsaKey)
        {
            if (string.IsNullOrEmpty(rsaKey))
                throw new ArgumentNullException(nameof(rsaKey));

            _rsa = new RSACryptoServiceProvider();
            _rsa.FromXmlString(rsaKey);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RsaManager" />-class and creates a new key pair.
        /// </summary>
        public RsaManager()
        {
            _rsa = new RSACryptoServiceProvider(DefaultKeySize);
            _rsa.ToXmlString(true);
            _rsa.PersistKeyInCsp = false;
        }

        /// <summary>
        ///     Returns the private key.
        /// </summary>
        public string PrivateKey => _rsa.ToXmlString(true);

        /// <summary>
        ///     Returns the public key.
        /// </summary>
        public string PublicKey => _rsa.ToXmlString(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _rsa.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     Calculates the signature for the given data.
        /// </summary>
        /// <param name="data">The data to calculate the signature for.</param>
        /// <returns>The calculated signature.</returns>
        public byte[] SignData(byte[] data)
        {
            return _rsa.SignData(data, typeof(SHA512));
        }

        /// <summary>
        ///     Calculates the signature for the given data.
        /// </summary>
        /// <param name="stream">The stream containing the data.</param>
        /// <returns>The calculated signature.</returns>
        public byte[] SignData(Stream stream)
        {
            return _rsa.SignData(stream, typeof(SHA512));
        }

        /// <summary>
        ///     Checks if the signature for the given data is valid.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="signature">The signature to check.</param>
        /// <returns>Returns "true" if the signature is correct, otherwise "false".</returns>
        public bool VerifyData(byte[] data, byte[] signature)
        {
            return _rsa.VerifyData(data, typeof(SHA512), signature);
        }

        /// <summary>
        ///     Checks if the signature for the given data is valid.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="signature">The stream containing the data.</param>
        /// <returns>Returns "true" if the signature is correct, otherwise "false".</returns>
        public bool VerifyData(Stream stream, byte[] signature)
        {
            return VerifyDataInternal(stream, "SHA512", signature); // TODO: typeof(SHA512)
        }

        private bool VerifyDataInternal(Stream stream, object halg, byte[] signature)
        {
            var hash = (HashAlgorithm) CryptoConfig.CreateFromName((string) halg);
            //HashAlgorithm hash = (HashAlgorithm)halg;
            var hashVal = hash.ComputeHash(stream);
            return _rsa.VerifyHash(hashVal, (string) halg, signature);
        }
    }
}