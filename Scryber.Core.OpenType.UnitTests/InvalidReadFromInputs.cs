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
    public class InvalidReadFromInputs
    {
        public const string RootUrl = ValidateHelvetica.RootUrl;

        public const string UrlPath = ValidateHelvetica.UrlPath;
        public const string FailingUrlPath = "/NOT_HERE/" + UrlPath;

        public static readonly string PartialFilePath = UrlPath;
        public static readonly string FailingPartialFilePath = FailingUrlPath;

        /// <summary>
        /// A download of a text (csproj) file that can make sure an http client is still alive
        /// </summary>
        public const string CheckAliveUrl = "https://raw.githubusercontent.com/richard-scryber/scryber.core.opentype/master/Scryber.Core.OpenType/Scryber.Core.OpenType.csproj";


        [TestMethod("1. Fail load from a file stream")]
        public void FailLoadFromFileStream()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, FailingPartialFilePath);

                Assert.ThrowsException<DirectoryNotFoundException>(() =>
                {
                    using (var stream = new FileStream(path, FileMode.Open))
                    {

                        var info = reader.ReadTypeface(stream, path);

                    }
                });
            }

        }

        [TestMethod("2. Fail load Info from a file path")]
        public void FailLoadInfoFromFilePath()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, FailingPartialFilePath);

                Assert.ThrowsException<FileNotFoundException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });

            }

        }

        [TestMethod("3. Fail load Info from a file info")]
        public void FailLoadInfoFromFileInfo()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, FailingPartialFilePath);
                var fi = new FileInfo(path);

                Assert.ThrowsException<FileNotFoundException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });

            }

        }

        [TestMethod("4. Fail load Info from a base directory and path")]
        public void FailLoadInfoFromDirectoryAndPath()
        {
            var path = System.Environment.CurrentDirectory;
            var di = new System.IO.DirectoryInfo(path);

            using (var reader = new TypefaceReader(di))
            {
                path = FailingPartialFilePath;

                Assert.ThrowsException<FileNotFoundException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });

            }

        }


        [TestMethod("5. Fail load from an absolute Url")]
        public void FailLoadInfoFromFullUrl()
        {
            var path = RootUrl;

            using (var reader = new TypefaceReader())
            {
                path = path + FailingUrlPath;
#if NET48

                Assert.ThrowsException<System.Net.WebException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });

#else 
                Assert.ThrowsException<AggregateException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });
#endif

            }

        }

        [TestMethod("6. Fail load from a base and partial Url")]
        public void FailLoadInfoFromPartialUrl()
        {
            var path = RootUrl;
            TypefaceReader reader;
            StreamLoader loader;

            using (reader = new TypefaceReader(new Uri(path)))
            {
                path = FailingUrlPath;
#if NET48

                Assert.ThrowsException<System.Net.WebException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });

#else 
                Assert.ThrowsException<AggregateException>(() =>
                {
                    var info = reader.ReadTypeface(path);
                });
#endif
                loader = reader.Loader;
            }
            //check clean up
            Assert.IsNull(reader.Loader, "The readers loader should have been set to null");
            Assert.IsNull(loader.Client, "The loaders http should have been disposed and set to null");
        }

        [TestMethod("7. Fail load Info from base + partial + http")]
        public void FailLoadInfoFromPartialUrlWithHttp()
        {
#if NET48
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#else
            var path = RootUrl;
            TypefaceReader reader;
            StreamLoader loader;
            using (var http = new System.Net.Http.HttpClient())
            {
                using (reader = new TypefaceReader(new Uri(path), http))
                {
                    Assert.IsNotNull(reader.Loader.Client, "The loader SHOULD have a client as it was provided");

                    path = FailingUrlPath;

                    Assert.ThrowsException<AggregateException>(() =>
                    {
                        var info = reader.ReadTypeface(path);
                    });

                    //check http is set
                    Assert.IsNotNull(reader.Loader.Client, "The loader should STILL have a client as it was provided");
                    Assert.IsFalse(reader.Loader.OwnsClient, "The loader should NOT own the client as it was  provided");

                    loader = reader.Loader;
                }
                //check clean up
                Assert.IsNull(reader.Loader, "The readers loader should have been set to null");
                Assert.IsNull(loader.Client, "The loaders http should have been set to null, but not disposed");

                //Simple check to make sure we are still able to use the http client
                var data = http.GetStringAsync(CheckAliveUrl).Result;
                Assert.IsNotNull(data);
                Assert.IsTrue(data.StartsWith("<Project "));

            }

#endif

        }


        [TestMethod("8. Fail load with just http client")]
        public void FailLoadInfoWithHttp()
        {
#if NET48
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#else
            var path = RootUrl;
            TypefaceReader reader;
            StreamLoader loader;
            using (var http = new System.Net.Http.HttpClient())
            {
                using (reader = new TypefaceReader(http))
                {
                    Assert.IsNotNull(reader.Loader.Client, "The loader SHOULD have a client as it was provided");

                    path += FailingUrlPath;

                    Assert.ThrowsException<AggregateException>(() =>
                    {
                        var info = reader.ReadTypeface(path);

                    });

                    //check http is set
                    Assert.IsNotNull(reader.Loader.Client, "The loader should STILL have a client as it was provided");
                    Assert.IsFalse(reader.Loader.OwnsClient, "The loader should NOT own the client as it was  provided");

                    loader = reader.Loader;
                }
                //check clean up
                Assert.IsNull(reader.Loader, "The readers loader should have been set to null");
                Assert.IsNull(loader.Client, "The loaders http should have been set to null, but not disposed");

                //Simple check to make sure we are still able to use the http client
                var data = http.GetStringAsync(CheckAliveUrl).Result;
                Assert.IsNotNull(data);
                Assert.IsTrue(data.StartsWith("<Project "));

            }

#endif

        }

    }
}
