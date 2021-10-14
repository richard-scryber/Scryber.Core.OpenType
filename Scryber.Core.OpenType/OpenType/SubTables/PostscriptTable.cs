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
    public class PostscriptTable : TrueTypeFontTable
    {

        public PostscriptTable(long offset)
            : base(offset)
        {

        }

        private float _italangle;

        public float ItalicAngle
        {
            get { return _italangle; }
            set { _italangle = value; }
        }

        private short _underlinepos;

        public short UnderlinePosition
        {
            get { return _underlinepos; }
            set { _underlinepos = value; }
        }

        private short _uthick;

        public short UnderlineThickness
        {
            get { return _uthick; }
            set { _uthick = value; }
        }

        private uint _fixedpitch;

        public uint FixedPitch
        {
            get { return _fixedpitch; }
            set { _fixedpitch = value; }
        }

        public bool IsMonoSpaced
        {
            get { return FixedPitch != 0; }
        }

        private uint _minmemTT;

        public uint MinMemoryOpenType
        {
            get { return _minmemTT; }
            set { _minmemTT = value; }
        }

        private uint _maxmemTT;

        public uint MaxMemoryOpenType
        {
            get { return _maxmemTT; }
            set { _maxmemTT = value; }
        }

        private uint _minmemT1;

        public uint MinMemoryType1
        {
            get { return _minmemT1; }
            set { _minmemT1 = value; }
        }

        private uint _maxmemT1;

        public uint MaxMemoryType1
        {
            get { return _maxmemT1; }
            set { _maxmemT1 = value; }
        }


        private List<GlyphName> _names;

        public List<GlyphName> Names
        {
            get { return _names; }
            set { _names = value; }
        }

    }

    public class GlyphName
    {
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return "[empty] {" + this.Index.ToString() + "}";
            }
            else
                return this.Name + " {" + this.Index.ToString() + "}";
        }

    }
}
