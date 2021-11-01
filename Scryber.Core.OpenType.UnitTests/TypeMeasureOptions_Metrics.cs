using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    [TestClass]
    public class TypeMeasureOptions_Metrics
    {

        private const string TextToMeasure = "This is the text to measure";
        private static FileInfo path = new FileInfo(Path.Combine(System.Environment.CurrentDirectory, ValidateHelvetica.UrlPath));
        private static double fontSize = 12.0;
        private static double availableWidth = 1000; //fit all characters
        private static double smallWidth = 90.0;
       

        [TestMethod("1. Default options")]
        public void TypeMeasureOptions_Default()
        {

            var options = TypeMeasureOptions.Default;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsFalse(options.BreakOnWordBoundaries);
            Assert.IsFalse(options.IgnoreStartingWhiteSpace);
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using(var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size

                // "This is the text t"
                // "o measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(18, size.CharsFitted, "Can only fit 18 chars on smaller width");
                Assert.IsFalse(size.OnWordBoudary, "Breaking on word boundary should be false");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");

            }
        }

        [TestMethod("2. Break on words options")]
        public void TypeMeasureOptions_BreakOnWords()
        {

            var options = TypeMeasureOptions.BreakOnWords;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsTrue(options.BreakOnWordBoundaries, "The static break on words should have this flag as true");
            Assert.IsFalse(options.IgnoreStartingWhiteSpace);
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary, as eveything fitted");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size

                // "This is the text"
                // " to measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(16, size.CharsFitted, "Can only fit 16 chars on smaller width with word break");
                Assert.IsTrue(size.OnWordBoudary, "Breaking on word boundary should be true");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");

                //Check the remaining text
                size = metrics.MeasureLine(TextToMeasure, size.CharsFitted, fontSize, smallWidth, options);

                Assert.AreEqual(16, size.FirstCharacter, "The first character should be the space at index 16");
                Assert.AreEqual(TextToMeasure.Length - size.FirstCharacter, size.CharsFitted, "Should have been able to fit the remaining text");

            }
        }

        [TestMethod("3. Ignoring the white space at the start")]
        public void TypeMeasureOptions_IgnoreStartingWhitespace()
        {

            var options = TypeMeasureOptions.BreakOnWords;
            options.IgnoreStartingWhiteSpace = true;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsTrue(options.BreakOnWordBoundaries, "The static break on words should have this flag as true");
            Assert.IsTrue(options.IgnoreStartingWhiteSpace, "The starting white space should be ignored");
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary, as eveything fitted");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size

                // "This is the text"
                // "to measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(16, size.CharsFitted, "Can only fit 16 chars on smaller width with word break");
                Assert.IsTrue(size.OnWordBoudary, "Breaking on word boundary should be true");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");

                //Check the remaining text
                size = metrics.MeasureLine(TextToMeasure, size.CharsFitted, fontSize, smallWidth, options);

                Assert.AreEqual(17, size.FirstCharacter, "The space should be ignored and the first character should be the 't' at index 17");
                Assert.AreEqual(TextToMeasure.Length - size.FirstCharacter, size.CharsFitted, "Should have been able to fit the remaining text");

            }
        }

        [TestMethod("4. Using Header font measures")]
        public void TypeMeasureOptions_FontUnitHeader()
        {

            var options = TypeMeasureOptions.BreakOnWords;
            options.IgnoreStartingWhiteSpace = true;
            options.FontUnits = FontUnitType.UseHeadMetrics;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsTrue(options.BreakOnWordBoundaries, "The static break on words should have this flag as true");
            Assert.IsTrue(options.IgnoreStartingWhiteSpace, "The starting white space should be ignored");
            Assert.AreEqual(FontUnitType.UseHeadMetrics, options.FontUnits, "The font units should be the header metrics");


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary, as eveything fitted");
                Assert.AreEqual(12.0, size.RequiredHeight, "Should use the font header size, rather than typographic.");

            }
        }

        [TestMethod("5. Using OS/2 Typographic font measures")]
        public void TypeMeasureOptions_FontUnitTypo()
        {

            var options = TypeMeasureOptions.BreakOnWords;
            options.IgnoreStartingWhiteSpace = true;
            options.FontUnits = FontUnitType.UseTypographicMetrics;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsTrue(options.BreakOnWordBoundaries, "The static break on words should have this flag as true");
            Assert.IsTrue(options.IgnoreStartingWhiteSpace, "The starting white space should be ignored");
            Assert.AreEqual(FontUnitType.UseTypographicMetrics, options.FontUnits, "The font units should be the header metrics");


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary, as eveything fitted");
                Assert.AreEqual(11.4, Math.Round(size.RequiredHeight,1), "Should use the typographic size, rather than header.");

            }
        }


        [TestMethod("6. Add Word Spacing")]
        public void TypeMeasureOptions_WordSpacing()
        {

            var options = TypeMeasureOptions.Default;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsFalse(options.BreakOnWordBoundaries);
            Assert.IsFalse(options.IgnoreStartingWhiteSpace);
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                //Without spacing should be 140.53 wide
                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53, Math.Round(size.RequiredWidth, 2), "The width of the string was not as statically caclulated");

                options.WordSpacing = 5.0;
                var extra = options.WordSpacing * 5.0; // five spaces in the text to measure

                
                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53 + extra, Math.Round(size.RequiredWidth, 2), "The width of the string did not match with the expected spacing");


                //Check the smaller size with the extra spacing

                // "This is the tex"
                // "t to measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(15, size.CharsFitted, "Can only fit 15 chars on smaller width with extra word space");
                Assert.IsFalse(size.OnWordBoudary, "Breaking on word boundary should be false");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size with the extra spacing and break on words

                // "This is the"
                // " text to measure"

                options.BreakOnWordBoundaries = true;

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(11, size.CharsFitted, "Can only fit 11 chars on smaller width with extra word space and breaking on words");
                Assert.IsTrue(size.OnWordBoudary, "Breaking on word boundary should be true");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");



            }
        }


        [TestMethod("7. Add Character Spacing")]
        public void TypeMeasureOptions_CharacterSpacing()
        {

            var options = TypeMeasureOptions.Default;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsFalse(options.BreakOnWordBoundaries);
            Assert.IsFalse(options.IgnoreStartingWhiteSpace);
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                //Without spacing should be 140.53 wide
                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53, Math.Round(size.RequiredWidth, 2), "The width of the string was not as statically caclulated");

                options.CharacterSpacing = 2.0;
                var extra = options.CharacterSpacing * 27.0; // 27 character spaces in the text to measure


                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53 + extra, Math.Round(size.RequiredWidth, 2), "The width of the string did not match with the expected spacing");


                //Check the smaller size with the extra spacing

                // "This is the tex"
                // "t to measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(13, size.CharsFitted, "Can only fit 13 chars on smaller width with extra character space");
                Assert.IsFalse(size.OnWordBoudary, "Breaking on word boundary should be false");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size with the extra spacing and break on words

                // "This is the"
                // " text to measure"

                options.BreakOnWordBoundaries = true;

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(11, size.CharsFitted, "Can only fit 11 chars on smaller width with extra character space and breaking on words");
                Assert.IsTrue(size.OnWordBoudary, "Breaking on word boundary should be true");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");



            }
        }


        [TestMethod("8. Add Character and Word Spacing")]
        public void TypeMeasureOptions_CharacterAndWordSpacing()
        {

            var options = TypeMeasureOptions.Default;

            Assert.IsFalse(options.CharacterSpacing.HasValue);
            Assert.IsFalse(options.WordSpacing.HasValue);

            Assert.IsFalse(options.BreakOnWordBoundaries);
            Assert.IsFalse(options.IgnoreStartingWhiteSpace);
            Assert.AreEqual(FontUnitType.UseFontPreference, options.FontUnits);


            using (var reader = new TypefaceReader())
            {
                ITypefaceFont font = reader.GetFirstFont(path);

                Assert.IsNotNull(font);
                Assert.IsTrue(font.FamilyName == ValidateHelvetica.FamilyName);

                var metrics = font.GetMetrics(options);
                Assert.IsNotNull(metrics);

                //Without spacing should be 140.53 wide
                var size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53, Math.Round(size.RequiredWidth, 2), "The width of the string was not as statically caclulated");

                options.CharacterSpacing = 2.0;
                options.WordSpacing = 5.0;
                var extra = (options.CharacterSpacing * 22.0) + (options.WordSpacing * 5); // 22 character spaces + 5 word spaces in the text to measure


                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, availableWidth, options);

                Assert.AreEqual(TextToMeasure.Length, size.CharsFitted, "Should have fitted all the characters");
                Assert.IsFalse(size.OnWordBoudary, "Should not be breaking on a word boundary");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");
                Assert.AreEqual(140.53 + extra, Math.Round(size.RequiredWidth, 2), "The width of the string did not match with the expected word and character spacing");


                //Check the smaller size with the extra spacing

                // "This is the tex"
                // "t to measure"

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(11, size.CharsFitted, "Can only fit 11 chars on smaller width with extra character and word space");
                Assert.IsFalse(size.OnWordBoudary, "Breaking on word boundary should be false");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");


                //Check the smaller size with the extra spacing and break on words

                // "This is the"
                // " text to measure"

                options.BreakOnWordBoundaries = true;

                size = metrics.MeasureLine(TextToMeasure, 0, fontSize, smallWidth, options);

                Assert.AreEqual(11, size.CharsFitted, "Can only fit 11 chars on smaller width with extra character space and breaking on words");
                Assert.IsTrue(size.OnWordBoudary, "Breaking on word boundary should be true");
                Assert.AreEqual(12.0, size.RequiredHeight, "Helvetica font preferences should use the font header size, rather than typographic.");



            }
        }
    }
}
