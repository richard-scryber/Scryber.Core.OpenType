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
    public class NamingTable : TrueTypeFontTable
    {

        public NamingTable(long offset)
            : base(offset)
        {
        }

        private ushort _format;

        public ushort Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private ushort _count;

        public ushort Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private ushort _stringOffset;

        public ushort StringOffset
        {
            get { return _stringOffset; }
            set { _stringOffset = value; }
        }

        private NameEntryList _names = new NameEntryList();

        public NameEntryList Names
        {
            get { return _names; }
            set { _names = value; }
        }




        public string GetInvariantName(int NameID)
        {
            NameEntry entry = this.Names[NameID];
            if (null == entry)
                return string.Empty;

            string value = entry.InvariantName;
            if (string.IsNullOrEmpty(value))
                value = entry.LocalName;

            if (string.IsNullOrEmpty(value) && entry.NameItems.Count > 0)
                value = entry.NameItems[0].Value;

            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            return value;
        }
    }

    public class NameEntryList : System.Collections.ObjectModel.KeyedCollection<int, NameEntry>
    {
        protected override int GetKeyForItem(NameEntry item)
        {
            return item.NameID;
        }

        public bool TryGetEntry(int nameid, out NameEntry entry)
        {
            if (this.Count == 0)
            {
                entry = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(nameid, out entry);
        }
    }

    public class NameEntry
    {
        private int _nameid;

        public int NameID
        {
            get { return _nameid; }
            set { _nameid = value; }
        }

        private string _unicodeenty;

        public string InvariantName
        {
            get { return _unicodeenty; }
            set { _unicodeenty = value; }
        }

        private string _local;

        public string LocalName
        {
            get { return _local; }
            set { _local = value; }
        }


        private List<NameRecord> _items = new List<NameRecord>();

        public List<NameRecord> NameItems
        {
            get { return _items; }
            set { _items = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.LocalName) == false)
                return LocalName;
            else if (string.IsNullOrEmpty(this.InvariantName) == false)
                return InvariantName;
            else
            {
                string s = string.Empty;
                foreach (NameRecord rec in this.NameItems)
                {
                    if (string.IsNullOrEmpty(rec.Value) == false)
                    {
                        s = rec.Value;
                        break;
                    }
                }
                return s;
            }
        }

    }

    

}
