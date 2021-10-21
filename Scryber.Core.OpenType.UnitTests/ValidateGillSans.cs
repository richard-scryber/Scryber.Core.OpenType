using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    
    /// <summary>
    /// Validates the GillSans TTC collection file
    /// </summary>
    public static class ValidateGillSans
    {
        public const string FamilyName = "Gill Sans";

        public const WeightClass Weight = WeightClass.Normal;
        public const WidthClass Width = WidthClass.Medium;
        public const FontRestrictions Restrictions = 0;
        public const FontSelection Selections = 0;

        public const string RootUrl = "https://raw.githubusercontent.com/richard-scryber/Scryber.Core.OpenType/main/Scryber.Core.OpenType.UnitTests/";
        public const string UrlPath = "fonts/GillSans.ttc";

        private class FontFace
        {
            public WeightClass Weight;
            public FontSelection Selection;
            public WidthClass Width;
        }

        private static FontFace[] FontTypefaces = new FontFace[]
        {
            new FontFace() { Weight  = WeightClass.Normal, Selection = FontSelection.Regular, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.Normal, Selection = FontSelection.Italic, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.Bold, Selection = FontSelection.Bold, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.Bold, Selection = FontSelection.Italic | FontSelection.Bold, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.SemiBold, Selection = FontSelection.Regular, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.SemiBold, Selection = FontSelection.Italic, Width = WidthClass.Medium },
            new FontFace() { Weight  = (WeightClass)1000, Selection = FontSelection.Regular, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.Light, Selection = FontSelection.Regular, Width = WidthClass.Medium },
            new FontFace() { Weight  = WeightClass.Light, Selection = FontSelection.Italic, Width = WidthClass.Medium }
        };

        public static void AssertInfo(ITypefaceInfo info, string source, int testIndex)
        {
            Assert.IsNotNull(info);
            Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage));

            if (!string.IsNullOrEmpty(source))
            {
                Assert.IsNotNull(info.Source, "Gill Sans info Path was null for test " + testIndex);
                Assert.AreEqual(source, info.Source, "Gill Sans info Path was not equal to " + source + " for test " + testIndex);
            }

            Assert.IsNotNull(info.Fonts, "Gill Sans references was null for " + testIndex);
            Assert.AreEqual(FontTypefaces.Length, info.Fonts.Length, "Gill Sans references was not " + FontTypefaces.Length + " for " + testIndex);
            Assert.AreEqual(DataFormat.TTC, info.SourceFormat, "The source format for the font was not TTC for " + testIndex);

            //First should be normal
            var matches = new List<FontFace>(FontTypefaces);

            foreach (var typeface in info.Fonts)
            {
                Assert.IsNotNull(typeface);
                Assert.AreEqual(FamilyName, typeface.FamilyName, "The font names did not match for the typeface " + typeface);

                bool found = false;

                foreach (var match in matches)
                {
                    if (typeface.FontWeight == match.Weight
                        && typeface.FontWidth == match.Width
                        && typeface.Selections == match.Selection)
                    {
                        matches.Remove(match);
                        found = true;
                        break;
                    }
                }

                Assert.IsTrue(found, "The loaded font " + typeface.ToString() + " was not matched against any expected fonts in the collection");
            }

            Assert.AreEqual(0, matches.Count, "Not all the typefaces were matched.");
        }


        public static void AssertTypefaces(ITypefaceFont[] typefaces)
        {
            var matches = new List<FontFace>(FontTypefaces);

            foreach (var typeface in typefaces)
            {
                Assert.IsNotNull(typeface);
                Assert.AreEqual(FamilyName, typeface.FamilyName, "The font names did not match for the typeface " + typeface);
                Assert.AreEqual(DataFormat.TTF, typeface.SourceFormat);

                bool found = false;

                foreach(var match in matches)
                {
                    if(typeface.FontWeight == match.Weight
                        && typeface.FontWidth == match.Width
                        && typeface.Selections == match.Selection )
                    {
                        matches.Remove(match);
                        found = true;
                        break;
                    }
                }

                Assert.IsTrue(found, "The loaded font " + typeface.ToString() + " was not matched against any expected fonts in the collection");
            }

            Assert.AreEqual(0, matches.Count, "Not all the typefaces were matched.");
                
            
        }
    }
}
