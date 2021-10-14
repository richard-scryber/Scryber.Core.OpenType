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
    public abstract class CMAPSubTable
    {
        private ushort _format;
        public ushort Format
        {
            get { return _format; }
        }

        public CMAPSubTable(ushort format)
        {
            this._format = format;
        }

        public int GetCharacterGlyphOffset(string s, int charIndex)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s", "The string for getting the character offset cannot be null or empty");
            if (charIndex < 0 || charIndex >= s.Length)
                throw new ArgumentOutOfRangeException("charIndex", "The charIndex parameter must e between 0 and the length of the string -1");

            return this.GetCharacterGlyphOffset(s[charIndex]);
        }

        public abstract int GetCharacterGlyphOffset(char c);
    }
}
