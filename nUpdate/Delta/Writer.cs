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

        public void PutArray(byte[] b, int start, int end)
        {
            for (var i = start; i < end; i++) _a.Add(b[i]);
        }

        public byte[] ToArray()
        {
            return _a.ToArray();
        }
    }
}