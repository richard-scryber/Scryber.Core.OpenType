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
    public class CMAP_4_SubTable : CMAPSubTable
    {

        public int segCount;
        public int entrySelector;
        public int rangeShift;
        public ushort[] endCount;
        public ushort[] startCount;
        public short[] idDelta;
        public ushort[] idRangeOffset;
        public ushort[] glyphids;
        


        public CMAP_4_SubTable(ushort format)
            : base(format)
        {
            if (format != 4)
                throw new ArgumentOutOfRangeException("format", "The format paramter must be 4 for a segment mapping to data value sub table");
        }

        public override int GetCharacterGlyphOffset(char c)
        {
           
            ushort charcode = (ushort)c;

            //from CMap.java
            int index = 0;
            ushort glyphcode = 0;
            try
            {
                int controlGlyph = getControlCodeGlyph(charcode, true);
                if (controlGlyph >= 0)
                    return controlGlyph;

                if (this.startCount[rangeShift] <= charcode)
                    index = rangeShift;

                int val = entrySelector;
                while(val-- > 0)
                {
                    if (startCount[index + (1 << val)] <= charcode)
                    {
                        index += (1 << val);
                    }
                }

                if (charcode >= startCount[index] && charcode <= endCount[index])
                {
                    int rangeOffset = idRangeOffset[index];

                    if (rangeOffset == 0)
                    {
                        glyphcode = (ushort)(charcode + idDelta[index]);
                    }
                    else
                    {
                        int glyphIdIndex = rangeOffset - segCount + index + (charcode - startCount[index]);

                        glyphcode = glyphids[glyphIdIndex];

                        if (glyphcode != 0)
                        {
                            glyphcode = (ushort)(glyphcode + idDelta[index]);
                        }
                    }
                }

            }
            catch (Exception)
            {
                glyphcode = 0;
            }
            
            return glyphcode;
            
        }

        private int getControlCodeGlyph(ushort charcode, bool noSurrogates)
        {
            if (charcode < 0x0010)
            {
                switch (charcode)
                {
                    case (0x0009):
                    case (0x000a):
                    case (0x000d):
                        return INVISIBLE_GLYPH_ID;
                    default:
                        break;
                }
            }
            else if (charcode >= 0x200c)
            {
                if ((charcode <= 0x200f) || (charcode >= 0x2028 && charcode <= 0x202e) || (charcode >= 0x206a && charcode <= 0x206f))
                    return INVISIBLE_GLYPH_ID;
                else if (noSurrogates && charcode >= 0xffff)
                    return 0;
                
            }
            return -1;
        }

        private const ushort INVISIBLE_GLYPH_ID = 0xffff;
    }
}
