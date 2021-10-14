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
    public class CMAP_0_SubTable : CMAPSubTable
    {
        private ushort _len;

        public ushort Length
        {
            get { return _len; }
            set { _len = value; }
        }

        private ushort _lang;

        public ushort Language
        {
            get { return _lang; }
            set { _lang = value; }
        }

        private byte[] _glyphoffsets;

        public byte[] GlyphOffsets
        {
            get { return _glyphoffsets; }
            set { _glyphoffsets = value; }
        }

        public CMAP_0_SubTable(ushort format) : base(format)
        {
            if (format != 0)
                throw new ArgumentOutOfRangeException("format", "The format for the Apple Standard character set can only be 0");
        }

        public override int GetCharacterGlyphOffset(char c)
        {
            byte b = (byte)c;
            return _glyphoffsets[b];
        }
    }
}
