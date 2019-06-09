/*
   Copyright 2016 Endel Dreyer (C# port)
   Copyright 2014 Dmitry Chestnykh (JavaScript port)
   Copyright 2007 D. Richard Hipp  (original C version)
   All rights reserved.
   
   Redistribution and use in source and binary forms, with or
   without modification, are permitted provided that the
   following conditions are met:
   
   1. Redistributions of source code must retain the above
   copyright notice, this list of conditions and the
   following disclaimer.
   
   2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the
   following disclaimer in the documentation and/or other
   materials provided with the distribution.
   
   THIS SOFTWARE IS PROVIDED BY THE AUTHORS ``AS IS'' AND ANY EXPRESS
   OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
   WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
   ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OR CONTRIBUTORS BE
   LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
   CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
   SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
   BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
   WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
   OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
   EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
   
   The views and conclusions contained in the software and documentation
   are those of the authors and contributors and should not be interpreted
   as representing official policies, either expressed or implied, of anybody
   else.
*/

using System;

namespace nUpdate.Delta
{
    internal class Reader
    {
        private static readonly int[] ZValue = {
            -1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1,
            0,  1,  2,  3,  4,  5,  6,  7,    8,  9, -1, -1, -1, -1, -1, -1,
            -1, 10, 11, 12, 13, 14, 15, 16,   17, 18, 19, 20, 21, 22, 23, 24,
            25, 26, 27, 28, 29, 30, 31, 32,   33, 34, 35, -1, -1, -1, -1, 36,
            -1, 37, 38, 39, 40, 41, 42, 43,   44, 45, 46, 47, 48, 49, 50, 51,
            52, 53, 54, 55, 56, 57, 58, 59,   60, 61, 62, -1, -1, -1, 63, -1
        };

        public readonly byte[] A;
        public uint Pos;

        public Reader(byte[] array)
        {
            A = array;
            Pos = 0;
        }

        public bool HaveBytes()
        {
            return Pos < A.Length;
        }

        public byte GetByte()
        {
            byte b = A[Pos];
            Pos++;
            if (Pos > A.Length)
                throw new IndexOutOfRangeException("out of bounds");
            return b;
        }

        public char GetChar()
        {
            //  return String.fromCharCode(this.getByte());
            return (char)GetByte();
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
            while (HaveBytes() && (c = ZValue[0x7f & GetByte()]) >= 0)
            {
                v = (uint)(((int)v << 6) + c);
            }
            Pos--;
            return v;
        }
    }
}
