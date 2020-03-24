// Writer.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Collections.Generic;

namespace nUpdate.Delta
{
    internal class Writer
    {
        private static readonly uint[] ZDigits =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '_', 'a', 'b', 'c', 'd', 'e',
            'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
            't', 'u', 'v', 'w', 'x', 'y', 'z', '~'
        };

        private readonly List<byte> _a;

        public Writer()
        {
            _a = new List<byte>();
        }

        public void PutArray(byte[] b, int start, int end)
        {
            for (var i = start; i < end; i++) _a.Add(b[i]);
        }

        public void PutChar(char c)
        {
            _a.Add((byte) c);
        }

        public void PutInt(uint v)
        {
            int i, j;
            var zBuf = new uint[20];

            if (v == 0)
            {
                PutChar('0');
                return;
            }

            for (i = 0; v > 0; i++, v >>= 6) zBuf[i] = ZDigits[v & 0x3f];
            for (j = i - 1; j >= 0; j--) _a.Add((byte) zBuf[j]);
        }

        public byte[] ToArray()
        {
            return _a.ToArray();
        }
    }
}