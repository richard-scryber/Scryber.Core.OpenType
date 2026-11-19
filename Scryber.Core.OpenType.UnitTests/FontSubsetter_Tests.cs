using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// Subsetting TrueType fonts - only the outlines of the glyphs used are kept, with the same glyph numbers.
    /// The subset data is read here with a separate reader of the tables, and by the TypefaceReader.
    /// </summary>
    [TestClass()]
    [TestCategory("Font Subsetter")]
    public class FontSubsetter_Tests
    {
        private const string RobotoPath = "fonts/Roboto.ttf";
        private const string OpenSansPath = "fonts/OpenSans-Light.ttf";
        private const string NotoPath = "fonts/NotoTC.otf";
        private const string CourierCollectionPath = "fonts/Courier.ttc";

        //
        // helpers
        //

        private static byte[] ReadFontData(string path)
        {
            var full = System.IO.Path.Combine(System.Environment.CurrentDirectory, path);
            Assert.IsTrue(File.Exists(full), "The test font " + full + " was not found");
            return File.ReadAllBytes(full);
        }

        private static ITypefaceFont ReadFont(byte[] data)
        {
            using (var reader = new TypefaceReader())
            using (var ms = new MemoryStream(data))
            {
                var info = reader.ReadTypeface(ms, "subset");
                Assert.IsNotNull(info, "The font data could not be read");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "The font data had an error: " + info.ErrorMessage);
                ms.Position = 0;
                var font = reader.GetFonts(ms, "subset").FirstOrDefault();
                Assert.IsNotNull(font, "No font was read from the data");
                return font;
            }
        }

        /// <summary>
        /// The glyph numbers of the characters in the font, from its unicode cmap
        /// </summary>
        private static int[] GlyphsFor(ITypefaceFont font, string chars)
        {
            var otf = font as IOpenTypeFont;
            Assert.IsNotNull(otf, "The font is not an open type font");
            Assert.IsTrue(otf.TryGetTable<CMAPTable>("cmap", out var cmap), "The font has no cmap");
            var map = cmap.GetOffsetTable(CMapEncoding.Windows(WindowsCharacterEncodings.Unicode))
                      ?? cmap.GetOffsetTable(CMapEncoding.UnicodeDefault);
            Assert.IsNotNull(map, "The font has no unicode cmap");

            return chars.Select(c => map.GetCharacterGlyphOffset(c)).Distinct().ToArray();
        }

        private sealed class Table
        {
            public string Tag;
            public uint Checksum;
            public int Offset;
            public int Length;
        }

        private static Dictionary<string, Table> ReadDirectory(byte[] data)
        {
            int count = (data[4] << 8) | data[5];
            var all = new Dictionary<string, Table>();
            for (var i = 0; i < count; i++)
            {
                var pos = 12 + (i * 16);
                all[Encoding.ASCII.GetString(data, pos, 4)] = new Table()
                {
                    Tag = Encoding.ASCII.GetString(data, pos, 4),
                    Checksum = U32(data, pos + 4),
                    Offset = (int)U32(data, pos + 8),
                    Length = (int)U32(data, pos + 12)
                };
            }
            return all;
        }

        private static uint U32(byte[] d, int p) => ((uint)d[p] << 24) | ((uint)d[p + 1] << 16) | ((uint)d[p + 2] << 8) | d[p + 3];
        private static int U16(byte[] d, int p) => (d[p] << 8) | d[p + 1];
        private static int I16(byte[] d, int p) => (short)U16(d, p);

        private static uint Sum(byte[] d, int offset, int length)
        {
            uint sum = 0;
            for (var i = offset; i < offset + length; i += 4)
            {
                uint word = 0;
                for (var b = 0; b < 4; b++)
                    word = (word << 8) | (i + b < offset + length ? d[i + b] : (byte)0);
                sum = unchecked(sum + word);
            }
            return sum;
        }

        /// <summary>
        /// Returns the outline data of each glyph in the font (empty if it has no outline)
        /// </summary>
        private static byte[][] ReadGlyphs(byte[] data)
        {
            var tables = ReadDirectory(data);
            var head = tables["head"];
            var loca = tables["loca"];
            var glyf = tables["glyf"];
            int numGlyphs = U16(data, tables["maxp"].Offset + 4);
            bool longFormat = I16(data, head.Offset + 50) == 1;

            var glyphs = new byte[numGlyphs][];
            for (var i = 0; i < numGlyphs; i++)
            {
                long start = longFormat ? U32(data, loca.Offset + i * 4) : U16(data, loca.Offset + i * 2) * 2;
                long end = longFormat ? U32(data, loca.Offset + (i + 1) * 4) : U16(data, loca.Offset + (i + 1) * 2) * 2;
                var glyph = new byte[Math.Max(0, end - start)];
                Buffer.BlockCopy(data, (int)(glyf.Offset + start), glyph, 0, glyph.Length);
                glyphs[i] = glyph;
            }
            return glyphs;
        }

        /// <summary>
        /// The outline data, without the padding at the end (the subset pads each glyph to 4 bytes)
        /// </summary>
        private static void AssertSameOutline(byte[] original, byte[] subset, int gid)
        {
            Assert.IsTrue(subset.Length >= original.Length && subset.Length - original.Length < 4, "Glyph " + gid + " has the length of the original (with padding)");
            for (var i = 0; i < original.Length; i++)
                Assert.AreEqual(original[i], subset[i], "Glyph " + gid + " byte " + i + " is the same as the original");
        }

        private static int[] ComponentsOf(byte[] glyph)
        {
            var result = new List<int>();
            if (glyph.Length < 10 || I16(glyph, 0) >= 0)
                return result.ToArray();

            var pos = 10;
            int flags;
            do
            {
                flags = U16(glyph, pos);
                result.Add(U16(glyph, pos + 2));
                pos += 4 + ((flags & 0x0001) != 0 ? 4 : 2);
                if ((flags & 0x0008) != 0) pos += 2;
                else if ((flags & 0x0040) != 0) pos += 4;
                else if ((flags & 0x0080) != 0) pos += 8;
            } while ((flags & 0x0020) != 0);
            return result.ToArray();
        }

        //
        // tests
        //

        [TestMethod()]
        public void FontSubsetter_01_SubsetIsSmaller()
        {
            var data = ReadFontData(RobotoPath);
            var font = ReadFont(data);
            var glyphs = GlyphsFor(font, "Hello World");

            var result = FontSubsetter.Subset(data, glyphs);

            Assert.IsTrue(result.Success, "The font was subset: " + result.Reason);
            Assert.IsNull(result.Reason);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data.Length < data.Length / 4, "The subset (" + result.Data.Length + ") is much smaller than the font (" + data.Length + ")");

            var originalGlyphs = ReadGlyphs(data);
            Assert.AreEqual(originalGlyphs.Length, result.GlyphCount, "The subset has the same number of glyphs");

            //.notdef and the 8 different characters of Hello World (H e l o space W r d)
            Assert.AreEqual(glyphs.Length + 1, result.IncludedGlyphCount);
        }

        [TestMethod()]
        public void FontSubsetter_02_SubsetReadsAsTheSameFont()
        {
            var data = ReadFontData(RobotoPath);
            var font = ReadFont(data);
            var result = FontSubsetter.Subset(data, GlyphsFor(font, "The quick brown fox"));
            Assert.IsTrue(result.Success, result.Reason);

            var subset = ReadFont(result.Data);
            Assert.IsNotNull(subset);
            Assert.IsTrue(subset.IsValid, "The subset is a valid font");
            Assert.AreEqual(font.FamilyName, subset.FamilyName);
            Assert.AreEqual(font.FontWeight, subset.FontWeight);
            Assert.AreEqual(font.Restrictions, subset.Restrictions);

            ValidateRoboto.AssertMetrics(subset.GetMetrics(TypeMeasureOptions.Default));

            //the characters measure the same in the subset
            var original = font.GetMetrics(TypeMeasureOptions.Default).MeasureLine("The quick brown fox", 0, 12, 1000, TypeMeasureOptions.Default);
            var measured = subset.GetMetrics(TypeMeasureOptions.Default).MeasureLine("The quick brown fox", 0, 12, 1000, TypeMeasureOptions.Default);
            Assert.AreEqual(original.RequiredWidth, measured.RequiredWidth, 0.0001, "The same width in the subset");
        }

        [TestMethod()]
        public void FontSubsetter_03_KeepsTheOutlinesUsedWithTheSameGlyphNumbers()
        {
            var data = ReadFontData(RobotoPath);
            var font = ReadFont(data);
            var used = GlyphsFor(font, "Scryber");
            var result = FontSubsetter.Subset(data, used);
            Assert.IsTrue(result.Success, result.Reason);

            var original = ReadGlyphs(data);
            var subset = ReadGlyphs(result.Data);
            Assert.AreEqual(original.Length, subset.Length);

            var kept = new HashSet<int>(used) { 0 };
            for (var gid = 0; gid < original.Length; gid++)
            {
                if (kept.Contains(gid))
                    AssertSameOutline(original[gid], subset[gid], gid);
                else
                    Assert.AreEqual(0, subset[gid].Length, "Glyph " + gid + " is not used, so it has no outline");
            }
        }

        [TestMethod()]
        public void FontSubsetter_04_ChecksumsAreValid()
        {
            var data = ReadFontData(RobotoPath);
            var font = ReadFont(data);
            var result = FontSubsetter.Subset(data, GlyphsFor(font, "Checksums"));
            Assert.IsTrue(result.Success, result.Reason);

            var subset = result.Data;
            var tables = ReadDirectory(subset);
            foreach (var table in tables.Values)
            {
                Assert.AreEqual(0, table.Offset % 4, table.Tag + " is on a 4 byte boundary");
                if (table.Tag == "head")
                {
                    //the head checksum is of the table with a zero adjustment
                    var copy = (byte[])subset.Clone();
                    copy[table.Offset + 8] = copy[table.Offset + 9] = copy[table.Offset + 10] = copy[table.Offset + 11] = 0;
                    Assert.AreEqual(table.Checksum, Sum(copy, table.Offset, table.Length), "head checksum");
                }
                else
                    Assert.AreEqual(table.Checksum, Sum(subset, table.Offset, table.Length), table.Tag + " checksum");
            }

            //the whole font sums to the magic number with the adjustment in head
            Assert.AreEqual(0xB1B0AFBA, Sum(subset, 0, subset.Length), "The whole font checksum");

            //and the table directory search values
            int count = U16(subset, 4);
            int entrySelector = (int)Math.Floor(Math.Log(count, 2));
            Assert.AreEqual((1 << entrySelector) * 16, U16(subset, 6), "searchRange");
            Assert.AreEqual(entrySelector, U16(subset, 8), "entrySelector");
            Assert.AreEqual(count * 16 - (1 << entrySelector) * 16, U16(subset, 10), "rangeShift");
        }

        [TestMethod()]
        public void FontSubsetter_05_CompositeGlyphsKeepTheirComponents()
        {
            var data = ReadFontData(RobotoPath);
            var original = ReadGlyphs(data);

            //find a composite glyph in the font (e.g. an accented letter)
            var composite = -1;
            for (var gid = 1; gid < original.Length && composite < 0; gid++)
            {
                if (ComponentsOf(original[gid]).Length > 1)
                    composite = gid;
            }
            Assert.IsTrue(composite > 0, "Roboto has a composite glyph");
            var components = ComponentsOf(original[composite]);

            var result = FontSubsetter.Subset(data, new[] { composite });
            Assert.IsTrue(result.Success, result.Reason);

            var subset = ReadGlyphs(result.Data);
            AssertSameOutline(original[composite], subset[composite], composite);
            foreach (var component in components)
            {
                Assert.IsTrue(subset[component].Length > 0, "The component " + component + " of glyph " + composite + " is kept");
                AssertSameOutline(original[component], subset[component], component);
            }
            Assert.IsTrue(result.IncludedGlyphCount >= 2 + components.Distinct().Count() - (components.Contains(0) ? 1 : 0));
        }

        [TestMethod()]
        public void FontSubsetter_06_OnlyTheTablesNeededAreKept()
        {
            var data = ReadFontData(RobotoPath);
            var originalTables = ReadDirectory(data);
            var font = ReadFont(data);
            var result = FontSubsetter.Subset(data, GlyphsFor(font, "Tables"));
            Assert.IsTrue(result.Success, result.Reason);

            var tables = ReadDirectory(result.Data);
            var allowed = new[] { "OS/2", "cmap", "cvt ", "fpgm", "glyf", "head", "hhea", "hmtx", "loca", "maxp", "name", "post", "prep" };
            foreach (var tag in tables.Keys)
                Assert.IsTrue(allowed.Contains(tag), "The table " + tag + " is not needed in the subset");

            foreach (var tag in allowed)
                Assert.AreEqual(originalTables.ContainsKey(tag), tables.ContainsKey(tag), "The table " + tag + " is kept if the font has it");

            Assert.IsFalse(tables.ContainsKey("GPOS") || tables.ContainsKey("GSUB"), "Layout tables are not kept");

            //tables are in tag order
            var tags = tables.Keys.ToList();
            CollectionAssert.AreEqual(tags.OrderBy(t => t, StringComparer.Ordinal).ToList(), tags);

            //long loca, post version 3 without glyph names
            Assert.AreEqual(1, I16(result.Data, tables["head"].Offset + 50), "Long loca offsets");
            Assert.AreEqual((ReadGlyphs(data).Length + 1) * 4, tables["loca"].Length);
            Assert.AreEqual(0x00030000u, U32(result.Data, tables["post"].Offset));
            Assert.AreEqual(32, tables["post"].Length);

            //the other tables are unchanged
            foreach (var tag in new[] { "hhea", "hmtx", "maxp", "OS/2", "cmap" })
            {
                var o = originalTables[tag];
                var s = tables[tag];
                Assert.AreEqual(o.Length, s.Length, tag + " length");
                for (var i = 0; i < o.Length; i++)
                    Assert.AreEqual(data[o.Offset + i], result.Data[s.Offset + i], tag + " byte " + i);
            }
        }

        [TestMethod()]
        public void FontSubsetter_07_CFFFontsAreNotSubset()
        {
            var data = ReadFontData(NotoPath);

            Assert.IsFalse(FontSubsetter.CanSubset(data, out var reason));
            StringAssert.Contains(reason, "CFF");

            var result = FontSubsetter.Subset(data, new[] { 1, 2, 3 });
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
            StringAssert.Contains(result.Reason, "CFF");
        }

        [TestMethod()]
        public void FontSubsetter_08_NoSubsettingLicenceIsRespected()
        {
            var data = ReadFontData(RobotoPath);
            Assert.IsTrue(FontSubsetter.CanSubset(data, out var reason), reason);

            //set the no-subsetting bit of the OS/2 fsType
            var restricted = (byte[])data.Clone();
            var os2 = ReadDirectory(restricted)["OS/2"];
            var fsType = U16(restricted, os2.Offset + 8) | FontSubsetter.NoSubsettingFlag;
            restricted[os2.Offset + 8] = (byte)(fsType >> 8);
            restricted[os2.Offset + 9] = (byte)fsType;

            Assert.IsFalse(FontSubsetter.CanSubset(restricted, out reason));
            StringAssert.Contains(reason, "licence");

            var result = FontSubsetter.Subset(restricted, new[] { 1, 2, 3 });
            Assert.IsFalse(result.Success);
            StringAssert.Contains(result.Reason, "0x0100");
        }

        [TestMethod()]
        public void FontSubsetter_09_CollectionsAndOtherDataAreNotSubset()
        {
            var collection = ReadFontData(CourierCollectionPath);
            var result = FontSubsetter.Subset(collection, new[] { 1 });
            Assert.IsFalse(result.Success);
            StringAssert.Contains(result.Reason, "collection");

            result = FontSubsetter.Subset(new byte[] { 1, 2, 3 }, new[] { 1 });
            Assert.IsFalse(result.Success);

            result = FontSubsetter.Subset(Encoding.ASCII.GetBytes("Not a font, but long enough to read a header"), new[] { 1 });
            Assert.IsFalse(result.Success);

            Assert.ThrowsException<ArgumentNullException>(() => FontSubsetter.Subset(null, new[] { 1 }));
            Assert.ThrowsException<ArgumentNullException>(() => FontSubsetter.Subset(collection, null));
        }

        [TestMethod()]
        public void FontSubsetter_10_GlyphsOutsideTheFontAreIgnored()
        {
            var data = ReadFontData(RobotoPath);
            var original = ReadGlyphs(data);

            var result = FontSubsetter.Subset(data, new[] { -1, original.Length, original.Length + 100, 0 });
            Assert.IsTrue(result.Success, result.Reason);
            Assert.AreEqual(1, result.IncludedGlyphCount, "Only .notdef");

            var subset = ReadGlyphs(result.Data);
            AssertSameOutline(original[0], subset[0], 0);
            Assert.AreEqual(0, subset.Skip(1).Sum(g => g.Length), "No other outlines");

            //and no glyphs at all is .notdef only
            result = FontSubsetter.Subset(data, new int[0]);
            Assert.IsTrue(result.Success, result.Reason);
            Assert.AreEqual(1, result.IncludedGlyphCount);
        }

        [TestMethod()]
        public void FontSubsetter_11_ShortLocaFont()
        {
            //Open Sans has a short loca (or long) - either way the subset is long, and reads as the same font
            var data = ReadFontData(OpenSansPath);
            var font = ReadFont(data);
            var used = GlyphsFor(font, "Open Sans Light 0123456789");
            var result = FontSubsetter.Subset(data, used);
            Assert.IsTrue(result.Success, result.Reason);
            Assert.IsTrue(result.Data.Length < data.Length / 2, "The subset is smaller");

            var original = ReadGlyphs(data);
            var subset = ReadGlyphs(result.Data);
            foreach (var gid in used)
                AssertSameOutline(original[gid], subset[gid], gid);

            var read = ReadFont(result.Data);
            Assert.IsTrue(read.IsValid);
            Assert.AreEqual(font.FamilyName, read.FamilyName);
        }
    }
}
