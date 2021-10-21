using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class GetTypefaces
    {

        [TestMethod("1. Get the Helvetica typeface from a file path")]
        public void ValidGetTypefacesHelveticaPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHelvetica.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateHelvetica.AssertTypeface(all[0]);
            }
        }

        [TestMethod("2. Get the Helvetica typeface from a url")]
        public void ValidGetTypefacesHelveticaUrl()
        {
            var path = new Uri(ValidateHelvetica.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var file = new Uri(ValidateHelvetica.UrlPath, UriKind.Relative);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateHelvetica.AssertTypeface(all[0]);
            }
        }

        [TestMethod("3. Get the Gill sans collection from a file path")]
        public void ValidGetTypefacesGillSansPath()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateGillSans.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(9, all.Length);

                ValidateGillSans.AssertTypefaces(all);
            }
        }

        [TestMethod("4. Get the Gill Sans collection from a url")]
        public void ValidGetTypefacesGillSansUrl()
        {
            var path = new Uri(ValidateGillSans.RootUrl);

            using (var reader = new TypefaceReader(path))
            {
                var file = new Uri(ValidateGillSans.UrlPath, UriKind.Relative);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(9, all.Length);

                ValidateGillSans.AssertTypefaces(all);
            }
        }

        [TestMethod("5. Get the  Hachi Maru Pop typeface from a file")]
        public void ValidGetTypefacesHachiFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateHachi.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateHachi.AssertTypeface(all[0]);
            }
        }

        [TestMethod("6. Get the  Noto TC open type from a file")]
        public void ValidGetTypefacesNotoFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateNoto.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateNoto.AssertTypeface(all[0]);
            }
        }

        [TestMethod("7. Get the  Roboto typeface from a file")]
        public void ValidGetTypefacesRobotoFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateRoboto.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateRoboto.AssertTypeface(all[0]);
            }
        }

        [TestMethod("8. Get the Open Sans Black woff from a file")]
        public void ValidGetTypefacesOpenSansFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateOpenSans.UrlPath);

                var faces = reader.GetTypefaces(file);
                Assert.IsNotNull(faces);

                var all = faces.ToArray();
                Assert.AreEqual(1, all.Length);

                ValidateOpenSans.AssertTypeface(all[0]);
            }
        }

        [TestMethod("9. Get the  Festive Woff2 typeface from a file")]
        public void ValidGetTypefacesFestiveFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            using (var reader = new TypefaceReader(path))
            {
                var file = new FileInfo(ValidateFestive.UrlPath);

                Assert.ThrowsException<NotSupportedException>(() =>
                {
                    var faces = reader.GetTypefaces(file);
                });
                
                
            }
        }
    }
}
