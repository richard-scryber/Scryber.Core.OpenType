using System;
using System.IO;
using System.Net.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.OpenType.Utility;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// Reads teh ITypeFaceInformation for a single ttf file from the various different sources
    /// </summary>
    [TestClass()]
    public class TypefaceReader_ReadTypeface
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

        

        [TestMethod("1. Read Typeface Info from a file stream")]
        public void LoadInfoFromFileStream()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, PartialFilePath);

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var info = reader.ReadTypeface(stream, path);
                    ValidateHelvetica.AssertInfo(info, path, 1);
                }
            }

        }

        [TestMethod("2. Read Typeface Info from a file path")]
        public void LoadInfoFromFilePath()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, PartialFilePath);

                var info = reader.ReadTypeface(path);
                ValidateHelvetica.AssertInfo(info, path, 2);
                
            }

        }

        [TestMethod("3. Read Typeface Info from a file info")]
        public void LoadInfoFromFileInfo()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, PartialFilePath);
                var fi = new FileInfo(path);

                var info = reader.ReadTypeface(path);
                ValidateHelvetica.AssertInfo(info, path, 3);

            }

        }

        [TestMethod("4. Read Typeface Info from a base directory and path")]
        public void LoadInfoFromDirectoryAndPath()
        {
            var path = System.Environment.CurrentDirectory;
            var di = new System.IO.DirectoryInfo(path);

            using (var reader = new TypefaceReader(di))
            {
                path = PartialFilePath;
                var info = reader.ReadTypeface(path);
                ValidateHelvetica.AssertInfo(info, path, 4);

            }

        }


        [TestMethod("5. Read Typeface Info from an absolute Url")]
        public void LoadInfoFromFullUrl()
        {
            var path = RootUrl;
            
            using (var reader = new TypefaceReader())
            {
                path = path + UrlPath;
                var info = reader.ReadTypeface(path);
                ValidateHelvetica.AssertInfo(info, path, 5);

            }

        }

        [TestMethod("6. Read Typeface Info from a base and partial Url")]
        public void LoadInfoFromPartialUrl()
        {
            var path = RootUrl;
            TypefaceReader reader;
            StreamLoader loader;

            using (reader = new TypefaceReader(new Uri(path)))
            {
                path = UrlPath;
                var info = reader.ReadTypeface(path);
                ValidateHelvetica.AssertInfo(info, path, 6);

                //check http is set
                Assert.IsNotNull(reader.Loader.Client, "The loader should have a client as it was not provided, but needed");
                Assert.IsTrue(reader.Loader.OwnsClient, "The loader should own the client as it was not provided");

                loader = reader.Loader;
            }
            //check clean up
            Assert.IsNull(reader.Loader, "The readers loader should have been set to null");
            Assert.IsNull(loader.Client, "The loaders http should have been disposed and set to null");
        }



        [TestMethod("7. Read Typeface Info from base + partial + http")]
        public void LoadInfoFromPartialUrlWithHttp()
        {
            var path = RootUrl;
            TypefaceReader reader;
            StreamLoader loader;
            using (var http = new System.Net.Http.HttpClient())
            {
                using (reader = new TypefaceReader(new Uri(path), http))
                {
                    Assert.IsNotNull(reader.Loader.Client, "The loader SHOULD have a client as it was provided");

                    path = UrlPath;
                    var info = reader.ReadTypeface(path);
                    ValidateHelvetica.AssertInfo(info, path, 7);

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


        }


        [TestMethod("8. Read Typeface Info with just http client")]
        public void LoadInfoWithHttp()
        {
            TypefaceReader reader;
            StreamLoader loader;
            using (var http = new System.Net.Http.HttpClient())
            {
                using (reader = new TypefaceReader(http))
                {
                    Assert.IsNotNull(reader.Loader.Client, "The loader SHOULD have a client as it was provided");

                    var path = RootUrl;
                    path = path + UrlPath;
                    var info = reader.ReadTypeface(path);
                    ValidateHelvetica.AssertInfo(info, path, 8);

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

        }

    }
}
