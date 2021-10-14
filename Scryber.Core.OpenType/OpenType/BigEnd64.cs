/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Scryber.OpenType
{
    [StructLayout(LayoutKind.Explicit, Size = 4, CharSet = CharSet.Ansi)]
    public struct BigEnd64
    {
        [FieldOffset(4)]
        private BigEnd32 _low;

        public BigEnd32 LoLong
        {
            get { return _low; }
        }

        [FieldOffset(0)]
        private BigEnd32 _hi;

        public BigEnd32 HiLong
        {
            get { return _hi; }
        }

        [FieldOffset(0)]
        private ulong _unsignedlong;

        public ulong UnsignedValue
        {
            get { return _unsignedlong; }
        }

        [FieldOffset(0)]
        private long _signedlong;

        public long SignedLong
        {
            get { return _signedlong; }
        }

        public BigEnd64(byte[] data)
            : this(data, 0)
        {
        }

        public BigEnd64(byte[] data, int offset)
        {
            this._unsignedlong = 0;
            this._signedlong = 0;
            this._low = new BigEnd32(data,offset);
            this._hi = new BigEnd32(data, offset + 4);
        }

    }
}
