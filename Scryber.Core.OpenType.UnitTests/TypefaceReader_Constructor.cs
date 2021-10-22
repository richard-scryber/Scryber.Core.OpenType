using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.OpenType;

namespace Scryber.OpenType.UnitTests
{
    [TestClass]
    public class TypefaceReader_Constructor
    {
        [TestMethod("1. No Parameters is unrooted")]
        public void ParameterLess()
        {
            var reader = new TypefaceReader();
            Assert.IsNotNull(reader);
            Assert.IsNotNull(reader.Loader);
            Assert.IsInstanceOfType(reader.Loader, typeof(Utility.UnRootedStreamLoader), "The parameterless constructor should have an unrooted stream loader");
        }

        [TestMethod("2. Client only is unrooted")]
        public void ParameterHttpClient()
        {
#if !NET48

            Utility.UnRootedStreamLoader loader;

            using (var client = new System.Net.Http.HttpClient())
            {
                using (var reader = new TypefaceReader(client))
                {

                    Assert.IsNotNull(reader);
                    Assert.IsNotNull(reader.Loader);
                    Assert.IsInstanceOfType(reader.Loader, typeof(Utility.UnRootedStreamLoader), "The uri constructor should have an rooted uri stream loader");
                    loader = reader.Loader as Utility.UnRootedStreamLoader;
                    Assert.IsNotNull(loader);
                    Assert.IsNotNull(loader.Client, "The client was not set");
                    Assert.AreSame(loader.Client, client);
                    Assert.IsFalse(loader.OwnsClient, "The loader should not own the http client");
                }

                //reader is disposed and should be null for the loader.
                Assert.IsNull(loader.Client); 
            }

#else
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#endif

        }


        [TestMethod("3. Uri Parameter has a root url")]
        public void ParameterUri()
        {
            var root = new Uri("https://fonts.gstatic.com/s/");
            var reader = new TypefaceReader(root);
            Assert.IsNotNull(reader);

            Assert.IsNotNull(reader.Loader);
            Assert.IsInstanceOfType(reader.Loader, typeof(Utility.RootedUriStreamLoader), "The uri constructor should have an rooted uri stream loader");
            var loader = reader.Loader as Utility.RootedUriStreamLoader;
            Assert.IsNotNull(loader);
            Assert.IsNull(loader.Client, "An unused loader should not have an existing client");
            Assert.AreEqual(root, loader.BaseUri, "The base uri's do not match");

        }

        

        [TestMethod("4. Uri Parameter and client")]
        public void ParameterUriClient()
        {
#if !NET48

            var root = new Uri("https://fonts.gstatic.com/s/");
            Utility.RootedUriStreamLoader loader;

            using (var client = new System.Net.Http.HttpClient())
            {
                using (var reader = new TypefaceReader(root, client))
                {

                    Assert.IsNotNull(reader);
                    Assert.IsNotNull(reader.Loader);
                    Assert.IsInstanceOfType(reader.Loader, typeof(Utility.RootedUriStreamLoader), "The uri constructor should have an rooted uri stream loader");

                    loader = reader.Loader as Utility.RootedUriStreamLoader;
                    Assert.IsNotNull(loader);
                    Assert.IsNotNull(loader.Client, "The client was not set");
                    Assert.AreSame(loader.Client, client);
                    Assert.IsFalse(loader.OwnsClient, "The loader should not own the http client");

                    Assert.AreEqual(root, loader.BaseUri, "The base uri's do not match");
                }
                //This should be removed if the reader is disposed.
                Assert.IsNull(loader.Client);
            }

#else
            Assert.Inconclusive("Cannot test this in .Net 4.8");
#endif

        }

        [TestMethod("5. Uri Parameter and .net 4.8 WebClient with a root url")]
        public void ParameterUriWebClient_NET48()
        {
#if NET48
            var root = new Uri("https://fonts.gstatic.com/s/");
            Utility.RootedUriStreamLoader loader;

            using (var client = new System.Net.WebClient())
            {
                using (var reader = new TypefaceReader(root, client))
                {

                    Assert.IsNotNull(reader);
                    Assert.IsNotNull(reader.Loader);
                    Assert.IsInstanceOfType(reader.Loader, typeof(Utility.RootedUriStreamLoader), "The uri constructor should have an rooted uri stream loader");

                    loader = reader.Loader as Utility.RootedUriStreamLoader;
                    Assert.IsNotNull(loader);
                    Assert.IsNotNull(loader.Client, "The client was not set");
                    Assert.AreSame(loader.Client, client);
                    Assert.IsFalse(loader.OwnsClient, "The loader should not own the http client");

                    Assert.AreEqual(root, loader.BaseUri, "The base uri's do not match");
                }
                //This should be removed if the reader is disposed.
                Assert.IsNull(loader.Client);
            }

#else
            Assert.Inconclusive("Can only test this in .Net 4.8");
#endif
        }

        [TestMethod("6. Directory Parameter")]
        public void ParameterDirectory()
        {

            var root = new DirectoryInfo(System.Environment.CurrentDirectory);
            Utility.RootedFileStreamLoader loader;

            using (var reader = new TypefaceReader(root))
            {
                Assert.IsNotNull(reader);
                Assert.IsNotNull(reader.Loader);
                Assert.IsInstanceOfType(reader.Loader, typeof(Utility.RootedFileStreamLoader), "The directory constructor should have an rooted file stream loader");

                loader = reader.Loader as Utility.RootedFileStreamLoader;
                Assert.IsNotNull(loader);
                Assert.IsNull(loader.Client, "The client should be null");
                Assert.IsNotNull(loader.BaseDirectory, "The base directory was not set");
                Assert.AreEqual(root, loader.BaseDirectory, "The base directories do not match");
                Assert.AreEqual(root.FullName, loader.BaseDirectory.FullName, "The base directories do not match");
            }
            //This should be removed if the reader is disposed.
            Assert.IsNull(loader.Client);

        }

        [TestMethod("7. With a custom loader")]
        public void ParameterCustomLoader()
        {
            CustomLoader loader = new CustomLoader();
            using(var reader = new TypefaceReader(loader))
            {
                Assert.IsNotNull(reader);
                Assert.IsNotNull(reader.Loader);
                Assert.IsInstanceOfType(reader.Loader, typeof(CustomLoader), "The custom constructor should be set so it can be used");
            }
        }

        private class CustomLoader : Utility.StreamLoader
        {
            public override Stream GetStream(string path, bool ensureSeekable)
            {
                return null;
            }
        }
    }
}
