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
    public class CodePageRange : UIntBitRange
    {
        public CodePageRange(uint zero, uint one)
            : this(new uint[] { zero, one })
        {
        }

        public CodePageRange(uint[] data)
            : base(data, 2)
        { }

        public bool IsBitSet(CodePageBit bit)
        {
            return this.IsBitSet((int)bit);
        }

        public void SetBit(CodePageBit bit)
        {
            this.SetBit((int)bit);
        }

        public void ClearBit(CodePageBit bit)
        {
            this.ClearBit((int)bit);
        }

        public override string ToString()
        {
            return base.BuildString(typeof(CodePageBit), ", ");
        }

        public string Value
        {
            get { return this.ToString(); }
        }

        public static bool TryGetCodePage(CodePageBit bit, out int codepage)
        {
            codepage = GetCodePage(bit);
            return codepage > -1;
        }

        public static int GetCodePage(CodePageBit bit)
        {
            int page;
            switch (bit)
            {
                case CodePageBit.Latin1:
                    page = 1252;
                    break;
                case CodePageBit.Latin2EasternEurope:
                    page = 1250;
                    break;
                case CodePageBit.Cyrillic:
                    page = 1251;
                    break;
                case CodePageBit.Greek:
                    page = 1253;
                    break;
                case CodePageBit.Turkish:
                    page = 1254;
                    break;
                case CodePageBit.Hebrew:
                    page = 1255;
                    break;
                case CodePageBit.Arabic:
                    page = 1256;
                    break;
                case CodePageBit.WindowsBaltic:
                    page = 1257;
                    break;
                case CodePageBit.Vietnamese:
                    page = 1258;
                    break;
                case CodePageBit.Thai:
                    page = 874;
                    break;
                case CodePageBit.Japan:
                    page = 932;
                    break;
                case CodePageBit.ChineseSimplified:
                    page = 936;
                    break;
                case CodePageBit.KoreanWansung:
                    page = 949;
                    break;
                case CodePageBit.ChineseTraditional:
                    page = 950;
                    break;
                case CodePageBit.KoreanJohab:
                    page = 1361;
                    break;
                case CodePageBit.IBMGreek:
                    page = 869;
                    break;
                case CodePageBit.MSDOSRussian:
                    page = 866;
                    break;
                case CodePageBit.MSDOSNordic:
                    page = 865;
                    break;
                case CodePageBit.Arabic2:
                    page = 864;
                    break;
                case CodePageBit.MSDOSCanadianFrench:
                    page = 863;
                    break;
                case CodePageBit.Hebrew2:
                    page = 862;
                    break;
                case CodePageBit.MSDOSIcelandic:
                    page = 861;
                    break;
                case CodePageBit.MSDOSPortuguese:
                    page = 860;
                    break;
                case CodePageBit.IBMTurkish:
                    page = 857;
                    break;
                case CodePageBit.IBMCyrillic:
                    page = 855;
                    break;
                case CodePageBit.Latin2:
                    page = 852;
                    break;
                case CodePageBit.MSDOSBaltic:
                    page = 775;
                    break;
                case CodePageBit.GreekFormer437G:
                    page = 737;
                    break;
                case CodePageBit.ArabicFormerASMO708:
                    page = 708;
                    break;
                case CodePageBit.WELatin1:
                    page = 850;
                    break;
                case CodePageBit.US:
                    page = 437;
                    break;
                default:
                    page = -1;
                    break;
            }
            return page;
        }
    }
}
