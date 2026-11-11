using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// The file data of a font from a collection (ttc) must be a single font program of its own, as it is
    /// embedded in documents. The table offsets of a font in a collection are from the start of the collection file,
    /// so the data from the font header to the end of the file is not a valid font.
    /// </summary>
    [TestClass()]
    public class TypefaceReader_CollectionFileData
    {
        [TestMethod("1. Each Gill Sans font from the collection stream has the data of a single font")]
        public void CollectionFontsFromStream_HaveSingleFontData()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);
            var file = new FileInfo(Path.Combine(path.FullName, ValidateGillSans.UrlPath));

            using (var reader = new TypefaceReader(path))
            using (var stream = file.OpenRead())
            {
                //As the font factory in Scryber.Core reads the fonts in a collection
                var info = reader.ReadTypeface(stream, file.FullName);
                Assert.AreEqual(DataFormat.TTC, info.SourceFormat);
                Assert.AreEqual(ValidateGillSans.FontTypefaces.Length, info.FontCount);

                for (var i = 0; i < info.FontCount; i++)
                {
                    var face = reader.GetFont(stream, file.FullName, info.Fonts[i]);
                    Assert.IsNotNull(face, "Font " + i);

                    AssertSingleFontData(face, file.Length, i);
                }
            }
        }

        [TestMethod("2. The Gill Sans bold font from the collection file has the data of a single font")]
        public void CollectionFontFromFile_HasSingleFontData()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);
            var file = new FileInfo(Path.Combine(path.FullName, ValidateGillSans.UrlPath));

            using (var reader = new TypefaceReader(path))
            {
                var face = reader.GetFont(file, ValidateGillSans.BoldRegularIndex);
                Assert.IsNotNull(face);

                AssertSingleFontData(face, file.Length, ValidateGillSans.BoldRegularIndex);
            }
        }

        private static void AssertSingleFontData(ITypefaceFont face, long collectionLength, int index)
        {
            Assert.IsTrue(face.CanGetFileData(DataFormat.TTF), "Cannot get the TTF data for font " + index);
            var data = face.GetFileData(DataFormat.TTF);
            Assert.IsNotNull(data, "No data for font " + index);
            Assert.IsTrue(data.Length < collectionLength, "The data for font " + index + " is not smaller than the collection");

            //every table in the directory is inside the data
            var tableCount = (data[4] << 8) | data[5];
            Assert.IsTrue(tableCount > 0, "No tables for font " + index);
            for (var t = 0; t < tableCount; t++)
            {
                var entry = 12 + (16 * t);
                var offset = ReadUInt32(data, entry + 8);
                var length = ReadUInt32(data, entry + 12);
                Assert.IsTrue(offset + length <= data.Length, "Table " + t + " of font " + index + " is outside the data");
            }

            //and it reads back as the same font on its own
            using (var reader = new TypefaceReader())
            using (var ms = new MemoryStream(data))
            {
                var info = reader.ReadTypeface(ms, "font" + index);
                Assert.IsNotNull(info, "Could not read the data for font " + index);
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), info.ErrorMessage);
                Assert.AreEqual(DataFormat.TTF, info.SourceFormat, "Font " + index);
                Assert.AreEqual(1, info.FontCount, "Font " + index);

                var single = reader.GetFont(ms, "font" + index, info.Fonts[0]);
                Assert.IsNotNull(single, "Font " + index);
                Assert.AreEqual(face.FamilyName, single.FamilyName, "Font " + index);
                Assert.AreEqual(face.FontWeight, single.FontWeight, "Font " + index);
                Assert.AreEqual(face.Selections, single.Selections, "Font " + index);
            }
        }

        private static long ReadUInt32(byte[] data, int offset)
        {
            return ((long)data[offset] << 24) | ((long)data[offset + 1] << 16) | ((long)data[offset + 2] << 8) | data[offset + 3];
        }
    }
}
