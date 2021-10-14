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
    public static class TrueTypeTableNames
    {
        public const string HorizontalHeader = "hhea";
        public const string CharacterMapping = "cmap";
        public const string FontHeader = "head";
        public const string HorizontalMetrics = "hmtx";
        public const string MaximumProfile = "maxp";
        public const string NamingTable = "name";
        public const string WindowsMetrics = "OS/2";
        public const string PostscriptInformation = "post";
        
        //TrueType outlines
        public const string ControlValue = "cvt ";
        public const string FontProgram = "fpgm";
        public const string GlyphData = "glyf";
        public const string LocationIndex = "loca";
        public const string CVTProgram = "prep";
        
        //Postscript outlines
        public const string PostscriptProgram = "CFF ";
        public const string VerticalOrigin = "VORG";
        
        //Bitmap Glyphs
        public const string EmbeddedBitmapData = "EBDT";
        public const string EmbeddedBitmapLocationData = "EBLC";
        public const string EmbeddedBitmapScanningData = "EBSC";

        //Advanced Typogrpahic Tables
        public const string BaseLineData = "BASE";
        public const string GlyphDefinitionData = "GDEF";
        public const string GlyphPositionData = "GPOS";
        public const string GlyphSubstitutionData = "GSUB";
        public const string JustificationData = "JSTF";
        
        //Other open type tables
        public const string DigitalSignature = "DISG";
        public const string GridFittingAndScanConversion = "gasp";
        public const string HorizontalDeviceMetrics = "hdmx";
        public const string Kerning = "kern";
        public const string LinearThresholdData = "LTSH";
        public const string PCL5Data = "PCLT";
        public const string VerticalDeviceMetrics = "VMDX";
        public const string VerticalMetricsHeader = "vhea";
        public const string VerticalMetrics = "vmtx";

    }
}
