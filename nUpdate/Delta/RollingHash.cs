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

namespace nUpdate.Delta
{
    internal class RollingHash
    {
        private ushort _a;
        private ushort _b;
        private ushort _i;
        private byte[] _z;

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
                a = (ushort)((a + x) & 0xffff);
                b = (ushort)((b + (Delta.Nhash - i) * x) & 0xffff);
                _z[i] = (byte)x;
            }
            _a = (ushort)(a & 0xffff);
            _b = (ushort)(b & 0xffff);
            _i = 0;
        }

        /**
		 * Advance the rolling hash by a single character "c"
		 */
        public void Next(byte c)
        {
            ushort old = _z[_i];
            _z[_i] = c;
            _i = (ushort)((_i + 1) & (Delta.Nhash - 1));
            _a = (ushort)(_a - old + c);
            _b = (ushort)(_b - Delta.Nhash * old + _a);
        }


        /**
		 * Return a 32-bit hash value
		 */
        public uint Value()
        {
            return (uint)(_a & 0xffff) | ((uint)(_b & 0xffff) << 16);
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
            return a | ((uint)b << 16);
        }

    }

}
