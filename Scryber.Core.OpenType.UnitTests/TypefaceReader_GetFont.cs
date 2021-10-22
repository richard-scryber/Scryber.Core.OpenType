using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class TypefaceReader_GetFont
    {

        [TestMethod("1. Helvetica Regular font from a file path and font index")]
        public void ValidGetFontHelveticaPathAndIndex()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var face = reader.GetFont(file, 0);
                Assert.IsNotNull(face);

                
                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("2. Helvetica Regular font from a url and font index")]
        public void ValidGetFontHelveticaUrlAndIndex()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var face = reader.GetFont(url, 0);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("3. Helvetica Regular font from info file and font index")]
        public void ValidGetFontHelveticaFileInfoAndIndex()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var info = reader.ReadTypeface(file);

                //Get the font with the info and an index of 0
                var face = reader.GetFont(info, 0);

                Assert.IsNotNull(face);


                ValidateHelvetica.AssertTypeface(face);
            }
        }



        [TestMethod("4. Helvetica Regular font from a info url and font index")]
        public void ValidGetFontHelveticaUrlInfoAndIndex()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var file = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var info = reader.ReadTypeface(file);

                var face = reader.GetFont(info, 0);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        

        [TestMethod("5. Gill sans Bold font from a file path to the collection")]
        public void ValidGetFontGillSansBold()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.BoldRegularIndex;

                var face = reader.GetFont(file, index);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

        [TestMethod("6. Gill sans SemiBoldItalic font from a file path to the collection")]
        public void ValidGetFontGillSansSemiBoldItalic()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.SemiBoldItalicIndex;

                var face = reader.GetFont(file, index);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

        [TestMethod("7. Gill sans SemiBoldItalic font from info and reference")]
        public void ValidGetFontGillSansBoldInfoAndRef()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.BoldRegularIndex;

                var info = reader.ReadTypeface(file);
                var fref = info.Fonts[index];

                //Get the font with the info and the font reference
                var face = reader.GetFont(info, fref);

                Assert.IsNotNull(face);

                //Make sure the style matches
                var expected = ValidateGillSans.FontTypefaces[index];
                ValidateGillSans.AssertMatches(expected, face);
            }
        }

        [TestMethod("8. Helvetica Regular font from a url and reference")]
        public void ValidGetFontHelveticaUrlAndReference()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var uri = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var info = reader.ReadTypeface(uri);
                var fref = info.Fonts[0];

                //Load from a known url and a font reference
                var face = reader.GetFont(uri, fref);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }


        [TestMethod("9. Gill sans SemiBoldItalic font from a file path and reference")]
        public void ValidGetFontGillSansSemiBoldItalicFileInfoAndReference()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.SemiBoldItalicIndex;

                var info = reader.ReadTypeface(file);
                var fref = info.Fonts[index];

                //Load from a known file and a font reference
                var face = reader.GetFont(file, fref);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

    }
}
