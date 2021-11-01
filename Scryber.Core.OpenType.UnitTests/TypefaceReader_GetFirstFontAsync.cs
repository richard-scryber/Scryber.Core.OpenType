using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class TypefaceReader_GetFirstFontAsync
    {

        [TestMethod("1. Get the first Helvetica regular font from a file path")]
        public async Task ValidGetFirstFontHelveticaPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var face = await reader.GetFirstFontAsync(file);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("2. Get the first Helvetica font from a url")]
        public async Task ValidGetFirstFontHelveticaUrl()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var face = await reader.GetFirstFontAsync(url);


                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("3. Get first Gill sans font in the collection from a file path")]
        public async Task ValidGetFirstFontGillSansPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var url = new FileInfo(ValidateGillSans.UrlPath);

                var face = await reader.GetFirstFontAsync(url);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[0], face);
            }
        }

        [TestMethod("4. Get the first Gill Sans font in collection from a url")]
        public async Task ValidGetFirstFontGillSansUrl()
        {
            var path = new Uri(ValidateGillSans.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateGillSans.UrlPath, UriKind.Relative);

                var face = await reader.GetFirstFontAsync(url);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[0], face);
            }
        }

        [TestMethod("5. Get the  first Hachi Maru Pop font from a file")]
        public async Task ValidGetTypefacesHachiFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHachi.UrlPath);

                var face = await reader.GetFirstFontAsync(file);
                Assert.IsNotNull(face);

                ValidateHachi.AssertTypeface(face);
            }
        }

        [TestMethod("6. Get the  Noto TC open type from a file")]
        public async Task ValidGetTypefacesNotoFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateNoto.UrlPath);

                var face = await reader.GetFirstFontAsync(file);
                Assert.IsNotNull(face);


                ValidateNoto.AssertTypeface(face);
            }
        }



        [TestMethod("7. Get the Open Sans Black woff from a file")]
        public async Task ValidGetTypefacesOpenSansFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateOpenSans.UrlPath);

                var face = await reader.GetFirstFontAsync(file);
                Assert.IsNotNull(face);


                ValidateOpenSans.AssertTypeface(face);
            }
        }


        [TestMethod("8. Festive Woff2 typeface NOT SUPPORTED")]
        public async Task ValidGetTypefacesFestiveFile()
        {
            //var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            //using (var reader = new TypefaceReader(path))
            //{
            //    var file = new FileInfo(ValidateFestive.UrlPath);

            //    //We are not currently supported in Woff2

            //    await Assert.ThrowsExceptionAsync<TypefaceReadException>(async () =>
            //    {
            //        var faces = await reader.GetFirstFontAsync(file);
            //    });


            //}

            Assert.Inconclusive("The Woff2 format needs to be implemented in the OpenType library");
        }

    }

}
