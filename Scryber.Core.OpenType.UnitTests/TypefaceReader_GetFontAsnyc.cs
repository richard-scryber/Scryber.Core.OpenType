using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class TypefaceReader_GetFontAsnyc
    {

        [TestMethod("1. Async Helvetica Regular font from a file path and font index")]
        public async Task ValidGetFontHelveticaPathAndIndex()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var face = await reader.GetFontAsync(file, 0);
                Assert.IsNotNull(face);

                
                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("2. Helvetica Regular font from a url and font index")]
        public async Task ValidGetFontHelveticaUrlAndIndex()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var face = await reader.GetFontAsync(url, 0);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("3. Helvetica Regular font from info file and font index")]
        public async Task ValidGetFontHelveticaFileInfoAndIndex()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var info = await reader.ReadTypefaceAsync(file);

                //Get the font with the info and an index of 0
                var face = await reader.GetFontAsync(info, 0);

                Assert.IsNotNull(face);


                ValidateHelvetica.AssertTypeface(face);
            }
        }



        [TestMethod("4. Helvetica Regular font from a info url and font index")]
        public async Task ValidGetFontHelveticaUrlInfoAndIndex()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var file = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var info = await reader.ReadTypefaceAsync(file);

                var face = await reader.GetFontAsync(info, 0);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        

        [TestMethod("5. Gill sans Bold font from a file path to the collection")]
        public async Task ValidGetFontGillSansBold()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.BoldRegularIndex;

                var face = await reader.GetFontAsync(file, index);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

        [TestMethod("6. Gill sans SemiBoldItalic font from a file path to the collection")]
        public async Task ValidGetFontGillSansSemiBoldItalic()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.SemiBoldItalicIndex;

                var face = await reader.GetFontAsync(file, index);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

        [TestMethod("7. Gill sans SemiBoldItalic font from info and reference")]
        public async Task ValidGetFontGillSansBoldInfoAndRef()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.BoldRegularIndex;

                var info = await reader.ReadTypefaceAsync(file);
                var fref = info.Fonts[index];

                //Get the font with the info and the font reference
                var face = await reader.GetFontAsync(info, fref);

                Assert.IsNotNull(face);

                //Make sure the style matches
                var expected = ValidateGillSans.FontTypefaces[index];
                ValidateGillSans.AssertMatches(expected, face);
            }
        }

        [TestMethod("8. Helvetica Regular font from a url and reference")]
        public async Task ValidGetFontHelveticaUrlAndReference()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var uri = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var info = await reader.ReadTypefaceAsync(uri);
                var fref = info.Fonts[0];

                //Load from a known url and a font reference
                var face = await reader.GetFontAsync(uri, fref);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }


        [TestMethod("9. Gill sans SemiBoldItalic font from a file path and reference")]
        public async Task ValidGetFontGillSansSemiBoldItalicFileInfoAndReference()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);
                var index = ValidateGillSans.SemiBoldItalicIndex;

                var info = await reader.ReadTypefaceAsync(file);
                var fref = info.Fonts[index];

                //Load from a known file and a font reference
                var face = await reader.GetFontAsync(file, fref);
                Assert.IsNotNull(face);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[index], face);
            }
        }

    }
}
