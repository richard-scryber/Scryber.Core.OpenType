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
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.SubTables
{
    public class FontHeader : TrueTypeFontTable
    {

        public FontHeader(long offset)
            : base(offset)
        {
        }

        

        private Version _forntrev;

        public Version FontRevision
        {
            get { return _forntrev; }
            set { _forntrev = value; }
        }

        private uint _check;

        public uint ChecksumAdjustment
        {
            get { return _check; }
            set { _check = value; }
        }

        private uint _majic;

        public uint MagicNumber
        {
            get { return _majic; }
            set { _majic = value; }
        }

        private FontHeaderFlags _fflags;

        public FontHeaderFlags FontFlags
        {
            get { return _fflags; }
            set { _fflags = value; }
        }

        private ushort unitsperem;

        public ushort UnitsPerEm
        {
            get { return unitsperem; }
            set { unitsperem = value; }
        }

        private DateTime _created;

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        private DateTime _mod;

        public DateTime Modified
        {
            get { return _mod; }
            set { _mod = value; }
        }

        private short _xmin;

        public short XMin
        {
            get { return _xmin; }
            set { _xmin = value; }
        }

        private short _ymin;

        public short YMin
        {
            get { return _ymin; }
            set { _ymin = value; }
        }

        private short _xmax;

        public short XMax
        {
            get { return _xmax; }
            set { _xmax = value; }
        }

        private short _ymax;

        public short YMax
        {
            get { return _ymax; }
            set { _ymax = value; }
        }

        private FontStyleFlags _macstyle;

        public FontStyleFlags MacStyle
        {
            get { return this._macstyle; }
            set { this._macstyle = value; }
        }

        private ushort _smallestscreen;

        public ushort SmallestScreenFont
        {
            get { return _smallestscreen; }
            set { _smallestscreen = value; }
        }

        private FontDirectionFlags _directionhints;

        public FontDirectionFlags DirectionHints
        {
            get { return _directionhints; }
            set { _directionhints = value; }
        }

        private FontIndexLocationFormat _idx;

        public FontIndexLocationFormat IndexToLocationFormat
        {
            get { return _idx; }
            set { _idx = value; }
        }

        private GlyphDataFormat _gd;

        public GlyphDataFormat GlyphDataFormat
        {
            get { return _gd; }
            set { _gd = value; }
        }


 
    }
}
