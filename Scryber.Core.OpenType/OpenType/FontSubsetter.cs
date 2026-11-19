using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Scryber.OpenType
{
    /// <summary>
    /// The result of subsetting a font - the data of the subset font, or the reason the font could not be subset.
    /// </summary>
    public sealed class FontSubsetResult
    {
        /// <summary>
        /// True if the font was subset, and the Data is the subset font
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// The data of the subset font (a TrueType font), or null if the font could not be subset
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The reason the font could not be subset (or null on success)
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// The number of glyphs in the font (the subset keeps the same glyph numbers)
        /// </summary>
        public int GlyphCount { get; private set; }

        /// <summary>
        /// The number of glyphs that have outlines in the subset - .notdef, the glyphs requested and the parts of any composite glyphs
        /// </summary>
        public int IncludedGlyphCount { get; private set; }

        internal static FontSubsetResult Succeeded(byte[] data, int glyphCount, int included)
        {
            return new FontSubsetResult() { Success = true, Data = data, GlyphCount = glyphCount, IncludedGlyphCount = included };
        }

        internal static FontSubsetResult Failed(string reason)
        {
            return new FontSubsetResult() { Success = false, Reason = reason };
        }
    }

    /// <summary>
    /// Creates a subset of a TrueType font (glyf outlines) with only the outlines of the glyphs used, to embed in a document.
    /// </summary>
    /// <remarks>
    /// <para>The subset keeps the glyph numbers of the original font - the glyphs that are not used are empty (they have no outline),
    /// so text that refers to the font by glyph number (e.g. a PDF CIDFontType2 with an Identity CIDToGIDMap) is unchanged.
    /// Glyph 0 (.notdef) and the parts of composite glyphs are always kept.</para>
    /// <para>Only the tables needed to draw the glyphs are kept: head, hhea, hmtx, maxp, loca, glyf, and cvt, fpgm and prep for the
    /// hinting, with cmap, OS/2, name and post (as version 3, without glyph names) for the font information. Layout tables
    /// (GSUB, GPOS, kern...) are not kept. The loca table is written in the long format.</para>
    /// <para>A font cannot be subset if it has CFF (PostScript) outlines, is a collection, or its OS/2 fsType has the
    /// no-subsetting bit (0x0100) set. The result then has the reason, and the whole font should be used.</para>
    /// </remarks>
    public static class FontSubsetter
    {
        /// <summary>
        /// The fsType bit that means the font must not be subset before embedding
        /// </summary>
        public const ushort NoSubsettingFlag = 0x0100;

        private const uint ChecksumMagic = 0xB1B0AFBA;

        private static readonly string[] KeptTables = new string[]
        {
            "OS/2", "cmap", "cvt ", "fpgm", "glyf", "head", "hhea", "hmtx", "loca", "maxp", "name", "post", "prep"
        };

        private static readonly string[] RequiredTables = new string[]
        {
            "glyf", "head", "hhea", "hmtx", "loca", "maxp"
        };

        /// <summary>
        /// Returns true if the TrueType font data can be subset, or false with the reason it cannot
        /// </summary>
        public static bool CanSubset(byte[] fontData, out string reason)
        {
            var tables = ReadTables(fontData, out reason);
            if (null == tables)
                return false;

            return CanSubset(fontData, tables, out reason);
        }

        /// <summary>
        /// Creates a subset of the TrueType font data, keeping the outlines of the glyphs (by glyph number), and of .notdef,
        /// and the components of any composite glyphs. Glyph numbers outside the font are ignored.
        /// </summary>
        /// <param name="fontData">The data of a single TrueType font (not a collection, or a WOFF)</param>
        /// <param name="glyphs">The glyph numbers that are used</param>
        /// <returns>The result with the subset font data, or the reason it could not be subset</returns>
        public static FontSubsetResult Subset(byte[] fontData, IEnumerable<int> glyphs)
        {
            if (null == fontData)
                throw new ArgumentNullException(nameof(fontData));
            if (null == glyphs)
                throw new ArgumentNullException(nameof(glyphs));

            string reason;
            var tables = ReadTables(fontData, out reason);
            if (null == tables)
                return FontSubsetResult.Failed(reason);

            if (!CanSubset(fontData, tables, out reason))
                return FontSubsetResult.Failed(reason);

            try
            {
                return DoSubset(fontData, tables, glyphs);
            }
            catch (Exception ex)
            {
                return FontSubsetResult.Failed("The font data could not be read to subset: " + ex.Message);
            }
        }

        //
        // implementation
        //

        private sealed class TableRecord
        {
            public string Tag;
            public uint Offset;
            public uint Length;
        }

        private static Dictionary<string, TableRecord> ReadTables(byte[] data, out string reason)
        {
            reason = null;
            if (null == data || data.Length < 12)
            {
                reason = "The font data is too short to be a font";
                return null;
            }

            var version = ReadUInt32(data, 0);
            if (version == 0x74746366) // 'ttcf'
            {
                reason = "The font data is a collection - the data of a single font is needed to subset";
                return null;
            }
            if (version == 0x4F54544F) // 'OTTO'
            {
                reason = "The font has CFF (PostScript) outlines, which cannot be subset";
                return null;
            }
            if (version == 0x774F4646 || version == 0x774F4632) // 'wOFF', 'wOF2'
            {
                reason = "The font data is a WOFF font - the TrueType data is needed to subset";
                return null;
            }
            if (version != 0x00010000 && version != 0x74727565) // 1.0 or 'true'
            {
                reason = "The font data is not a TrueType font";
                return null;
            }

            int count = ReadUInt16(data, 4);
            if (data.Length < 12 + (count * 16))
            {
                reason = "The font table directory is incomplete";
                return null;
            }

            var tables = new Dictionary<string, TableRecord>(count);
            for (var i = 0; i < count; i++)
            {
                var pos = 12 + (i * 16);
                var record = new TableRecord()
                {
                    Tag = Encoding.ASCII.GetString(data, pos, 4),
                    Offset = ReadUInt32(data, pos + 8),
                    Length = ReadUInt32(data, pos + 12)
                };

                if ((long)record.Offset + record.Length > data.Length)
                {
                    reason = "The table '" + record.Tag + "' is outside the font data";
                    return null;
                }
                tables[record.Tag] = record;
            }
            return tables;
        }

        private static bool CanSubset(byte[] data, Dictionary<string, TableRecord> tables, out string reason)
        {
            reason = null;

            if (tables.ContainsKey("CFF ") || tables.ContainsKey("CFF2"))
            {
                reason = "The font has CFF (PostScript) outlines, which cannot be subset";
                return false;
            }

            foreach (var required in RequiredTables)
            {
                if (!tables.ContainsKey(required))
                {
                    reason = "The font does not have a '" + required.Trim() + "' table, so cannot be subset";
                    return false;
                }
            }

            if (tables.TryGetValue("OS/2", out var os2) && os2.Length >= 10)
            {
                var fsType = ReadUInt16(data, (int)os2.Offset + 8);
                if ((fsType & NoSubsettingFlag) != 0)
                {
                    reason = "The licence of the font (OS/2 fsType 0x" + fsType.ToString("X4") + ") does not allow it to be subset";
                    return false;
                }
            }

            return true;
        }

        private static FontSubsetResult DoSubset(byte[] data, Dictionary<string, TableRecord> tables, IEnumerable<int> glyphs)
        {
            var head = tables["head"];
            var maxp = tables["maxp"];
            var loca = tables["loca"];
            var glyf = tables["glyf"];

            int numGlyphs = ReadUInt16(data, (int)maxp.Offset + 4);
            int locaFormat = ReadInt16(data, (int)head.Offset + 50);

            //read the glyph offsets
            var offsets = new uint[numGlyphs + 1];
            for (var i = 0; i <= numGlyphs; i++)
            {
                uint offset;
                if (locaFormat == 0)
                    offset = (uint)(ReadUInt16(data, (int)loca.Offset + (i * 2)) * 2);
                else
                    offset = ReadUInt32(data, (int)loca.Offset + (i * 4));

                if (offset > glyf.Length)
                    offset = glyf.Length;
                offsets[i] = offset;
            }

            //the glyphs to keep - .notdef, the glyphs used, and the components of any composite glyphs
            var keep = new bool[numGlyphs];
            var pending = new Stack<int>();
            keep[0] = true;
            pending.Push(0);

            foreach (var gid in glyphs)
            {
                if (gid >= 0 && gid < numGlyphs && !keep[gid])
                {
                    keep[gid] = true;
                    pending.Push(gid);
                }
            }

            while (pending.Count > 0)
            {
                var gid = pending.Pop();
                foreach (var component in GetComponents(data, glyf, offsets, gid))
                {
                    if (component < numGlyphs && !keep[component])
                    {
                        keep[component] = true;
                        pending.Push(component);
                    }
                }
            }

            //the new glyf and loca (long format), with an empty outline for the glyphs not kept
            int included = 0;
            var newGlyf = new MemoryStream();
            var newLoca = new byte[(numGlyphs + 1) * 4];

            for (var gid = 0; gid < numGlyphs; gid++)
            {
                WriteUInt32(newLoca, gid * 4, (uint)newGlyf.Length);

                if (keep[gid])
                {
                    included++;
                    var start = offsets[gid];
                    var length = offsets[gid + 1] > start ? offsets[gid + 1] - start : 0;
                    if (length > 0)
                    {
                        newGlyf.Write(data, (int)(glyf.Offset + start), (int)length);

                        //each glyph on a 4 byte boundary
                        while (newGlyf.Length % 4 != 0)
                            newGlyf.WriteByte(0);
                    }
                }
            }
            WriteUInt32(newLoca, numGlyphs * 4, (uint)newGlyf.Length);

            //the tables to write
            var output = new SortedDictionary<string, byte[]>(StringComparer.Ordinal);
            foreach (var tag in KeptTables)
            {
                if (!tables.TryGetValue(tag, out var record))
                    continue;

                byte[] table;
                if (tag == "glyf")
                    table = newGlyf.ToArray();
                else if (tag == "loca")
                    table = newLoca;
                else if (tag == "post")
                    table = CreatePostVersion3(data, record);
                else
                {
                    table = new byte[record.Length];
                    Buffer.BlockCopy(data, (int)record.Offset, table, 0, (int)record.Length);
                }

                if (tag == "head")
                {
                    //long loca offsets, and the checksum adjustment is set after the font is written
                    WriteUInt16(table, 50, 1);
                    WriteUInt32(table, 8, 0);
                }

                output[tag] = table;
            }

            var font = WriteFont(output);
            return FontSubsetResult.Succeeded(font, numGlyphs, included);
        }

        /// <summary>
        /// Returns the glyph numbers of the components of a composite glyph (or none for a simple glyph)
        /// </summary>
        private static IEnumerable<int> GetComponents(byte[] data, TableRecord glyf, uint[] offsets, int gid)
        {
            var start = offsets[gid];
            var end = offsets[gid + 1];
            if (end <= start || end - start < 10)
                yield break;

            var pos = (int)(glyf.Offset + start);
            var limit = (int)(glyf.Offset + end);
            int contours = ReadInt16(data, pos);
            if (contours >= 0)
                yield break;

            //composite - after the glyph header (10 bytes), each component is flags, glyph index, arguments and an optional transform
            pos += 10;

            const ushort ArgsAreWords = 0x0001;
            const ushort HasScale = 0x0008;
            const ushort MoreComponents = 0x0020;
            const ushort HasXYScale = 0x0040;
            const ushort HasTwoByTwo = 0x0080;

            ushort flags;
            do
            {
                if (pos + 4 > limit)
                    yield break;

                flags = ReadUInt16(data, pos);
                int component = ReadUInt16(data, pos + 2);
                pos += 4;

                yield return component;

                pos += (flags & ArgsAreWords) != 0 ? 4 : 2;

                if ((flags & HasScale) != 0)
                    pos += 2;
                else if ((flags & HasXYScale) != 0)
                    pos += 4;
                else if ((flags & HasTwoByTwo) != 0)
                    pos += 8;
            }
            while ((flags & MoreComponents) != 0);
        }

        /// <summary>
        /// The post table without the glyph names (version 3.0) - the first 32 bytes, with the version changed
        /// </summary>
        private static byte[] CreatePostVersion3(byte[] data, TableRecord post)
        {
            var table = new byte[32];
            var length = (int)Math.Min(32, post.Length);
            Buffer.BlockCopy(data, (int)post.Offset, table, 0, length);
            WriteUInt32(table, 0, 0x00030000);
            return table;
        }

        private static byte[] WriteFont(SortedDictionary<string, byte[]> tables)
        {
            int count = tables.Count;
            int entrySelector = 0;
            while ((1 << (entrySelector + 1)) <= count)
                entrySelector++;
            int searchRange = (1 << entrySelector) * 16;
            int rangeShift = (count * 16) - searchRange;

            int headerLength = 12 + (count * 16);
            long total = headerLength;
            foreach (var table in tables.Values)
                total += Pad4(table.Length);

            var font = new byte[total];
            WriteUInt32(font, 0, 0x00010000);
            WriteUInt16(font, 4, (ushort)count);
            WriteUInt16(font, 6, (ushort)searchRange);
            WriteUInt16(font, 8, (ushort)entrySelector);
            WriteUInt16(font, 10, (ushort)rangeShift);

            int entry = 12;
            int offset = headerLength;
            int headOffset = -1;

            foreach (var pair in tables)
            {
                var table = pair.Value;
                Buffer.BlockCopy(table, 0, font, offset, table.Length);

                var tag = Encoding.ASCII.GetBytes(pair.Key);
                Buffer.BlockCopy(tag, 0, font, entry, 4);
                WriteUInt32(font, entry + 4, Checksum(font, offset, table.Length));
                WriteUInt32(font, entry + 8, (uint)offset);
                WriteUInt32(font, entry + 12, (uint)table.Length);

                if (pair.Key == "head")
                    headOffset = offset;

                entry += 16;
                offset += Pad4(table.Length);
            }

            //the checksum of the whole font, with the adjustment in the head table, is the magic number
            if (headOffset >= 0)
            {
                var sum = Checksum(font, 0, font.Length);
                WriteUInt32(font, headOffset + 8, unchecked(ChecksumMagic - sum));
            }

            return font;
        }

        /// <summary>
        /// The sum of the data as big endian 32 bit words (the last word padded with zeros)
        /// </summary>
        internal static uint Checksum(byte[] data, int offset, int length)
        {
            uint sum = 0;
            int end = offset + length;
            for (var i = offset; i < end; i += 4)
            {
                uint word = 0;
                for (var b = 0; b < 4; b++)
                {
                    word <<= 8;
                    if (i + b < end)
                        word |= data[i + b];
                }
                sum = unchecked(sum + word);
            }
            return sum;
        }

        private static int Pad4(int length)
        {
            return (length + 3) & ~3;
        }

        private static ushort ReadUInt16(byte[] data, int pos)
        {
            return (ushort)((data[pos] << 8) | data[pos + 1]);
        }

        private static short ReadInt16(byte[] data, int pos)
        {
            return (short)ReadUInt16(data, pos);
        }

        private static uint ReadUInt32(byte[] data, int pos)
        {
            return ((uint)data[pos] << 24) | ((uint)data[pos + 1] << 16) | ((uint)data[pos + 2] << 8) | data[pos + 3];
        }

        private static void WriteUInt16(byte[] data, int pos, ushort value)
        {
            data[pos] = (byte)(value >> 8);
            data[pos + 1] = (byte)value;
        }

        private static void WriteUInt32(byte[] data, int pos, uint value)
        {
            data[pos] = (byte)(value >> 24);
            data[pos + 1] = (byte)(value >> 16);
            data[pos + 2] = (byte)(value >> 8);
            data[pos + 3] = (byte)value;
        }
    }
}
