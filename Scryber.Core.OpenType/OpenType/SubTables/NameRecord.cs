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
    public class NameRecord
    {
        private ushort _platformid;

        public ushort PlatformID
        {
            get { return _platformid; }
            set { _platformid = value; }
        }

        private ushort _encid;

        public ushort EncodingID
        {
            get { return _encid; }
            set { _encid = value; }
        }

        private ushort _langid;

        public ushort LanguageID
        {
            get { return _langid; }
            set { _langid = value; }
        }

        private ushort _nameid;

        public ushort NameID
        {
            get { return _nameid; }
            set { _nameid = value; }
        }

        private ushort _len;

        public ushort StringLength
        {
            get { return _len; }
            set { _len = value; }
        }

        private ushort _soffset;

        public ushort StringDataOffset
        {
            get { return _soffset; }
            set { _soffset = value; }
        }

        private string _val;

        public string Value
        {
            get { return _val; }
            set { _val = value; }
        }

        public override string ToString()
        {
            return this.Value + " (platform : " + this.PlatformID.ToString() +
                ", encoding: " + this.EncodingID.ToString() + ", language: " +
                this.LanguageID.ToString() + ")";
        }

    }
}
