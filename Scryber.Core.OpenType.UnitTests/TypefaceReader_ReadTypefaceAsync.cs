using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass()]
    public class TypefaceReader_ReadTypefaceAsync
    {

        public const string RootUrl = ValidateHelvetica.RootUrl;

        public const string UrlPath = ValidateHelvetica.UrlPath;
        public const string FailingUrlPath = "/NOT_HERE/" + UrlPath;

        public static readonly string PartialFilePath = UrlPath;
        public static readonly string FailingPartialFilePath = FailingUrlPath;

        

        [TestMethod("1. Async load from valid absolute url")]
        public async Task AsyncLoadFromAbsoluteUrl()
        {

            ITypefaceInfo info;

            using (var reader = new TypefaceReader())
            {
                var path = RootUrl;

                //valid path
                path = path + UrlPath;
                var uri = new Uri(path);

                info = await reader.ReadTypefaceAsync(uri);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info
                ValidateHelvetica.AssertInfo(info, path, 5);

            }

        }

        [TestMethod("2. Async to load from valid relative url")]
        public async Task AsyncLoadFromRelativeUrl()
        {

            ITypefaceInfo info;

            var path = RootUrl;

            using (var reader = new TypefaceReader(new Uri(path)))
            {
                //valid path
                path = UrlPath;
                var uri = new Uri(path, UriKind.Relative);

                info = await reader.ReadTypefaceAsync(uri);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path
                ValidateHelvetica.AssertInfo(info, null, 6);

            }

        }

        
        [TestMethod("3. Async load from valid absolute file path")]
        public async Task AsyncLoadFromAbsoluteFile()
        {

            ITypefaceInfo info;

            var path = System.Environment.CurrentDirectory;

            using (var reader = new TypefaceReader())
            {
                //valid path
                path = Path.Combine(path, PartialFilePath);
                var file = new FileInfo(path);

                info = await reader.ReadTypefaceAsync(file);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path, as this will be changed
                ValidateHelvetica.AssertInfo(info, null, 7);

            }

        }

        [TestMethod("4. Async load from valid relative file path")]
        public async Task AsyncLoadFromRelativeFile()
        {

            ITypefaceInfo info;

            var path = System.Environment.CurrentDirectory;

            using (var reader = new TypefaceReader(new DirectoryInfo(path)))
            {
                //valid path
                path = PartialFilePath;
                var file = new FileInfo(path);

                info = await reader.ReadTypefaceAsync(file);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path, as this will be changed
                ValidateHelvetica.AssertInfo(info, null, 7);

            }

        }

        [TestMethod("5. Async load from valid relative file string")]
        public async Task AsyncLoadFromRelativeFileString()
        {

            ITypefaceInfo info;

            var path = System.Environment.CurrentDirectory;

            using (var reader = new TypefaceReader(new DirectoryInfo(path)))
            {
                //valid path
                path = PartialFilePath;
                

                info = await reader.ReadTypefaceAsync(path);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path, as this will be changed
                ValidateHelvetica.AssertInfo(info, null, 7);

            }

        }

        [TestMethod("6. Async load from valid relative url string")]
        public async Task AsyncLoadFromRelativeUrlString()
        {

            ITypefaceInfo info;

            var path = RootUrl;

            using (var reader = new TypefaceReader(new Uri(path)))
            {
                //valid path
                path = UrlPath;

                info = await reader.ReadTypefaceAsync(path);

                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path
                ValidateHelvetica.AssertInfo(info, null, 6);

            }

        }
    }
}
