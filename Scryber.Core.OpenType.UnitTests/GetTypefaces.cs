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
    }
}
