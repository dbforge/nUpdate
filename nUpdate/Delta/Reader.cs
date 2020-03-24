// Reader.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Delta
{
    internal class Reader
    {
        private static readonly int[] ZValue =
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -1, -1, -1, -1, -1,
            -1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
            25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, 36,
            -1, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, -1, -1, -1, 63, -1
        };

        public readonly byte[] A;
        public uint Pos;

        public Reader(byte[] array)
        {
            A = array;
            Pos = 0;
        }

        public byte GetByte()
        {
            var b = A[Pos];
            Pos++;
            if (Pos > A.Length)
                throw new IndexOutOfRangeException("out of bounds");
            return b;
        }

        public char GetChar()
        {
            //  return String.fromCharCode(this.getByte());
            return (char) GetByte();
        }

        /**
		 * Read bytes from *pz and convert them into a positive integer.  When
		 * finished, leave *pz pointing to the first character past the end of
		 * the integer.  The *pLen parameter holds the length of the string
		 * in *pz and is decremented once for each character in the integer.
		 */
        public uint GetInt()
        {
            uint v = 0;
            int c;
            while (HaveBytes() && (c = ZValue[0x7f & GetByte()]) >= 0) v = (uint) (((int) v << 6) + c);
            Pos--;
            return v;
        }

        public bool HaveBytes()
        {
            return Pos < A.Length;
        }
    }
}