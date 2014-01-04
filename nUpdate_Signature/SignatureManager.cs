/**
 *  Klasse zum Signieren von Daten mit Hilfe des RSA-Algorithmus
 *  
 *  Author: Tim Schiewe (timmi31061)
 *  Lizenz: GPL v3
 *  Erstellt: 09. Dezember 2013
 *  Modifiziert:  Trade - 15.12.13 - Design des Konstruktors verbessert
 *  Modifiziert:  Tim Schiewe (timmi31061) - 04.01.14 - Parametervalidation zum Konstruktor hinzugefügt; mehr IntelliSense
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace nUpdate.SignatureManager
{
    /// <summary>
    /// Klasse zum Signieren von Daten mit Hilfe des RSA-Algorithmus
    /// </summary>
    public class RsaSignature
    {
        /// <summary>
        /// Die Standardschlüsselgröße in Bits
        /// </summary>
        public const int DefaultKeySize = 4096; 

        private RSACryptoServiceProvider rsa;

        /// <summary>
        /// Gibt den öffentlichen Schlüssel zurück.
        /// </summary>
        public string PublicKey
        {
            get
            {
                return this.rsa.ToXmlString(false); // Öffentlichen Schlüssel exportieren.
            }
        }

        /// <summary>
        /// Gibt den privaten Schlüssel zurück.
        /// </summary>
        public string PrivateKey
        {
            get
            {
                return this.rsa.ToXmlString(true);  // Privaten und öffentlichen Schlüssel exportieren.
            }
        }

        /// <summary>
        /// Erstellt eine neue RsaSignature-Instanz.
        /// </summary>
        /// <param name="rsaKey">Der zu verwendene RSA-Schlüssel.</param>
        public RsaSignature(string rsaKey)
        {
            if (string.IsNullOrWhiteSpace(rsaKey))          // Wenn kein oder ein leerer Schlüssel übergeben wurde...
            {
                throw new ArgumentNullException("rsaKey");  // ArgumentNullException werfen.
            }
            
            this.rsa = new RSACryptoServiceProvider();      // Schlüssel wurde angegeben...
            this.rsa.FromXmlString(rsaKey);                 // ...also wird dieser importiert.
            
        }

        /// <summary>
        /// Erstellt eine neue RsaSignature-Instanz und einen Schlüssel mit der Standardschlüsselgröße.
        /// </summary>
        public RsaSignature()
        {
            this.rsa = new RSACryptoServiceProvider(DefaultKeySize);    // Einen neuen Schlüssel mit der Standardschlüsselgröße erstellen.
            this.rsa.ToXmlString(true);                                 // Ein Dummy, um den Schlüssel zu erstellen.
        }

        /// <summary>
        /// Berechnet die Signatur für die angegebenen Daten.
        /// </summary>
        /// <param name="data">Die Daten, für die die Signatur berechnet werden soll.</param>
        /// <returns>Die berechnete Signatur.</returns>
        public byte[] SignData(byte[] data)
        {
            return this.rsa.SignData(data, typeof(SHA512));  // Berechnet die Signatur und gibt sie zurück.
        }

        /// <summary>
        /// Überprüft die angegebene Signatur für die angegebenen Daten.
        /// </summary>
        /// <param name="data">Die Daten, für die die Signatur überprüft werden soll.</param>
        /// <param name="signature">Die Signatur, die überprüft werden soll.</param>
        /// <returns>True, wenn die Signatur gültig ist, andernfalls false.</returns>
        public bool VerifyData(byte[] data, byte[] signature)
        {
            return this.rsa.VerifyData(data, typeof(SHA512), signature); // Überprüft, ob die berechnete Signatur für diese Daten gültig ist.
        }
    }
}
