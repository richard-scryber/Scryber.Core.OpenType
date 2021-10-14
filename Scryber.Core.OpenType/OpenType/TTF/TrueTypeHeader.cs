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

namespace Scryber.OpenType
{
    public class TrueTypeHeader : TypefaceHeader
    {

        private int _searchrange;

        public int SearchRange
        {
            get { return _searchrange; }
            set { _searchrange = value; }
        }

        private int _entrySel;

        public int EntrySelector
        {
            get { return _entrySel; }
            set { _entrySel = value; }
        }

        private int _rangeShift;

        public int RangeShift
        {
            get { return _rangeShift; }
            set { _rangeShift = value; }
        }

        public TrueTypeHeader(TrueTypeVersionReader version, int numTables)
            : base(version, numTables)
        {

        }

        internal static bool TryReadHeader(BigEndianReader reader, out TrueTypeHeader header)
        {
            header = null;

            TypefaceVersionReader vers;
            if (TypefaceVersionReader.TryGetVersion(reader, out vers) == false)
                return false;
            else if (vers is TrueTypeVersionReader ttv)
                return TryReadHeaderAfterVersion(reader, ttv, false, out header);
            else
                return false;

        }

        internal static bool TryReadHeaderAfterVersion(BigEndianReader reader, TrueTypeVersionReader version, bool validate, out TrueTypeHeader header)
        {
            header = null;

            ushort numtables = reader.ReadUInt16();
            ushort search = reader.ReadUInt16();
            ushort entry = reader.ReadUInt16();
            ushort range = reader.ReadUInt16();

            //Validate values returned.
            if (validate)
            {
                //searchRange is the (Maximum power of 2 <= numTables) * 16
                ushort max2 = 2;
                while (max2 * 2 <= numtables)
                    max2 *= 2;


                if (search != max2 * 16)
                    return false;

                //entrySelector is Log2(max2)
                if (Math.Log(max2, 2) != entry)
                    return false;

                //rangeShift = numTables * 16-searchRange
                if (range != ((numtables * 16) - search))
                    return false;

            }

            header = new TrueTypeHeader(version, numtables);
            header.SearchRange = search;
            header.EntrySelector = entry;
            header.RangeShift = range;

            return true;
        }
    }
}
