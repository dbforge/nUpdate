using System.Security.Cryptography;
using System.Text;

namespace nUpdate.Security;

/// <summary>
///     Reads and writes RSA keys as PEM: the public key as SubjectPublicKeyInfo (<c>BEGIN PUBLIC KEY</c>), the private
///     key as PKCS#8 (<c>BEGIN PRIVATE KEY</c>). Hand-written DER so that it works on every runtime the library targets.
/// </summary>
internal static class RsaKeyPem
{
    private const string PublicLabel = "PUBLIC KEY";
    private const string PrivateLabel = "PRIVATE KEY";

    private static readonly byte[] RsaAlgorithmIdentifier =
        [0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00];

    public static string EncodePublic(RSAParameters parameters)
    {
        var publicKey = Sequence(Integer(parameters.Modulus), Integer(parameters.Exponent));
        var spki = Sequence(RsaAlgorithmIdentifier, BitString(publicKey));
        return Pem(PublicLabel, spki);
    }

    public static string EncodePrivate(RSAParameters parameters)
    {
        if (parameters.D is null)
            throw new ArgumentException("The parameters hold no private key.", nameof(parameters));
        var rsaPrivateKey = Sequence(Integer([0]), Integer(parameters.Modulus), Integer(parameters.Exponent),
            Integer(parameters.D),
            Integer(parameters.P), Integer(parameters.Q), Integer(parameters.DP), Integer(parameters.DQ),
            Integer(parameters.InverseQ));
        var pkcs8 = Sequence(Integer([0]), RsaAlgorithmIdentifier, OctetString(rsaPrivateKey));
        return Pem(PrivateLabel, pkcs8);
    }

    /// <exception cref="ArgumentException">The text is not a PEM public key.</exception>
    public static RSAParameters DecodePublic(string pem)
    {
        var reader = new DerReader(Unpem(pem, PublicLabel));
        var spki = reader.Sequence();
        spki.Skip(); // the algorithm identifier
        var bits = new DerReader(spki.BitString());
        var key = bits.Sequence();
        var modulus = key.Integer();
        var exponent = key.Integer();
        return new RSAParameters { Modulus = modulus, Exponent = exponent };
    }

    /// <exception cref="ArgumentException">The text is not a PEM private key.</exception>
    public static RSAParameters DecodePrivate(string pem)
    {
        var reader = new DerReader(Unpem(pem, PrivateLabel));
        var pkcs8 = reader.Sequence();
        pkcs8.Integer(); // version
        pkcs8.Skip(); // the algorithm identifier
        var key = new DerReader(pkcs8.OctetString()).Sequence();
        key.Integer(); // version
        var modulus = key.Integer();
        var half = (modulus.Length + 1) / 2;
        return new RSAParameters
        {
            Modulus = modulus,
            Exponent = key.Integer(),
            D = Pad(key.Integer(), modulus.Length),
            P = Pad(key.Integer(), half),
            Q = Pad(key.Integer(), half),
            DP = Pad(key.Integer(), half),
            DQ = Pad(key.Integer(), half),
            InverseQ = Pad(key.Integer(), half),
        };
    }

    public static bool IsPublicKey(string? text) => text is not null && text.Contains($"-----BEGIN {PublicLabel}-----");

    private static string Pem(string label, byte[] der)
    {
        var builder = new StringBuilder();
        builder.Append("-----BEGIN ").Append(label).Append("-----\n");
        var base64 = Convert.ToBase64String(der);
        for (var i = 0; i < base64.Length; i += 64)
            builder.Append(base64, i, Math.Min(64, base64.Length - i)).Append('\n');
        builder.Append("-----END ").Append(label).Append("-----");
        return builder.ToString();
    }

    private static byte[] Unpem(string pem, string label)
    {
        if (pem is null)
            throw new ArgumentNullException(nameof(pem));
        var begin = $"-----BEGIN {label}-----";
        var end = $"-----END {label}-----";
        var start = pem.IndexOf(begin, StringComparison.Ordinal);
        var stop = pem.IndexOf(end, StringComparison.Ordinal);
        if (start < 0 || stop < start)
            throw new ArgumentException($"The text is not a PEM {label.ToLowerInvariant()}.", nameof(pem));
        var body = pem.Substring(start + begin.Length, stop - start - begin.Length);
        try
        {
            return Convert.FromBase64String(body.Replace("\r", string.Empty).Replace("\n", string.Empty)
                .Replace(" ", string.Empty));
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"The PEM {label.ToLowerInvariant()} is not valid Base64.", nameof(pem), ex);
        }
    }

    private static byte[] Sequence(params byte[][] items) => Tagged(0x30, items.SelectMany(i => i).ToArray());

    private static byte[] OctetString(byte[] content) => Tagged(0x04, content);

    private static byte[] BitString(byte[] content) => Tagged(0x03, new byte[] { 0 }.Concat(content).ToArray());

    private static byte[] Integer(byte[]? value)
    {
        if (value is null)
            throw new ArgumentException("The key parameters are incomplete.");
        var start = 0;
        while (start < value.Length - 1 && value[start] == 0)
            start++;
        var trimmed = value.Skip(start).ToArray();
        var content = (trimmed[0] & 0x80) != 0 ? new byte[] { 0 }.Concat(trimmed).ToArray() : trimmed;
        return Tagged(0x02, content);
    }

    private static byte[] Tagged(byte tag, byte[] content)
    {
        var length = content.Length;
        byte[] lengthBytes;
        if (length < 0x80)
        {
            lengthBytes = [(byte)length];
        }
        else
        {
            var bytes = new List<byte>();
            for (var remaining = length; remaining > 0; remaining >>= 8)
                bytes.Insert(0, (byte)(remaining & 0xFF));
            lengthBytes = new[] { (byte)(0x80 | bytes.Count) }.Concat(bytes).ToArray();
        }

        return new[] { tag }.Concat(lengthBytes).Concat(content).ToArray();
    }

    /// <summary>Left-pads an integer (already without leading zeros) to the length <see cref="RSAParameters" /> expects.</summary>
    private static byte[] Pad(byte[] value, int length)
    {
        if (value.Length > length)
            throw new ArgumentException("The key parameters are inconsistent.");
        return new byte[length - value.Length].Concat(value).ToArray();
    }

    /// <summary>Walks a DER byte sequence one element at a time.</summary>
    private sealed class DerReader(byte[] data)
    {
        private int _position;

        public DerReader Sequence() => new(Read(0x30));

        public byte[] Integer()
        {
            var content = Read(0x02);
            var start = 0;
            while (start < content.Length - 1 && content[start] == 0)
                start++;
            return content.Skip(start).ToArray();
        }

        public byte[] OctetString() => Read(0x04);

        public byte[] BitString()
        {
            var content = Read(0x03);
            if (content.Length == 0 || content[0] != 0)
                throw new ArgumentException("The key has an unexpected bit string.");
            return content.Skip(1).ToArray();
        }

        /// <summary>Skips the next element whatever it is.</summary>
        public void Skip()
        {
            if (_position >= data.Length)
                throw new ArgumentException("The key ends unexpectedly.");
            Read(data[_position]);
        }

        private byte[] Read(byte expectedTag)
        {
            if (_position >= data.Length)
                throw new ArgumentException("The key ends unexpectedly.");
            var tag = data[_position++];
            if (tag != expectedTag)
                throw new ArgumentException(
                    $"The key has an unexpected element 0x{tag:X2} where 0x{expectedTag:X2} was expected.");

            if (_position >= data.Length)
                throw new ArgumentException("The key ends unexpectedly.");
            var length = (int)data[_position++];
            if ((length & 0x80) != 0)
            {
                var count = length & 0x7F;
                if (count is 0 or > 3)
                    throw new ArgumentException("The key has an unsupported length encoding.");
                if (_position + count > data.Length)
                    throw new ArgumentException("The key ends unexpectedly.");
                length = 0;
                for (var i = 0; i < count; i++)
                    length = (length << 8) | data[_position++];
            }

            if (length > data.Length - _position)
                throw new ArgumentException("The key ends unexpectedly.");
            var content = new byte[length];
            Array.Copy(data, _position, content, 0, length);
            _position += length;
            return content;
        }
    }
}
