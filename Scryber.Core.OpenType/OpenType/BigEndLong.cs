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
    [StructLayout(LayoutKind.Explicit,Size=4,CharSet=CharSet.Ansi)]
    public struct BigEnd32
    {
        [FieldOffset(2)]
        private BigEnd16 _loword;
        public BigEnd16 LoWord
        {
            get { return _loword; }
        }

        [FieldOffset(0)]
        private BigEnd16 _hiword;
        public BigEnd16 HiWord
        {
            get { return _hiword; }
        }

        [FieldOffset(0)]
        private uint _val;
        public uint UnsignedValue
        {
            get { return _val; }
        }

        [FieldOffset(0)]
        private int _sval;

        public int SignedValue
        {
            get { return _sval; }
        }

        public BigEnd32(byte[] data)
            : this(data, 0)
        {
        }

        public BigEnd32(byte[] data, int offset)
        {
            this._sval = 0;
            this._val = 0;
            this._loword = new BigEnd16(data, offset);
            this._hiword = new BigEnd16(data, offset + 2);
        }
    }
}
