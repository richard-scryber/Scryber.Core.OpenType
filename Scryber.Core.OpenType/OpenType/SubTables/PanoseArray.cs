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

namespace Scryber.OpenType.SubTables
{
    public class PanoseArray
    {
        private byte[] _data;

        public PanoseArray()
            : this(new byte[10])
        {
        }

        public PanoseArray(byte[] data)
        {
            if (data == null || data.Length != 10)
                throw new ArgumentException("The Panose data should be a byte array of length 10");

            this._data = data;
        }

        public byte FamilyType { get { return this._data[0]; } set { this._data[0] = value; } }

        public byte SerifStyle { get { return this._data[1]; } set { this._data[1] = value; } }

        public byte Weight { get { return this._data[2]; } set { this._data[2] = value; } }

        public byte Proportion { get { return this._data[3]; } set { this._data[3] = value; } }

        public byte Contrast { get { return this._data[4]; } set { this._data[4] = value; } }

        public byte StrokeVariation { get { return this._data[5]; } set { this._data[5] = value; } }

        public byte ArmStyle { get { return this._data[6]; } set { this._data[6] = value; } }

        public byte Letterform { get { return this._data[7]; } set { this._data[7] = value; } }

        public byte Midline { get { return this._data[8]; } set { this._data[8] = value; } }

        public byte XHeight { get { return this._data[9]; } set { this._data[9] = value; } }
    }
}
