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
    public class HorizontalHeader : TrueTypeFontTable
    {

        public HorizontalHeader(long offset)
            : base(offset)
        { }

        private short _asc;

        public short Ascender
        {
            get { return _asc; }
            set { _asc = value; }
        }

        private short _desc;

        public short Descender
        {
            get { return _desc; }
            set { _desc = value; }
        }

        private short _line;

        public short LineGap
        {
            get { return _line; }
            set { _line = value; }
        }

        private ushort _advanceWidth;

        public ushort AdvanceWidthMax
        {
            get { return _advanceWidth; }
            set { _advanceWidth = value; }
        }

        private short _leftmin;

        public short MinimumLeftSideBearing
        {
            get { return _leftmin; }
            set { _leftmin = value; }
        }

        private short _rightmin;

        public short MinimumRightSideBearing
        {
            get { return _rightmin; }
            set { _rightmin = value; }
        }

        private short _xmax;

        public short XMaxExtent
        {
            get { return _xmax; }
            set { _xmax = value; }
        }

        private short _sloperise;

        public short CaretSlopeRise
        {
            get { return _sloperise; }
            set { _sloperise = value; }
        }

        private short _sloperun;

        public short CaretSlopeRun
        {
            get { return _sloperun; }
            set { _sloperun = value; }
        }

        private short _offset;

        public short CaretOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }


        private short _res1;

        public short Reserved1
        {
            get { return _res1; }
            set { _res1 = value; }
        }

        private short _res2;

        public short Reserved2
        {
            get { return _res2; }
            set { _res2 = value; }
        }

        private short _res3;

        public short Reserved3
        {
            get { return _res3; }
            set { _res3 = value; }
        }

        private short _res4;

        public short Reserved4
        {
            get { return _res4; }
            set { _res4 = value; }
        }


        private short _metricformat;

        public short MetricDataFormat
        {
            get { return _metricformat; }
            set { _metricformat = value; }
        }

        private ushort _numhmet;

        public ushort NumberOfHMetrics
        {
            get { return _numhmet; }
            set { _numhmet = value; }
        }



    }
}