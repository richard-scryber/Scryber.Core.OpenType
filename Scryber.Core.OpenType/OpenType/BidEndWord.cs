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
    [StructLayout(LayoutKind.Explicit, Size = 2, CharSet = CharSet.Ansi)]
    public struct BigEnd16
    {
        [FieldOffset(1)]
        private byte _lobyte;
        public byte LoByte
        {
            get { return _lobyte; }
            set { _lobyte = value; }
        }

        [FieldOffset(0)]
        private byte _hibyte;
        public byte HiByte
        {
            get { return _hibyte; }
            set { _hibyte = value; }
        }

        [FieldOffset(0)]
        private ushort _value;
        public ushort UnsignedValue
        {
            get { return _value; }
            set { _value = value; }
        }

        [FieldOffset(0)]
        private short _sval;

        public short SignedValue
        {
            get { return _sval; }
            set { _sval = value; }
        }

        
        public BigEnd16(byte lo, byte hi)
        {
            this._sval = 0;
            this._value = 0;
            this._lobyte = lo;
            this._hibyte = hi;
        }

        public BigEnd16(byte[] bigenddata)
            : this(bigenddata, 0)
        {
        }

        public BigEnd16(byte[] bigenddata, int offset)
        {
            this._sval = 0;
            this._value = 0;
            this._lobyte = bigenddata[offset++];
            this._hibyte = bigenddata[offset];
        }

    }
}
