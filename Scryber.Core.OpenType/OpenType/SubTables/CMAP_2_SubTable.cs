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
    public class CMAP_2_SubTable : CMAPSubTable
    {

        public CMAP_2_SubTable(ushort format)
            : base(format)
        {
            if (format != 2)
                throw new ArgumentOutOfRangeException("format", "The format for the High-Byte mapping table can only be 2");

        }

        public override int GetCharacterGlyphOffset(char c)
        {
            throw new NotSupportedException("Searching of the format 2 (High-Byte mapping) table is not supported");
        }
    }
}
