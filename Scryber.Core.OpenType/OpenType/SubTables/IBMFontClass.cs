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
    public class IBMFontClass : IEquatable<IBMFontClass>, IComparable<IBMFontClass>, IComparable
    {
        private byte _classid;

        public byte ClassID
        {
            get { return _classid; }
        }

        private byte _subclass;

        public byte SubClassID
        {
            get { return _subclass; }
        }

        public IBMFontClass(byte classid, byte subclassid)
        {
            this._classid = classid;
            this._subclass = subclassid;
        }

        public override int GetHashCode()
        {
            return ((int)ClassID * 256) + (int)SubClassID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IBMFontClass);
        }

        public override string ToString()
        {
            string c;
            if (_classnames.TryGetValue(this.ClassID, out c) == false)
                return "Reserved";
            string s;
            if (_subclassnames.TryGetValue(this, out s) == false)
                return "Reserved";

            return c + " : " + s;
        }

        private static Dictionary<byte, string> _classnames;
        private static Dictionary<IBMFontClass, string> _subclassnames;
        static IBMFontClass()
        {
            _classnames = new Dictionary<byte, string>();
            _classnames.Add(0, "No Classification");
            _classnames.Add(1, "Oldstyle Serifs");
            _classnames.Add(2, "Transitional Serifs");
            _classnames.Add(3, "Modern Serifs");
            _classnames.Add(4, "Clarendon Serifs");
            _classnames.Add(5, "Slab Serifs");
            _classnames.Add(7, "Freeform Serifs");
            _classnames.Add(8, "Sans Serif");
            _classnames.Add(9, "Ornamentals");
            _classnames.Add(10, "Scripts");
            _classnames.Add(12, "Symbolic");

            _subclassnames = new Dictionary<IBMFontClass, string>();
            _subclassnames.Add(new IBMFontClass(1, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(1, 1), "IBM Rounded Legibility");
            _subclassnames.Add(new IBMFontClass(1, 2), "Garalde");
            _subclassnames.Add(new IBMFontClass(1, 3), "Venetian");
            _subclassnames.Add(new IBMFontClass(1, 4), "Modified Venetian");
            _subclassnames.Add(new IBMFontClass(1, 5), "Dutch Modern");
            _subclassnames.Add(new IBMFontClass(1, 6), "Dutch Traditional");
            _subclassnames.Add(new IBMFontClass(1, 7), "Contemporary");
            _subclassnames.Add(new IBMFontClass(1, 8), "Caligraphic");
            _subclassnames.Add(new IBMFontClass(1, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(2, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(2, 1), "Direct Line");
            _subclassnames.Add(new IBMFontClass(2, 2), "Script");
            _subclassnames.Add(new IBMFontClass(2, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(3, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(3, 1), "Italian");
            _subclassnames.Add(new IBMFontClass(3, 2), "Script");
            _subclassnames.Add(new IBMFontClass(3, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(4, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(4, 1), "Clarendon");
            _subclassnames.Add(new IBMFontClass(4, 2), "Modern");
            _subclassnames.Add(new IBMFontClass(4, 3), "Traditional");
            _subclassnames.Add(new IBMFontClass(4, 4), "Newspaper");
            _subclassnames.Add(new IBMFontClass(4, 5), "Stub Serif");
            _subclassnames.Add(new IBMFontClass(4, 6), "Monotone");
            _subclassnames.Add(new IBMFontClass(4, 7), "Typewriter");
            _subclassnames.Add(new IBMFontClass(4, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(5, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(5, 1), "Monotone");
            _subclassnames.Add(new IBMFontClass(5, 2), "Humanist");
            _subclassnames.Add(new IBMFontClass(5, 3), "Geometric");
            _subclassnames.Add(new IBMFontClass(5, 4), "Swiss");
            _subclassnames.Add(new IBMFontClass(5, 5), "Typewriter");
            _subclassnames.Add(new IBMFontClass(5, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(7, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(7, 1), "Modern");
            _subclassnames.Add(new IBMFontClass(7, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(8, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(8, 1), "IBM Neo-grotesque Gothic");
            _subclassnames.Add(new IBMFontClass(8, 2), "Humanist");
            _subclassnames.Add(new IBMFontClass(8, 3), "Low-x Round Geometric");
            _subclassnames.Add(new IBMFontClass(8, 4), "High-x Round Geometric");
            _subclassnames.Add(new IBMFontClass(8, 5), "Neo-grotesque Gothic");
            _subclassnames.Add(new IBMFontClass(8, 6), "Modified Neo-grotesque Gothic");
            _subclassnames.Add(new IBMFontClass(8, 9), "Typewriter Gothic");
            _subclassnames.Add(new IBMFontClass(8, 10), "Matrix");
            _subclassnames.Add(new IBMFontClass(8, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(9, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(9, 1), "Engraver");
            _subclassnames.Add(new IBMFontClass(9, 2), "Black Letter");
            _subclassnames.Add(new IBMFontClass(9, 3), "Decorative");
            _subclassnames.Add(new IBMFontClass(9, 4), "Three Dimensional");
            _subclassnames.Add(new IBMFontClass(9, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(10, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(10, 1), "Uncial");
            _subclassnames.Add(new IBMFontClass(10, 2), "Brush Joined");
            _subclassnames.Add(new IBMFontClass(10, 3), "Formal Joined");
            _subclassnames.Add(new IBMFontClass(10, 4), "Monotone Joined");
            _subclassnames.Add(new IBMFontClass(10, 5), "Calligraphic");
            _subclassnames.Add(new IBMFontClass(10, 6), "Brush Unjoined");
            _subclassnames.Add(new IBMFontClass(10, 9), "Formal Unjoined");
            _subclassnames.Add(new IBMFontClass(10, 10), "Monotone Unjoined");
            _subclassnames.Add(new IBMFontClass(10, 15), "Miscellaneous");

            _subclassnames.Add(new IBMFontClass(12, 0), "No Classification");
            _subclassnames.Add(new IBMFontClass(12, 3), "Mixed Serif");
            _subclassnames.Add(new IBMFontClass(12, 6), "Oldstyle Serif");
            _subclassnames.Add(new IBMFontClass(12, 7), "Neo-grotesque Sans serif");
            _subclassnames.Add(new IBMFontClass(12, 15), "Miscellaneous");
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return this.CompareTo(obj as IBMFontClass);
        }

        #endregion

        #region IComparable<IBMFontClass> Members

        public int CompareTo(IBMFontClass other)
        {
            if (other == null)
                throw new ArgumentNullException("Cannot perform a comparison when one or more of the arguments are null");

            return this.GetHashCode().CompareTo(other.GetHashCode());

        }

        #endregion

        #region IEquatable<IBMFontClass> Members

        public bool Equals(IBMFontClass other)
        {
            if (other == null)
                throw new ArgumentNullException("Cannot perform an equality comparison when one or more of the arguments are null");

            return this.GetHashCode() == other.GetHashCode();
        }

        #endregion
    }

    
}
