// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;

namespace nUpdate.Administration
{
    internal sealed class KeyManager : Singleton<KeyManager>
    {
        private readonly Dictionary<Uri, object> _secrets = new Dictionary<Uri, object>();

        private KeyManager()
        {
            byte[] data;
            try
            {
                data = File.ReadAllBytes(FilePathProvider.KeyDatabaseFilePath);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            if (Properties.Settings.Default.UsesEncryptedKeyDatabase)
                data = AesCryptoProvider.Decrypt(data, GlobalSession.MasterPassword);
            _secrets = BinarySerializer.DeserializeType<Dictionary<Uri, object>>(data);
        }

        internal object Load(Uri identifer)
        {
            if (identifer == null)
                throw new ArgumentNullException(nameof(identifer));
            return _secrets[identifer];
        }

        private void Save()
        {
            byte[] data = BinarySerializer.Serialize(_secrets);
            if (Properties.Settings.Default.UsesEncryptedKeyDatabase)
                data = AesCryptoProvider.Encrypt(data, GlobalSession.MasterPassword);
            File.WriteAllBytes(FilePathProvider.KeyDatabaseFilePath, data);
        }

        internal void Store(Uri identifer, object secret)
        {
            if (identifer == null)
                throw new ArgumentNullException(nameof(identifer));
            if (secret == null)
                throw new ArgumentNullException(nameof(secret));
            if (!secret.GetType().IsSerializable)
                throw new ArgumentException("Must be serializable.", nameof(secret));

            if (_secrets.ContainsKey(identifer))
                _secrets[identifer] = secret;
            else
                _secrets.Add(identifer, secret);
            Save();
        }
    }
}