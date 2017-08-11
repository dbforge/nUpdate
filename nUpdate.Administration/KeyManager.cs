// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using nUpdate.Administration.Properties;

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
                data = File.ReadAllBytes(PathProvider.KeyDatabaseFilePath);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            if (Settings.Default.UseEncryptedKeyDatabase)
                data = AesCryptoProvider.Decrypt(data, GlobalSession.MasterPassword);
            _secrets = BinarySerializer.DeserializeType<Dictionary<Uri, object>>(data);
        }

        internal object this[Uri identifier]
        {
            get
            {
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                return _secrets[identifier];
            }
            set
            {
                var secret = value;
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));
                if (secret == null)
                    throw new ArgumentNullException(nameof(secret));
                if (!secret.GetType().IsSerializable)
                    throw new ArgumentException("Must be serializable.", nameof(secret));

                if (_secrets.ContainsKey(identifier))
                    _secrets[identifier] = secret;
                else
                    _secrets.Add(identifier, secret);
                Save();
            }
        }

        internal void Save()
        {
            byte[] data = BinarySerializer.Serialize(_secrets);
            if (Settings.Default.UseEncryptedKeyDatabase)
                data = AesCryptoProvider.Encrypt(data, GlobalSession.MasterPassword);
            File.WriteAllBytes(PathProvider.KeyDatabaseFilePath, data);
        }
    }
}