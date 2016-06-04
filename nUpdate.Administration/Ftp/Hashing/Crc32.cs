using System;
using System.Security.Cryptography;

namespace nUpdate.Administration.Ftp.Hashing
{
    internal class Crc32 : HashAlgorithm
    {
        /// <summary>
        /// </summary>
        public const uint DEFAULT_POLYNOMIAL = 0xedb88320;

        /// <summary>
        /// </summary>
        public const uint DEFAULT_SEED = 0xffffffff;

        private static uint[] _defaultTable;

        private readonly uint _seed;
        private readonly uint[] _table;
        private uint _hash;

        /// <summary>
        /// </summary>
        public Crc32()
        {
            _table = InitializeTable(DEFAULT_POLYNOMIAL);
            _seed = DEFAULT_SEED;
            Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="polynomial"></param>
        /// <param name="seed"></param>
        public Crc32(uint polynomial, uint seed)
        {
            _table = InitializeTable(polynomial);
            _seed = seed;
            Initialize();
        }

        /// <summary>
        /// </summary>
        public override int HashSize => 32;

        public override sealed void Initialize()
        {
            _hash = _seed;
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        protected override void HashCore(byte[] buffer, int start, int length)
        {
            _hash = CalculateHash(_table, _hash, buffer, start, length);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            byte[] hashBuffer = UInt32ToBigEndianBytes(~_hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static uint Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DEFAULT_POLYNOMIAL), DEFAULT_SEED, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static uint Compute(uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DEFAULT_POLYNOMIAL), seed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="polynomial"></param>
        /// <param name="seed"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static uint Compute(uint polynomial, uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static uint[] InitializeTable(uint polynomial)
        {
            if (polynomial == DEFAULT_POLYNOMIAL && _defaultTable != null)
                return _defaultTable;

            var createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                var entry = (uint) i;
                for (int j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                createTable[i] = entry;
            }

            if (polynomial == DEFAULT_POLYNOMIAL)
                _defaultTable = createTable;

            return createTable;
        }

        private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint crc = seed;
            for (int i = start; i < size; i++)
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            return crc;
        }

        private byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new[]
            {
                (byte) ((x >> 24) & 0xff),
                (byte) ((x >> 16) & 0xff),
                (byte) ((x >> 8) & 0xff),
                (byte) (x & 0xff)
            };
        }
    }
}