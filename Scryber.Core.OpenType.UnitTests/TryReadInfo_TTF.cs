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
    public class TryReadValidAndInvalid
    {
        public const string RootUrl = "https://raw.githubusercontent.com/richard-scryber/scryber.core/master/Scryber.Drawing/Text";

        public const string UrlPath = "/_FontResources/Helvetica/Helvetica.ttf";
        public const string FailingUrlPath = "/NOT_HERE/Helvetica/Helvetica.ttf";

        public static readonly string PartialFilePath = "fonts/Helvetica.ttf";
        public static readonly string FailingPartialFilePath = "/NOT_FOUND/fonts/Helvetica.ttf";

        /// <summary>
        /// A download of a text (csproj) file that can make sure an http client is still alive
        /// </summary>
        public const string CheckAliveUrl = "https://raw.githubusercontent.com/richard-scryber/scryber.core.opentype/master/Scryber.Core.OpenType/Scryber.Core.OpenType.csproj";


        


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
                    result = reader.TryGetTypefaceInformation(stream, path, out info);
                }

                Assert.IsTrue(result, "Could not read the valid input from a stream");
                Assert.IsNotNull(info, "The info was not returned for the try method");
                ValidateHelvetica.AssertInfo(info, path, 1);

                //And the fail for a disposed stream

                path = System.Environment.CurrentDirectory;
                path = Path.Combine(path, FailingPartialFilePath);

                result = reader.TryGetTypefaceInformation(stream, path, out info);
                
                Assert.IsFalse(result, "Reported as true, to read the INVALID input from a stream");
                Assert.IsNotNull(info, "Info was not returned even though we are trying");
                Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned");

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

                //failing path and stream
                path = Path.Combine(path, FailingPartialFilePath);
                FileStream stream = null;

                result = reader.TryGetTypefaceInformation(stream, path, out info);

                Assert.IsFalse(result, "Reported as true, to read the INVALID input from a stream");
                Assert.IsNotNull(info, "Info was not returned even though we are trying");
                Assert.IsFalse(string.IsNullOrEmpty(info.ErrorMessage), "No Error message was returned");

            }
        }


        public void TryLoadFromUrlStream()
        {

        }

        public void TryFailFormUrlStream()
        {

        }

        

    }
}
