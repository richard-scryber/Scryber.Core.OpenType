using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    [TestCategory("Get First Font")]
    public class TypefaceReader_GetFirstFont
    {

        [TestMethod("1. Get the first Helvetica regular font from a file path")]
        public void ValidGetFirstFontHelveticaPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var face = reader.GetFirstFont(file);
                Assert.IsNotNull(face);

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("2. Get the first Helvetica font from a url")]
        public void ValidGetFirstFontHelveticaUrl()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var face = reader.GetFirstFont(url);
                

                ValidateHelvetica.AssertTypeface(face);
            }
        }

        [TestMethod("3. Get first Gill sans font in the collection from a file path")]
        public void ValidGetFirstFontGillSansPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var url = new FileInfo(ValidateGillSans.UrlPath);

                var face = reader.GetFirstFont(url);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[0], face);
            }
        }

        [TestMethod("4. Get the first Gill Sans font in collection from a url")]
        public void ValidGetFirstFontGillSansUrl()
        {
            var path = new Uri(ValidateGillSans.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var url = new Uri(ValidateGillSans.UrlPath, UriKind.Relative);

                var face = reader.GetFirstFont(url);

                ValidateGillSans.AssertMatches(ValidateGillSans.FontTypefaces[0], face);
            }
        }

        [TestMethod("5. Get the  first Hachi Maru Pop font from a file")]
        public void ValidGetTypefacesHachiFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHachi.UrlPath);

                var face = reader.GetFirstFont(file);
                Assert.IsNotNull(face);

                ValidateHachi.AssertTypeface(face);
            }
        }

        [TestMethod("6. Get the  Noto TC open type from a file")]
        public void ValidGetTypefacesNotoFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateNoto.UrlPath);

                var face = reader.GetFirstFont(file);
                Assert.IsNotNull(face);

                
                ValidateNoto.AssertTypeface(face);
            }
        }

        

        [TestMethod("7. Get the Open Sans Black woff from a file")]
        public void ValidGetTypefacesOpenSansFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateOpenSans.UrlPath);

                var face = reader.GetFirstFont(file);
                Assert.IsNotNull(face);


                ValidateOpenSans.AssertTypeface(face);
            }
        }

        [TestMethod("8. Festive Woff2 typeface NOT SUPPORTED")]
        public void ValidGetTypefacesFestiveFile()
        {
            // var path = new DirectoryInfo(System.Environment.CurrentDirectory);
             
            // using (var reader = new TypefaceReader(path))
            // {
            //     var file = new FileInfo(ValidateFestive.UrlPath);

            //     //We are not currently supported in Woff2

            //     Assert.ThrowsException<TypefaceReadException>(() =>
            //     {
            //         var faces = reader.GetFirstFont(file);
            //     });
                
                
            // }

            Assert.Inconclusive("The Woff2 format needs to be implemented in the OpenType library");
        }

        
    }

}
