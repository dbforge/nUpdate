// RollingHash.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Delta
{
    internal class RollingHash
    {
        private ushort _a;
        private ushort _b;
        private ushort _i;
        private readonly byte[] _z;

        public RollingHash()
        {
            _a = 0;
            _b = 0;
            _i = 0;
            _z = new byte[Delta.Nhash];
        }

        /**
		 * Initialize the rolling hash using the first NHASH characters of z[]
		 */
        public void Init(byte[] z, int pos)
        {
            ushort a = 0, b = 0, i, x;
            for (i = 0; i < Delta.Nhash; i++)
            {
                x = z[pos + i];
                a = (ushort) ((a + x) & 0xffff);
                b = (ushort) ((b + (Delta.Nhash - i) * x) & 0xffff);
                _z[i] = (byte) x;
            }

            _a = (ushort) (a & 0xffff);
            _b = (ushort) (b & 0xffff);
            _i = 0;
        }

        /**
		 * Advance the rolling hash by a single character "c"
		 */
        public void Next(byte c)
        {
            ushort old = _z[_i];
            _z[_i] = c;
            _i = (ushort) ((_i + 1) & (Delta.Nhash - 1));
            _a = (ushort) (_a - old + c);
            _b = (ushort) (_b - Delta.Nhash * old + _a);
        }

        /*
		 * Compute a hash on NHASH bytes.
		 *
		 * This routine is intended to be equivalent to:
		 *    hash h;
		 *    hash_init(&h, zInput);
		 *    return hash_32bit(&h);
		 */
        public static uint Once(byte[] z)
        {
            ushort a, b, i;
            a = b = z[0];
            for (i = 1; i < Delta.Nhash; i++)
            {
                a += z[i];
                b += a;
            }

            return a | ((uint) b << 16);
        }


        /**
		 * Return a 32-bit hash value
		 */
        public uint Value()
        {
            return (uint) (_a & 0xffff) | ((uint) (_b & 0xffff) << 16);
        }
    }
}