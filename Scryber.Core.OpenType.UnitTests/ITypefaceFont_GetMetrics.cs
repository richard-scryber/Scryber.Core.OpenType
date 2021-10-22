using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass]
    public class ITypefaceFont_GetMetrics
    {
        private const string TextToMeasure = "This is the text to measure";

        [TestMethod("1. Get Metrics for Helvetica")]
        public void GetHelveticaMetrics()
        {

            var path = new FileInfo(Path.Combine(System.Environment.CurrentDirectory, ValidateHelvetica.UrlPath));
            var words = TextToMeasure;
            var offset = 0;
            var fontSize = 12.0;
            var available = 1000; //fit all characters
            var options = TypeMeasureOptions.Default;

            IFontMetrics metrics;

            using (var reader = new TypefaceReader())
            {
                var font = reader.GetFirstFont(path);

                metrics = font.GetMetrics(options);


                ValidateHelvetica.AssertMetrics(metrics);
                var size = metrics.MeasureLine(words, offset, fontSize, available, options);


                Assert.AreEqual(12.0, fontSize, "The measurements are for a point size of 12");
                Assert.AreEqual(words.Length, size.CharsFitted, "Should be able to fit everything in the first measurement");

                //checked to fit all at 140.53125
                Assert.AreEqual(140.53, Math.Round(size.RequiredWidth, 2), "The width of the string was not as statically caclulated");

                //checked height of a line to 12
                Assert.AreEqual(12.0, size.RequiredHeight, "The height of the string was not as statically calculated");

                // check 2
                //reduce the size so not all characters can fit.
                //expected 90.50 and fitted 19
                available = 90;
                size = metrics.MeasureLine(words, offset, fontSize, available, options);

                //This is the text t
                Assert.AreEqual(18, size.CharsFitted);
                Assert.AreEqual(83.84, Math.Round(size.RequiredWidth, 2), "The width of the restricted string was not as statically calculated");
                Assert.AreEqual(12.0, size.RequiredHeight, "The height of the string was not as statically calculated");


                // check 3
                //now set breaking on words only
                options.BreakOnWordBoundaries = true;

                size = metrics.MeasureLine(words, offset, fontSize, available, options);
                //This is the text
                Assert.AreEqual(16, size.CharsFitted);
                Assert.AreEqual(77.17, Math.Round(size.RequiredWidth,2), "The width of the restricted string was not as statically calculated");
                Assert.AreEqual(12.0, size.RequiredHeight, "The height of the string was not as statically calculated");

            }
        }
        
    }
}
