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
    public class CMAPTable : TrueTypeFontTable
    {
        public CMAPTable(long offset)
            : base(offset)
        {
        }

        private ushort _numTables;

        public ushort NumberOfTables
        {
            get { return _numTables; }
            set { _numTables = value; }
        }

        private CMAPRecordList _rec;

        public CMAPRecordList Records
        {
            get 
            {
                if (_rec == null)
                    _rec = new CMAPRecordList();
                return _rec;
            }
            set { _rec = value; }
        }

        private CMAPRecord _last = null;

        public CMAPSubTable GetOffsetTable(CMapEncoding cmapenc)
        {
            if (null != _last && _last.Encoding == cmapenc)
                return _last.SubTable;

            foreach (CMAPRecord rec in this.Records)
            {
                if (rec.Encoding == cmapenc)
                {
                    _last = rec;
                    return _last.SubTable;
                }
            }
            return null;
        }

    }

    public class CMAPRecord
    {
        private CMapEncoding _cmapEnc;

        /// <summary>
        /// Gets or sets the Platform specific CMAP Encoding
        /// </summary>
        public CMapEncoding Encoding
        {
            get { return _cmapEnc; }
            internal set { _cmapEnc = value; }
        }
        

        private uint _offset;

        public uint MapOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private CMAPSubTable _subtable;

        public CMAPSubTable SubTable
        {
            get { return _subtable; }
            set { _subtable = value; }
        }


        public override string ToString()
        {
            return this.Encoding + " {offset:" + MapOffset + "}";
        }

    }

    public class CMAPRecordList : List<CMAPRecord>
    {

        public new CMAPRecord this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }
	
        public CMAPRecordList()
            : base()
        {
        }

        public CMAPRecordList(IEnumerable<CMAPRecord> records)
            : base(records)
        { }
    }
}
