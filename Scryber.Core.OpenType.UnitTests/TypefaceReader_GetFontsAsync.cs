using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class TypefaceReader_GetFontsAsync
    {

        [TestMethod("1. Async Gill Sans fonts from a ITypefaceInfo")]
        public async Task GetFontsAsyncFromInfo()
        {
            ITypefaceInfo gill;

            var path = new DirectoryInfo(System.Environment.CurrentDirectory);
            var file = new FileInfo(System.IO.Path.Combine(path.FullName, ValidateGillSans.UrlPath));

            using (var reader = new TypefaceReader())
            {
                gill = reader.ReadTypeface(file);

            }

            //Clean reader with a typeface info

            using (var reader = new TypefaceReader())
            {
                var faces = await reader.GetFontsAsync(gill);

                ValidateGillSans.AssertTypefaces(faces.ToArray());
            }
        }


        [TestMethod("2. Async Gill Sans fonts from just a url")]
        public async Task GetFontsAsyncFromUri()
        {

            var path = new Uri(ValidateGillSans.RootUrl + ValidateGillSans.UrlPath);

            //Clean reader with a file stream

            using (var reader = new TypefaceReader())
            {
                var faces = await reader.GetFontsAsync(path);

                ValidateGillSans.AssertTypefaces(faces.ToArray());

            }
        }

        [TestMethod("3. Async Gill Sans fonts from just a file")]
        public async Task GetFontsAsyncFromFile()
        {
            var path = new DirectoryInfo(System.Environment.CurrentDirectory);
            var file = new FileInfo(System.IO.Path.Combine(path.FullName, ValidateGillSans.UrlPath));


            using (var reader = new TypefaceReader())
            {

                var faces = await reader.GetFontsAsync(file);

                ValidateGillSans.AssertTypefaces(faces.ToArray());

            }
        }

        [TestMethod("4. Async Gill Sans fonts from a relative url")]
        public async Task GetFontsAsyncFromRelativeUrl()
        {
            var path = new Uri(ValidateGillSans.RootUrl);

            //Set the base url

            using (var reader = new TypefaceReader(path))
            {
                //Set the relative path
                path = new Uri(ValidateGillSans.UrlPath, UriKind.Relative);

                var faces = await reader.GetFontsAsync(path);

                ValidateGillSans.AssertTypefaces(faces.ToArray());

            }
        }

        [TestMethod("5. Failing async Helvetica font from an INVALID relative url")]
        public async Task FailGetFontsAsyncFromRelativeUrl()
        {
#if NET48
            Assert.Inconclusive("Cannot test for HttpRequestException with 4.8");
#else
            var path = new Uri(ValidateHelvetica.RootUrl);

            //Set the base url

            using (var reader = new TypefaceReader(path))
            {
                //Set the relative path
                path = new Uri("INVALID/" + ValidateHelvetica.UrlPath, UriKind.Relative);

                await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
                {
                    var faces = await reader.GetFontsAsync(path);

                    ValidateGillSans.AssertTypefaces(faces.ToArray());
                });
            }

#endif

        }

        [TestMethod("6. Failing Helvetica font from an INVALID relative file")]
        public void FailGetFontsFromRelativePath()
        {

            var path = new DirectoryInfo(System.Environment.CurrentDirectory);

            //Set the base url

            using (var reader = new TypefaceReader(path))
            {
                //Set the relative path
                var file = new FileInfo("INVALID/" + ValidateHelvetica.UrlPath);

                Assert.ThrowsException<TypefaceReadException>(() =>
                {
                    var faces = reader.GetFonts(file);

                    ValidateGillSans.AssertTypefaces(faces.ToArray());
                });
            }

        }

    }

}
