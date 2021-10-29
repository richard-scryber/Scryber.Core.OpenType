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
using System.IO;
using System.Text;
using System.Threading;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType.TTF
{
    public abstract class TrueTypeTableFactory
    {
        private bool _throwOnNotFound;

        public bool ThrowOnNotFound
        {
            get { return _throwOnNotFound; }
        }

        private List<string> _required;

        protected List<string> RequiredTables
        {
            get { return this._required; }
        }

        public TrueTypeTableFactory(bool throwonnotfound, IEnumerable<string> required)
        {
            this._throwOnNotFound = throwonnotfound;
            this._required = new List<string>(required);
        }

        public TrueTypeFontTable ReadTable(string tname, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            TrueTypeTableEntry dir = list[tname];

            return ReadTable(dir, list, reader);
        }

        public TrueTypeFontTable ReadTable(TrueTypeTableEntry entry, TTFFile file, BigEndianReader reader)
        {
            return this.ReadTable(entry, file.Directories, reader);
        }


        public TrueTypeFontTable ReadTable(TrueTypeTableEntry directory, TTF.TrueTypeFile file, BigEndianReader reader)
        {
            return this.ReadTable(directory, file.Directories, reader);
        }

        public TrueTypeFontTable ReadTable(TrueTypeTableEntry directory, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            reader.Position = directory.Offset;
            return this.ReadTable(directory.Tag, directory.Length, list, reader);
        }

        

        public void ValidateReadTables()
        {
            if (this._required.Count > 0)
            {
                StringBuilder sb = new StringBuilder("The required tables '");
                for (int i = 0; i < this._required.Count; i++)
                {
                    if (i > 0)
                        sb.Append("', '");
                    sb.Append(this._required[i]);
                }
                sb.Append("' were not found in the font definition");

                throw new TypefaceReadException(sb.ToString());
            }
        }

        protected virtual TrueTypeFontTable ReadTable(string tag, uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {

            TrueTypeFontTable tbl;
            try
            {
                switch (tag)
                {
                    case ("cmap"):
                        tbl = this.ReadCMAPTable(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("head"):
                        tbl = this.ReadFontHeader(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("hhea"):
                        tbl = this.ReadHorizontalHeader(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("hmtx"):
                        tbl = this.ReadHorizontalMetrics(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("maxp"):
                        tbl = this.ReadMaxProfile(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("name"):
                        tbl = this.ReadNameTable(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("OS/2"):
                        tbl = this.ReadOS2Table(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("post"):
                        tbl = this.ReadPostscriptTable(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    case ("kern"):
                        tbl = this.ReadKerningTable(length, list, reader);
                        this.RequiredTables.Remove(tag);
                        break;

                    default:
                        if (this.ThrowOnNotFound)
                            throw new TypefaceReadException("The table '" + tag + "' is of an  unknown type.");
                        tbl = null;
                        break;
                }
            }
            catch (TypefaceReadException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TypefaceReadException("Could not read the table '" + tag + "'", ex);
            }
            return tbl;
        }

        #region hhea

        private TrueTypeFontTable ReadHorizontalHeader(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            HorizontalHeader hhead = new HorizontalHeader(reader.Position);
            hhead.TableVersion = reader.ReadFixedVersion();
            hhead.Ascender = reader.ReadInt16();
            hhead.Descender = reader.ReadInt16();
            hhead.LineGap = reader.ReadInt16();
            hhead.AdvanceWidthMax = reader.ReadUInt16();
            hhead.MinimumLeftSideBearing = reader.ReadInt16();
            hhead.MinimumRightSideBearing = reader.ReadInt16();
            hhead.XMaxExtent = reader.ReadInt16();
            hhead.CaretSlopeRise = reader.ReadInt16();
            hhead.CaretSlopeRun = reader.ReadInt16();
            hhead.CaretOffset = reader.ReadInt16();
            hhead.Reserved1 = reader.ReadInt16();
            hhead.Reserved2 = reader.ReadInt16();
            hhead.Reserved3 = reader.ReadInt16();
            hhead.Reserved4 = reader.ReadInt16();
            hhead.MetricDataFormat = reader.ReadInt16();
            hhead.NumberOfHMetrics = reader.ReadUInt16();

            return hhead;
        }

        #endregion

        #region head

        private TrueTypeFontTable ReadFontHeader(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            FontHeader fh = new FontHeader(reader.Position);
            fh.TableVersion = reader.ReadFixedVersion();
            fh.FontRevision = reader.ReadFixedVersion();
            fh.ChecksumAdjustment = reader.ReadUInt32();
            fh.MagicNumber = reader.ReadUInt32();
            fh.FontFlags = (FontHeaderFlags)reader.ReadUInt16();
            fh.UnitsPerEm = reader.ReadUInt16();
            fh.Created = reader.ReadDateTime();
            fh.Modified = reader.ReadDateTime();
            fh.XMin = reader.ReadInt16();
            fh.YMin = reader.ReadInt16();
            fh.XMax = reader.ReadInt16();
            fh.YMax = reader.ReadInt16();
            fh.MacStyle = (FontStyleFlags)reader.ReadUInt16();
            fh.SmallestScreenFont = reader.ReadUInt16();
            fh.DirectionHints = (FontDirectionFlags)reader.ReadInt16();
            fh.IndexToLocationFormat = (FontIndexLocationFormat)reader.ReadInt16();
            fh.GlyphDataFormat = (GlyphDataFormat)reader.ReadInt16();
            return fh;
        }

        #endregion

        #region cmap

        private TrueTypeFontTable ReadCMAPTable(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            SubTables.CMAPTable tbl = new Scryber.OpenType.SubTables.CMAPTable(reader.Position);
            Version vers = reader.ReadUShortVersion();
            ushort num = reader.ReadUInt16();
            tbl.NumberOfTables = num;
            tbl.TableVersion = vers;
            List<SubTables.CMAPRecord> recs = new List<Scryber.OpenType.SubTables.CMAPRecord>();

            for (int i = 0; i < num; i++)
            {
                ushort plat = reader.ReadUInt16();
                ushort enc = reader.ReadUInt16();
                uint offset = reader.ReadUInt32();
                CMAPRecord rec = new CMAPRecord();
                rec.Encoding = new CMapEncoding((CharacterPlatforms)plat, enc);
                rec.MapOffset = offset;

                recs.Add(rec);
            }

            foreach (CMAPRecord rec in recs)
            {
                reader.Position = tbl.FileOffset + rec.MapOffset;
                ushort format = reader.ReadUInt16();

                switch (format)
                {
                    case (0):
                        ReadCMAP_0_SubTable(format, rec, list, reader);

                        break;

                    case (2):
                        ReadCMAP_2_SubTable(format, rec, list, reader);
                        break;

                    case (4):
                        ReadCMAP_4_SubTable(format, rec, list, reader);
                        break;

                    case (6):
                        ReadCMAP_6_SubTable(format, rec, list, reader);
                        break;

                    case (8):
                        ReadCMAP_8_SubTable(format, rec, list, reader);
                        break;

                    case (10):
                        ReadCMAP_10_SubTable(format, rec, list, reader);
                        break;

                    case (12):
                        ReadCMAP_12_SubTable(format, rec, list, reader);
                        break;

                    default:
                        break;
                }
            }

            //TODO:Read the CMAP Records

            tbl.Records = new CMAPRecordList(recs);

            return tbl;
        }

        private void ReadCMAP_0_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            CMAP_0_SubTable zero = new CMAP_0_SubTable(format);
            zero.Length = reader.ReadUInt16();
            zero.Language = reader.ReadUInt16();

            byte[] offsets = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                offsets[i] = reader.ReadByte();
            }
            zero.GlyphOffsets = offsets;
            rec.SubTable = zero;
        }


        private void ReadCMAP_2_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {

        }

        private void ReadCMAP_4_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            //start of the subtable
            long pos = reader.Position - 2;

            CMAP_4_SubTable four = new CMAP_4_SubTable(format);
            int len = reader.ReadUInt16();

            reader.ReadUInt16();//skip language
            int segCount = reader.ReadUInt16() / 2;
            four.segCount = segCount;
            int searchRange = reader.ReadUInt16();
            four.entrySelector = reader.ReadUInt16();
            four.rangeShift = reader.ReadUInt16() / 2;
            four.startCount = new ushort[segCount];
            four.endCount = new ushort[segCount];
            four.idDelta = new short[segCount];
            four.idRangeOffset = new ushort[segCount];

            for (int i = 0; i < segCount; i++)
            {
                four.endCount[i] = reader.ReadUInt16();
            }

            reader.ReadUInt16();

            for (int i = 0; i < segCount; i++)
            {
                four.startCount[i] = reader.ReadUInt16();
            }

            for (int i = 0; i < segCount; i++)
            {
                four.idDelta[i] = reader.ReadInt16();
            }

            for (int i = 0; i < segCount; i++)
            {
                ushort val = reader.ReadUInt16();
                four.idRangeOffset[i] = (ushort)((val >> 1) & 0xffff);
            }

            List<ushort> glyphs = new List<ushort>();
            while (reader.Position < pos + len)
            {
                glyphs.Add(reader.ReadUInt16());
            }
            four.glyphids = glyphs.ToArray();
            rec.SubTable = four;
        }

        private void ReadCMAP_6_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {

        }

        private void ReadCMAP_8_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {

        }

        private void ReadCMAP_10_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {

        }

        private void ReadCMAP_12_SubTable(ushort format, CMAPRecord rec, TrueTypeTableEntryList list, BigEndianReader reader)
        {

        }

        #endregion

        #region hmtx

        private TrueTypeFontTable ReadHorizontalMetrics(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            var pos = reader.Position;
            //Get the number of entries for the hMetrics list from the horizontal header table.
            TrueTypeTableEntry hhead = list["hhea"];
            if (hhead == null)
                throw new ArgumentNullException("No horizontal header (hhea) has been specified in the font file, this is needed to load the hroizontal metrics");

            if (hhead.Table == null)
            {
                TrueTypeFontTable tbl = this.ReadTable(hhead, list, reader);
                if (tbl == null)
                    throw new ArgumentNullException("No horizontal header (hhea) has been specified in the font file, this is needed to load the hroizontal metrics");
                hhead.SetTable(tbl);
            }

            TrueTypeTableEntry os2 = list["OS/2"];

            if (os2 == null)
                throw new ArgumentNullException("No OS2 table has been specified in the font file, this is a required table and needed to load the horizontal metrics");

            if (os2.Table == null)
            {
                TrueTypeFontTable tbl = this.ReadTable(os2, list, reader);
                if (tbl == null)
                    throw new ArgumentNullException("No OS/2 has been specified in the font file, this is needed to load the hroizontal metrics");
                os2.SetTable(tbl);
            }

            if (reader.Position != pos)
                reader.Position = pos;

            int firstcharindex = (os2.Table as OS2Table).FirstCharIndex;
            HorizontalMetrics hm = new HorizontalMetrics(reader.Position);

            ushort count = (hhead.Table as SubTables.HorizontalHeader).NumberOfHMetrics;

            List<HMetric> metrics = new List<HMetric>();

            for (int i = 0; i < count; i++)
            {
                SubTables.HMetric metric = new HMetric(reader.ReadUInt16(), reader.ReadInt16(), (char)(i + firstcharindex));
                metrics.Add(metric);
            }

            hm.HMetrics = metrics;

            return hm;
        }

        #endregion

        #region maxp

        private static readonly Version MaxPv1 = new Version(1, 0);
        private static readonly Version MaxPv05 = new Version(0, 5);

        private TrueTypeFontTable ReadMaxProfile(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            long pos = reader.Position;
            Version vers = reader.ReadFixedVersion();

            if (vers == new Version(1, 0)) //0x00010000
            {
                MaxTTProfile ttp = new MaxTTProfile(pos);
                ttp.TableVersion = vers;
                ttp.NumberOfGlyphs = reader.ReadUInt16();
                ttp.MaxPoints = reader.ReadUInt16();
                ttp.MaxContours = reader.ReadUInt16();
                ttp.MaxCompositePoints = reader.ReadUInt16();
                ttp.MaxCompositeContours = reader.ReadUInt16();
                ttp.MaxZones = reader.ReadUInt16();
                ttp.MaxTwilightPoints = reader.ReadUInt16();
                ttp.MaxStorage = reader.ReadUInt16();
                ttp.MaxFunctionDefinitions = reader.ReadUInt16();
                ttp.MaxInstructionDefinitions = reader.ReadUInt16();
                ttp.MaxStackComponents = reader.ReadUInt16();
                ttp.MaxSizeOfInstructions = reader.ReadUInt16();
                ttp.MaxComponentComponents = reader.ReadUInt16();
                ttp.MaxComponentDepth = reader.ReadUInt16();

                return ttp;
            }
            else if (vers.Major == 0 && vers.Minor == 20480) //0x00005000
            {
                MaxProfile prof = new MaxProfile(pos);
                prof.TableVersion = MaxPv05;
                prof.NumberOfGlyphs = reader.ReadUInt16();

                return prof;
            }
            else
                throw new NotSupportedException("The MaxProfile version " + vers.ToString() + " is not supported");
        }

        #endregion

        #region name

        private TrueTypeFontTable ReadNameTable(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            NamingTable nt = new NamingTable(reader.Position);
            nt.Format = reader.ReadUInt16();
            nt.Count = reader.ReadUInt16();
            nt.StringOffset = reader.ReadUInt16();

            nt.Names = new NameEntryList();
            List<NameRecord> records = new List<NameRecord>();
            NameEntry entry;
            if (nt.Count > 0)
            {
                for (int i = 0; i < nt.Count; i++)
                {
                    NameRecord rec = new NameRecord();
                    rec.PlatformID = reader.ReadUInt16();
                    rec.EncodingID = reader.ReadUInt16();
                    rec.LanguageID = reader.ReadUInt16();
                    rec.NameID = reader.ReadUInt16();
                    rec.StringLength = reader.ReadUInt16();
                    rec.StringDataOffset = reader.ReadUInt16();

                    records.Add(rec);
                }

                long startStore = nt.FileOffset + nt.StringOffset;

                int currlang = System.Globalization.CultureInfo.CurrentCulture.LCID;
                int parentlang = System.Globalization.CultureInfo.CurrentCulture.Parent != null ?
                    System.Globalization.CultureInfo.CurrentCulture.Parent.LCID : 0;

                foreach (NameRecord rec in records)
                {
                    reader.Position = startStore + (long)rec.StringDataOffset;

                    if (rec.PlatformID == 0 || rec.PlatformID == 3)
                        rec.Value = reader.ReadUnicodeString(rec.StringLength);
                    else
                        rec.Value = reader.ReadString(rec.StringLength);

                    if (nt.Names.TryGetEntry(rec.NameID, out entry) == false)
                    {
                        entry = new NameEntry();
                        entry.NameID = rec.NameID;
                        nt.Names.Add(entry);
                    }
                    entry.NameItems.Add(rec);

                    if (rec.LanguageID == 0)
                        entry.InvariantName = rec.Value;
                    else if (rec.LanguageID == currlang)
                        entry.LocalName = rec.Value;
                    else if (rec.LanguageID == parentlang && string.IsNullOrEmpty(entry.LocalName))
                        entry.LocalName = rec.Value;

                }
            }
            return nt;
        }

        #endregion

        #region OS/2

        private TrueTypeFontTable ReadOS2Table(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            OS2Table os2 = new OS2Table(reader.Position);
            os2.Version = (OS2TableVersion)reader.ReadUInt16();
            os2.XAverageCharWidth = reader.ReadInt16();
            os2.WeightClass = (WeightClass)reader.ReadUInt16();
            os2.WidthClass = (WidthClass)reader.ReadUInt16();
            os2.FSType = (FontRestrictions)reader.ReadUInt16();

            os2.SubscriptXSize = reader.ReadInt16();
            os2.SubscriptYSize = reader.ReadInt16();
            os2.SubscriptXOffset = reader.ReadInt16();
            os2.SubscriptYOffset = reader.ReadInt16();

            os2.SuperScriptXSize = reader.ReadInt16();
            os2.SuperScriptYSize = reader.ReadInt16();
            os2.SuperscriptXOffset = reader.ReadInt16();
            os2.SuperscriptYOffset = reader.ReadInt16();

            os2.StrikeoutSize = reader.ReadInt16();
            os2.StrikeoutPosition = reader.ReadInt16();

            byte hi = reader.ReadByte();
            byte lo = reader.ReadByte();
            os2.FamilyClass = new IBMFontClass(hi, lo);

            byte[] data = reader.Read(10);
            os2.Panose = new PanoseArray(data);

            uint zero = reader.ReadUInt32();
            uint one = reader.ReadUInt32();
            uint two = reader.ReadUInt32();
            uint three = reader.ReadUInt32();
            os2.UnicodeRanges = new UnicodeRanges(zero, one, two, three);

            os2.VendorID = reader.ReadString(4);
            os2.Selection = (FontSelection)reader.ReadUInt16();

            os2.FirstCharIndex = reader.ReadUInt16();
            os2.LastCharIndex = reader.ReadUInt16();
            os2.TypoAscender = reader.ReadInt16();
            os2.TypoDescender = reader.ReadInt16();
            os2.TypoLineGap = reader.ReadInt16();
            os2.WinAscent = reader.ReadUInt16();
            os2.WinDescent = reader.ReadUInt16();

            if (os2.Version >= OS2TableVersion.TrueType166)
            {
                zero = reader.ReadUInt32();
                one = reader.ReadUInt32();
                os2.CodePageRanges = new CodePageRange(zero, one);

                if (os2.Version >= OS2TableVersion.OpenType12)
                {
                    os2.Height = reader.ReadInt16();
                    os2.CapHeight = reader.ReadInt16();
                    os2.DefaultChar = reader.ReadUInt16();
                    os2.BreakChar = reader.ReadUInt16();
                    os2.MaxContext = reader.ReadUInt16();
                }
            }

            return os2;
        }

        #endregion

        #region post

        private TrueTypeFontTable ReadPostscriptTable(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            PostscriptTable post = new PostscriptTable(reader.Position);
            post.TableVersion = reader.ReadFixedVersion();
            post.ItalicAngle = reader.ReadFixed1616();
            post.UnderlinePosition = reader.ReadInt16();
            post.UnderlineThickness = reader.ReadInt16();
            post.FixedPitch = reader.ReadUInt32();
            post.MinMemoryOpenType = reader.ReadUInt32();
            post.MaxMemoryOpenType = reader.ReadUInt32();
            post.MinMemoryType1 = reader.ReadUInt32();
            post.MaxMemoryType1 = reader.ReadUInt32();


            //TODO: Read the names if correct version.
            if (post.TableVersion == new Version(2, 0))
            {
                List<GlyphName> glyphnammes = new List<GlyphName>();
                Dictionary<int, GlyphName> offsets = new Dictionary<int, GlyphName>();

                ushort count = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    GlyphName gname = new GlyphName();
                    gname.Index = reader.ReadUInt16();
                    if (gname.Index > 257)
                        offsets.Add(gname.Index - 258, gname);
                    glyphnammes.Add(gname);
                }
                post.Names = glyphnammes;

                if (offsets.Count > 0)
                {
                    count = (ushort)offsets.Count;
                    for (int i = 0; i < count; i++)
                    {
                        string s = reader.ReadPascalString();
                        GlyphName g;
                        if (offsets.TryGetValue(i, out g))
                            g.Name = s;
                    }
                }
            }
            else if (post.TableVersion == new Version(2, 5))
            {
            }
            return post;
        }

        #endregion

        #region kern

        private TrueTypeFontTable ReadKerningTable(uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            KerningTable kern = new KerningTable(reader.Position);
            kern.TableVersion = reader.ReadUShortVersion();
            ushort tcount = reader.ReadUInt16();
            kern.TableCount = tcount;

            if (tcount > 0)
            {
                kern.SubTables = new List<KerningSubTable>();

                for (int i = 0; i < tcount; i++)
                {
                    KerningSubTable sub = new KerningSubTable();
                    sub.Version = reader.ReadUShortVersion();
                    sub.Length = reader.ReadUInt16();
                    sub.Coverage = (KerningCoverage)reader.ReadByte();
                    sub.Format = (KerningFormat)reader.ReadByte();

                    if (sub.Format == KerningFormat.Format0)
                    {
                        KerningFormat0 fdata = new KerningFormat0();
                        fdata.PairCount = reader.ReadUInt16();
                        fdata.SearchRange = reader.ReadUInt16();
                        fdata.EntrySelector = reader.ReadUInt16();
                        fdata.RangeShift = reader.ReadUInt16();

                        List<Kerning0Pair> pairs = new List<Kerning0Pair>();

                        for (int j = 0; j < fdata.PairCount; j++)
                        {
                            Kerning0Pair pair = new Kerning0Pair();
                            pair.LeftGlyphIndex = reader.ReadUInt16();
                            pair.RightGlyphIndex = reader.ReadUInt16();
                            pair.Value = reader.ReadInt16();
                            pairs.Add(pair);
                        }
                        fdata.KerningPairs = pairs;
                        sub.KerningFormatData = fdata;
                    }

                    kern.SubTables.Add(sub);
                }
            }

            return kern;
        }

        #endregion
    }


}
