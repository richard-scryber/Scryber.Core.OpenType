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

namespace Scryber.OpenType.SubTables
{
    public struct HMetric
    {
        private char _c;

        public char Character
        {
            get { return _c; }
            set { _c = value; }
        }

        private ushort _advance;

        public ushort AdvanceWidth
        {
            get { return _advance; }
            set { _advance = value; }
        }

        private short _leftsizebearing;

        public short LeftSideBearing
        {
            get { return _leftsizebearing; }
            set { _leftsizebearing = value; }
        }

        public HMetric(ushort advancewidth, short leftbearing, char c)
        {
            this._c = c;
            this._advance = advancewidth;
            this._leftsizebearing = leftbearing;
        }

        public override string ToString()
        {
            return "HMetric '" + this.Character.ToString() + "' {aw: " + this.AdvanceWidth.ToString() + ", lsb: " + this.LeftSideBearing.ToString() + "}";
        }

    }
}
