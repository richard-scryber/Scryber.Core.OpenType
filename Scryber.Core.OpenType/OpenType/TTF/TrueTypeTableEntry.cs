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

namespace Scryber.OpenType.TTF
{
    public class TrueTypeTableEntry
    {
        private string _tag;
        public string Tag
        {
            get { return _tag; }
            protected set { this._tag = value; }
        }

        private uint _checksum;
        public uint CheckSum
        {
            get { return _checksum; }
            protected set { _checksum = value; }
        }

        private uint _offset;
        public uint Offset
        {
            get { return _offset; }
            protected set { _offset = value; }
        }

        private uint _len;
        public uint Length
        {
            get { return _len; }
            protected set { _len = value; }
        }

        private TrueTypeFontTable _tbl;
        public TrueTypeFontTable Table
        {
            get { return _tbl; }
            protected set { _tbl = value; }
        }


        public virtual void Read(BigEndianReader reader)
        {
            this._tag = reader.ReadString(4);
            //this._tag = new string(tag);
            this._checksum = reader.ReadUInt32();
            this._offset = reader.ReadUInt32();
            this._len = reader.ReadUInt32();
        }

        

        public override string ToString()
        {
            return "Table Entry : " + this.Tag + " (from '" + this.Offset.ToString() + "' to '" + (this.Length + this.Offset).ToString() + "'";
        }

        internal void SetTable(TrueTypeFontTable tbl)
        {
            this._tbl = tbl;
        }
    }

    public class TrueTypeTableEntryList : System.Collections.ObjectModel.KeyedCollection<string, TrueTypeTableEntry>
    {
        public TrueTypeTableEntryList()
            : base()
        {
        }

        

        public TrueTypeTableEntryList(IEnumerable<TrueTypeTableEntry> items)
            : this()
        {
            if (items != null)
            {
                foreach (TrueTypeTableEntry item in items)
                {
                    this.Add(item);
                }
            }
        }

        protected override string GetKeyForItem(TrueTypeTableEntry item)
        {
            return item.Tag;
        }
    }
}
