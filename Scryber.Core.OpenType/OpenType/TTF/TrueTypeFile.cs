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
using System.Diagnostics;
using System.Text;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType.TTF
{
    public class TrueTypeFile : IOpenTypeFont
    {
        public DataFormat SourceFormat
        {
            get { return (null == this.Head) ? DataFormat.Other : this.Head.Version.DataFormat; }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private TypefaceHeader _head;
        public TypefaceHeader Head
        {
            get { return _head; }
        }

        private TTFTableSet _tables;

        public TTFTableSet Tables
        {
            get
            {
                if (null == _tables)
                    _tables = new TTFTableSet(this.Directories);
                return _tables;
            }
        }

        public int TableCount
        {
            get { return this.Directories.Count; }
        }

        public IEnumerable<string> TableKeys
        {
            get { return this.Directories.TableKeys; }
        }

        private byte[] _alldata;

        public byte[] FileData 
        { 
            get { return _alldata; }
            protected set { _alldata = value; }
        }

        private TrueTypeTableEntryList _dirs;
        public TrueTypeTableEntryList Directories
        {
            get { return _dirs; }
        }

        public bool IsValid
        {
            get { return this.Tables != null; }
        }

        private IFontInfo _ref;

        public IFontInfo Reference
        {
            get { return _ref; }
        }


        public string FamilyName { get { return _ref?.FamilyName; } }

        public WeightClass FontWeight { get { return _ref == null ? ((WeightClass)(0)) : _ref.FontWeight; } }

        public WidthClass FontWidth { get { return _ref == null ? ((WidthClass)(0)) : _ref.FontWidth; } }

        public FontRestrictions Restrictions { get { return _ref == null ? ((FontRestrictions)(0)) : _ref.Restrictions; } }

        public FontSelection Selections { get { return _ref == null ? ((FontSelection)(0)) : _ref.Selections; } }

        //
        // ctors
        //

        public TrueTypeFile(byte[] data, int headOffset)
            : base()
        {
            this._path = string.Empty;
            this.Read(data, headOffset);
        }

        public TrueTypeFile(TypefaceHeader header, TrueTypeTableEntryList entries)
        {
            this._head = header ?? throw new ArgumentNullException(nameof(header));
            this._dirs = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        //
        // methods
        //

        public IFontMetrics GetMetrics(TypeMeasureOptions options)
        {
            return GetMetrics(CMapEncoding.WindowsUnicode, options);
        }

        public virtual IFontMetrics GetMetrics(CMapEncoding? encoding, TypeMeasureOptions options)
        {
            if (!encoding.HasValue)
                encoding = CMapEncoding.WindowsUnicode;

            return TTFStringMeasurer.Create(this, encoding.Value, options);
        }

        public virtual byte[] GetFileData(DataFormat format)
        {
            if (format != this.Head.Version.DataFormat)
                throw new InvalidOperationException("Cannot convert the current data from " + this.Head.Version.DataFormat + " to " + format);
            return this.FileData;
        }

        public virtual bool CanGetFileData(DataFormat format)
        {
            return ((format == this.Head.Version.DataFormat) && (null != this.FileData));
        }

        public virtual void SetFileData(byte[] data, DataFormat format)
        {
            if(format != this.Head.Version.DataFormat)
                throw new TypefaceReadException("The data is not in the correct format");

            this.FileData = data;
        }

        public void EnsureReferenceMatched(IFontInfo reference)
        {
            this._ref = reference;

            if (reference.FamilyName != this.Tables.Names.GetInvariantName((int)NameTypes.FontFamily))
                throw new TypefaceReadException("The loaded font does not match the reference");
        }

        public bool HasCMap(CMapEncoding encoding)
        {
            CMAPSubTable tbl = this.Tables.CMap.GetOffsetTable(encoding);
            return null != tbl;
        }



        public bool TryGetTable(string name, out TrueTypeFontTable table)
        {
            if (null == this.Tables)
            {
                table = null;
                return false;
            }
            else
                return this.Tables.TryGetTable(name, out table);
        }

        bool IOpenTypeFont.TryGetTable(string name, out IOpenTypeFontTable table)
        {
            TrueTypeFontTable found;
            if(this.TryGetTable(name, out found))
            {
                table = found;
                return true;
            }
            else
            {
                table = null;
                return false;
            }

           
        }

        public bool TryGetTable<T>(string name, out T table) where T : IOpenTypeFontTable
        {
            TrueTypeFontTable found;
            bool success =  this.TryGetTable(name, out found);

            if (success && found is T match)
            {
                table = match;
                return success;
            }
            else
            {
                table = default;
                return false;
            }
        }

        //
        // reading methods
        //

        public void Read(byte[] data, int position)
        {
            System.IO.MemoryStream ms = null;
            BigEndianReader ber = null;
            try
            {
                ms = new System.IO.MemoryStream(data);
                if(position != 0)
                {
                    data = TTC.TTCollectionFile.ExtractTTFfromTTC(ms, position);
                    ms.Dispose();

                    ms = new System.IO.MemoryStream(data);
                }
                ber = new BigEndianReader(ms);
                

                this.Read(ber);
                this.FileData = data;
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Could not load the font file from the stream. " + ex.Message);
            }
            finally
            {
                if (null != ber)
                    ber.Dispose();
                if (null != ms)
                    ms.Dispose();
            }
        }
        

        protected void Read(BigEndianReader reader)
        {

            TrueTypeHeader header;
            if (TrueTypeHeader.TryReadHeader(reader, out header) == false)
                throw new NotSupportedException("The current stream is not a supported OpenType or TrueType font file");

            List<TrueTypeTableEntry> dirs;
            try
            {
                dirs = new List<TrueTypeTableEntry>();

                for (int i = 0; i < header.NumberOfTables; i++)
                {
                    TrueTypeTableEntry dir = new TrueTypeTableEntry();
                    dir.Read(reader);
                    dirs.Add(dir);
                }

                dirs.Sort(delegate(TrueTypeTableEntry one, TrueTypeTableEntry two) { return one.Offset.CompareTo(two.Offset); });
                this._dirs = new TrueTypeTableEntryList(dirs);
                this._head = header;

                TrueTypeTableFactory factory = this.GetFactory(header);
                foreach (TrueTypeTableEntry dir in dirs)
                {
                    TrueTypeFontTable tbl = factory.ReadTable(dir, this, reader);
                    if(tbl != null)
                        dir.SetTable(tbl);
                }
            }
            catch (Exception ex) { throw new TypefaceReadException("Could not read the TTF File", ex); }
        }

        protected virtual TrueTypeTableFactory GetFactory(TrueTypeHeader header)
        {
            var vers = header.Version as TrueTypeVersionReader;
            if (null == vers)
                throw new TypefaceReadException("Could not get the table reader from the header");

            return vers.GetTableFactory();
        }

        public const double NoWordSpace = 0.0;
        public const double NoCharacterSpace = 0.0;
        public const double NoHorizontalScale = 1.0;

        /// <summary>
        /// Measures the size of the provided string at the specified font size (starting at a specific offset), 
        /// stopping when the available space is full and returning the number of characters fitted.
        /// </summary>
        /// <param name="encoding">The encoding to use to map the characters</param>
        /// <param name="s">The string to measure the size of</param>
        /// <param name="startOffset">The starting (zero based) offset in that string to start measuring from</param>
        /// <param name="emsize">The M size in font units</param>
        /// <param name="availablePts">The max width allowed for this string</param>
        /// <param name="wordspace">The spacing between words in font units. Default 0</param>
        /// <param name="charspace">The spacing between characters in font units. Default 0</param>
        /// <param name="hscale">The horizontal scaling of all characters. Default 100</param>
        /// <param name="vertical">If true then this is vertical writing</param>
        /// <param name="wordboundary">If True the measuring will stop at a boundary to a word rather than character.</param>
        /// <param name="charsfitted">Set to the number of characters that can be renered at this size within the width.</param>
        /// <returns></returns>
        public LineSize MeasureString(CMapEncoding encoding, string s, int startOffset, double emsize, double availablePts, double? wordspacePts, double charspacePts, double hscale, bool vertical, bool wordboundary, out int charsfitted, FontUnitType useUnits = FontUnitType.UseFontPreference)
        {
            HorizontalMetrics table = this.Directories["hmtx"].Table as HorizontalMetrics;
            CMAPTable cmap = this.Directories["cmap"].Table as CMAPTable;
            OS2Table os2 = this.Directories["OS/2"].Table as OS2Table;
            
            CMAPSubTable mac = cmap.GetOffsetTable(encoding);
            if (mac == null)
                mac = cmap.GetOffsetTable(CMapEncoding.MacRoman);

            HorizontalHeader hhead = this.Directories["hhea"].Table as HorizontalHeader;
            FontHeader head = this.Directories["head"].Table as FontHeader;
            
            double availableFU = availablePts * ((double)head.UnitsPerEm / emsize);
            double charspaceFU = NoCharacterSpace;

            if (charspacePts != NoCharacterSpace)
                charspaceFU = charspacePts * ((double)head.UnitsPerEm / emsize);

            double wordspaceFU = NoWordSpace;

            if (wordspacePts.HasValue)
                wordspaceFU = (wordspacePts.Value * ((double)head.UnitsPerEm / emsize));
            else if (charspacePts != NoCharacterSpace)
                //If we dont have explicit wordspacing then we use the character spacing
                wordspaceFU = charspaceFU;


            double len = 0.0;
            double lastwordlen = 0.0;
            int lastwordcount = 0;
            charsfitted = 0;


            for (int i = startOffset; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c))
                {
                    lastwordlen = len;
                    lastwordcount = charsfitted;
                }

                int moffset = (int)mac.GetCharacterGlyphOffset(c);
                
                if (moffset >= table.HMetrics.Count)
                    moffset = table.HMetrics.Count - 1;

                Scryber.OpenType.SubTables.HMetric metric;
                metric = table.HMetrics[moffset];
                double w = metric.AdvanceWidth;

                if (i == 0)
                {
                    w -= metric.LeftSideBearing;
                }

                if (c == ' ')
                {
                    if (wordspaceFU != NoWordSpace)
                        w += wordspaceFU;
                }
                else if (charspaceFU != NoCharacterSpace)
                    w += charspaceFU;




                if (hscale != NoHorizontalScale)
                    w *= hscale;

                len += w;

                //check if we can fit more
                if (len > availableFU)
                {
                    len -= w;
                    break;
                }
                charsfitted++;
            }
            bool isboundary = false;

            if ((charsfitted + startOffset < s.Length) && wordboundary && lastwordlen > 0)
            {
                len = lastwordlen;
                charsfitted = lastwordcount;
                isboundary = true;
            }

            len = len * emsize;
            len = len / (double)head.UnitsPerEm;
            double h = GetLineHeight(useUnits, os2, hhead, head, emsize); 
            return new LineSize(len, h, charsfitted, startOffset, isboundary);
        }


        public LineSize MeasureString(CMapEncoding encoding, string s, int startOffset, double emsize, double available, bool wordboundary, out int charsfitted, FontUnitType useUnits = FontUnitType.UseFontPreference)
        {
            HorizontalMetrics table = this.Directories["hmtx"].Table as HorizontalMetrics;
            CMAPTable cmap = this.Directories["cmap"].Table as CMAPTable;
            OS2Table os2 = this.Directories["OS/2"].Table as OS2Table;

            
            CMAPSubTable mac = cmap.GetOffsetTable(encoding);
            if (mac == null)
                mac = cmap.GetOffsetTable(CMapEncoding.MacRoman);

            HorizontalHeader hhead = this.Directories["hhea"].Table as HorizontalHeader;
            FontHeader head = this.Directories["head"].Table as FontHeader;
            available = (available * head.UnitsPerEm) / emsize;

            double len = 0.0;
            double lastwordlen = 0.0;
            int lastwordcount = 0;
            charsfitted = 0;

            
            for (int i = startOffset; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c))
                {
                    lastwordlen = len;
                    lastwordcount = charsfitted;
                }

                int moffset = (int)mac.GetCharacterGlyphOffset(c);
                //System.Diagnostics.Debug.WriteLine("Character '" + chars[i].ToString() + "' (" + ((byte)chars[i]).ToString() + ") has offset '" + moffset.ToString() + "' in mac encoding and '" + woffset + "' in windows encoding");

                if (moffset >= table.HMetrics.Count)
                    moffset = table.HMetrics.Count - 1;
                Scryber.OpenType.SubTables.HMetric metric;
                metric = table.HMetrics[moffset];
                if (i == 0)
                    len = -metric.LeftSideBearing;
                len += metric.AdvanceWidth;
                
                //check if we can fit more
                if (len > available)
                {
                    len -= metric.AdvanceWidth;
                    break;
                }
                charsfitted++;
            }

            bool isboundary = false;
            if ((charsfitted + startOffset < s.Length) && wordboundary && lastwordlen > 0)
            {
                len = lastwordlen;
                charsfitted = lastwordcount;
                isboundary = true;
            }
            
            len = len / (double)head.UnitsPerEm;
            len = len * emsize;
            double h = GetLineHeight(useUnits, os2, hhead, head, emsize);
            return new LineSize((float)len, (float)h, charsfitted, startOffset, isboundary);
        }

        protected double GetLineHeight(FontUnitType units, OS2Table os2, HorizontalHeader hhead, FontHeader header, double emsize)
        {
            double h;
            bool useTypo;
            if (units == FontUnitType.UseFontPreference)
            {
                useTypo = (os2.Version >= OS2TableVersion.OpenType15) && ((os2.Selection & FontSelection.UseTypographicSizes) > 0);
            }
            else if (units == FontUnitType.UseHeadMetrics)
                useTypo = false;
            else if (null == os2)
                useTypo = false;
            else
                useTypo = true;

            if (useTypo)
                h = ((double)(os2.TypoAscender - os2.TypoDescender + os2.TypoLineGap) / ((double)header.UnitsPerEm) * emsize);
            else
                h = ((double)(hhead.Ascender - hhead.Descender + hhead.LineGap) / ((double)header.UnitsPerEm) * emsize);

            return h;
        }

        public override string ToString()
        {
            return "Typeface " + this.FamilyName + " (weight: " + this.FontWeight.ToString() + ", selection: " + this.Selections.ToString() + ", width: " + this.FontWidth.ToString() + ")";
        }


        public static bool CanRead(string path)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);

            return CanRead(fi);
        }


        public static bool CanRead(System.IO.FileInfo fi)
        {
            if (fi.Exists == false)
                return false;
            else
            {
                using (System.IO.FileStream fs = fi.OpenRead())
                {
                    return CanRead(fs);
                }
            }
        }

        public static bool CanRead(System.IO.Stream stream)
        {
            BigEndianReader reader = new BigEndianReader(stream);
            return CanRead(reader);
            
        }

        public static bool CanRead(BigEndianReader reader)
        {
            long oldpos = reader.Position;
            reader.Position = 0;
            TrueTypeHeader header;

            bool b = TrueTypeHeader.TryReadHeader(reader, out header);

            reader.Position = oldpos;

            return b;
        }
    }
}
