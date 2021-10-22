using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scryber.OpenType.Utility;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// Reads teh ITypeFaceInformation for a single ttf file from the various different sources
    /// </summary>
    [TestClass()]
    public class TypefaceReader_ReadAllTypefacesAsync
    {
        
        /// <summary>
        /// A download of a text (csproj) file that can make sure an http client is still alive
        /// </summary>
        public const string CheckAliveUrl = "https://raw.githubusercontent.com/richard-scryber/scryber.core.opentype/master/Scryber.Core.OpenType/Scryber.Core.OpenType.csproj";

        

        [TestMethod("1. Async Read all typefaces from a directory")]
        public async Task LoadAllFromDirectory()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                var all = await reader.ReadAllTypefacesAsync(dir);

                Assert.AreEqual(6, all.Length);
            }

        }

        [TestMethod("2. Async Read all typefaces from a directory with single match")]
        public async Task LoadAllFromDirectoryWithMatch()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                var match = "*.ttf";

                var all = await reader.ReadAllTypefacesAsync(dir, match);

                //only 3 fonts match the pattern
                int expected = 3;
                Assert.AreEqual(expected, all.Length);
            }

        }

        [TestMethod("3. Async Read all typefaces from a directory with multiple matches")]
        public async Task LoadAllFromDirectoryWithMultipleMatch()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                //use the pipe separator
                var match = "*.ttf| *.otf";

                var all = await reader.ReadAllTypefacesAsync(dir, match);

                //4 fonts match the pattern(s)
                int expected = 4;
                Assert.AreEqual(expected, all.Length);
            }

        }

        [TestMethod("4. Async Read all typefaces from sub directories with multiple matches")]
        public async Task LoadAllFromDirectoryWithMultipleMatchAndSubdirectories()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                var match = "*.ttf| *.otf";
                var includeSubs = true;

                var all = await reader.ReadAllTypefacesAsync(dir, match, includeSubs);

                //4 fonts match the pattern(s) in the root
                //+1 in the subfolder
                int expected = 5;
                Assert.AreEqual(expected, all.Length);
            }

        }


        [TestMethod("5. Async fail read all typefaces from sub directories")]
        public async Task LoadAllFromDirectoryAndSubdirectories()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                var match = ""; //should ignore null or empty strings
                var includeSubs = true;

                await Assert.ThrowsExceptionAsync<TypefaceReadException>(async () =>
                {
                    //Becase we have an unsupported woff2 in here and we are not capturing errors then we should fail.
                    var all = await reader.ReadAllTypefacesAsync(dir, match, includeSubs);

                });
                
            }

        }

        [TestMethod("6. Async Read all typefaces from sub directories, capture errors")]
        public async Task LoadAllFromDirectoryAndSubdirectoriesWithErrors()
        {
            using (var reader = new TypefaceReader())
            {
                var path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, ValidateHelvetica.UrlPath);

                //Get the parent directory
                var file = new FileInfo(path);
                var dir = file.Directory;

                var match = ""; //should ignore null or empty strings
                var includeSubs = true;
                var captureErrors = true;

                var all = await reader.ReadAllTypefacesAsync(dir, match, includeSubs, captureErrors);

                //6 fonts match the pattern(s) in the root
                //+1 in the subfolder
                //+1 unsupported
                int expected = 8;
                Assert.AreEqual(expected, all.Length);

                var error = "unsupported/Festive.woff2";

                var found = false;

                foreach (var info in all)
                {
                    if(!string.IsNullOrEmpty(info.ErrorMessage))
                    {
                        Assert.IsTrue(info.Source.EndsWith(error));
                        Assert.IsFalse(found, "We should be the one and only error");
                        found = true;
                    }
                }

            }

        }


        [TestMethod("7. Async Read all typefaces from paths")]
        public async Task LoadAllForPaths()
        {
            var parent = new DirectoryInfo(System.Environment.CurrentDirectory);
            using (var reader = new TypefaceReader(parent))
            {

                List<FileInfo> paths = new List<FileInfo>();
                paths.Add(new FileInfo(ValidateHelvetica.UrlPath));
                paths.Add(new FileInfo(ValidateOpenSans.UrlPath));
                paths.Add(new FileInfo(System.Environment.CurrentDirectory + "/fonts/subfolder/Roboto.ttf"));

                var all = await reader.ReadAllTypefacesAsync(paths);

                //4 fonts match the pattern(s) in the root
                //+1 in the subfolder
                int expected = 3;
                Assert.AreEqual(expected, all.Length);
            }

        }

        [TestMethod("8. Async Read all typefaces from uri's")]
        public async Task LoadAllFroUris()
        {
            var uri = new Uri(ValidateHelvetica.RootUrl);
            using (var reader = new TypefaceReader(uri))
            {

                List<Uri> uris = new List<Uri>();
                uris.Add(new Uri(ValidateHelvetica.UrlPath, UriKind.Relative));
                uris.Add(new Uri(ValidateOpenSans.UrlPath, UriKind.Relative));
                uris.Add(new Uri(ValidateRoboto.RootUrl + "fonts/subfolder/Roboto.ttf", UriKind.Absolute));

                var all = await reader.ReadAllTypefacesAsync(uris);

                //4 fonts match the pattern(s) in the root
                //+1 in the subfolder
                int expected = 3;
                Assert.AreEqual(expected, all.Length);
            }

        }


    }
}
