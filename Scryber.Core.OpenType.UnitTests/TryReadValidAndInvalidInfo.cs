using System;
using System.IO;
using System.Net.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.OpenType.Utility;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// Checks failures from various reads
    /// </summary>
    [TestClass()]
    public class TryReadValidAndInvalidInfo
    {
        public const string RootUrl = ValidateHelvetica.RootUrl;

        public const string UrlPath = ValidateHelvetica.UrlPath;
        public const string FailingUrlPath = "/NOT_HERE/" + UrlPath;

        public static readonly string PartialFilePath = UrlPath;
        public static readonly string FailingPartialFilePath = FailingUrlPath;

        

        


        [TestMethod("1. Try to load from a file stream")]
        public void TryLoadFromFileStream()
        {
            ITypefaceInfo info;
            bool result;

            using (var reader = new TypefaceReader())
            {
                // Success
                FileStream stream;

                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, PartialFilePath);

                using (stream = new FileStream(path, FileMode.Open))
                {
                    result = reader.TryReadTypeface(stream, path, out info);
                }

                Assert.IsTrue(result, "Could not read the valid input from a stream");
                Assert.IsNotNull(info, "The info was not returned for the try method");
                ValidateHelvetica.AssertInfo(info, path, 1);

            }

        }


        [TestMethod("2. Try and fail to load from invalid stream")]
        public void TryFailFromFileStream()
        {
            ITypefaceInfo info;
            bool result;

            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;

                //valid path and null stream
                path = Path.Combine(path, PartialFilePath);
                FileStream stream = null;

                result = reader.TryReadTypeface(stream, path, out info);

                Assert.IsFalse(result, "Reported as true, to read the INVALID input from a stream");
                Assert.IsNotNull(info, "Info was not returned even though we are trying");
                Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned");

            }
        }


        [TestMethod("3. Try to load from valid url stream")]
        public void TryLoadFromUrlStream()
        {

#if NET48
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#else
            ITypefaceInfo info;
            bool result;

            using (var reader = new TypefaceReader())
            {
                var path = RootUrl;

                //valid path
                path = path + UrlPath;

                using (var http = new HttpClient())
                {
                    //We need a seekable stream, so download to a buffer and use that.
                    var data = http.GetByteArrayAsync(path).Result;

                    using (var stream = new MemoryStream(data))
                        result = reader.TryReadTypeface(stream, path, out info);

                    Assert.IsTrue(result, "Reported as false, to read the input from a stream");
                    Assert.IsNotNull(info, "Info was not returned");
                    Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                    //check the info
                    ValidateHelvetica.AssertInfo(info, path, 3);
                }
            }

#endif
        }

        [TestMethod("4. Try and fail to load from a url stream")]
        public void TryFailLoadFromUrlStream()
        {

#if NET48
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#else
            ITypefaceInfo info;
            bool result;

            using (var reader = new TypefaceReader())
            {
                var path = RootUrl;

                //valid path
                path = path + UrlPath;

                using (var http = new HttpClient())
                {
                    //We need a seekable stream, this will fail.

                    using (var stream = http.GetStreamAsync(path).Result)
                        result = reader.TryReadTypeface(stream, path, out info);

                    Assert.IsFalse(result, "Reported as true, to read the input from a seekable stream");
                    Assert.IsNotNull(info, "Info was not returned");
                    Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned");
                    
                }
            }

#endif
        }


        [TestMethod("5. Try to load from valid absolute url")]
        public void TryLoadFromAbsoluteUrl()
        {

            ITypefaceInfo info;
            bool result;

            using (var reader = new TypefaceReader())
            {
                var path = RootUrl;

                //valid path
                path = path + UrlPath;
                var uri = new Uri(path);

                result = reader.TryReadTypeface(uri, out info);

                Assert.IsTrue(result, "Reported as false, to read the input from a stream");
                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info
                ValidateHelvetica.AssertInfo(info, path, 5);

            }

        }

        [TestMethod("6. Try to load from valid relative url")]
        public void TryLoadFromRelativeUrl()
        {

            ITypefaceInfo info;
            bool result;
            var path = RootUrl;

            using (var reader = new TypefaceReader(new Uri(path)))
            {
                //valid path
                path = UrlPath;
                var uri = new Uri(path, UriKind.Relative);

                result = reader.TryReadTypeface(uri, out info);

                Assert.IsTrue(result, "Reported as false, to read the input from a stream");
                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path
                ValidateHelvetica.AssertInfo(info, null, 6);

            }

        }

        [TestMethod("7. Try to load from valid relative file path")]
        public void TryLoadFromRelativeFile()
        {

            ITypefaceInfo info;
            bool result;
            var path = System.Environment.CurrentDirectory;

            using (var reader = new TypefaceReader(new DirectoryInfo(path)))
            {
                //valid path
                path = PartialFilePath;
                var file = new FileInfo(path);

                result = reader.TryReadTypeface(file, out info);

                Assert.IsTrue(result, "Reported as false, to read the input from a stream");
                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage), "An Error message was returned");
                //check the info - but not the path, as this will be changed
                ValidateHelvetica.AssertInfo(info, null, 7);

            }

        }

        [TestMethod("8. Try and fail to load from relative url")]
        public void TryFailLoadFromRelativeUrl()
        {

            ITypefaceInfo info;
            bool result;
            var path = RootUrl;

            using (var reader = new TypefaceReader(new Uri(path)))
            {
                //valid path
                path = FailingUrlPath;
                var uri = new Uri(path);

                result = reader.TryReadTypeface(uri, out info);

                Assert.IsFalse(result, "Reported as true, to read the input from an invalid stream");
                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned in the info for a fail load");


            }

        }

        [TestMethod("9. Try and fail to load from relative file path")]
        public void TryFailLoadFromRelativeFile()
        {

            ITypefaceInfo info;
            bool result;
            var path = System.Environment.CurrentDirectory;

            using (var reader = new TypefaceReader(new DirectoryInfo(path)))
            {
                //valid path
                path = FailingPartialFilePath;
                var file = new FileInfo(path);

                result = reader.TryReadTypeface(file, out info);

                Assert.IsFalse(result, "Reported as true, to read the input from an invalid stream");
                Assert.IsNotNull(info, "Info was not returned");
                Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned in the info for a fail load");


            }

        }
    }
}
