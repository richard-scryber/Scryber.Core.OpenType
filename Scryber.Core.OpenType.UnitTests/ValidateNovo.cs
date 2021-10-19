using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scryber.OpenType.UnitTests
{
    /// <summary>
    /// Validates the contents of Noto otf file
    /// </summary>
    public static class ValidateNoto
    {
        public const string FamilyName = "Noto Sans TC";
        public const WeightClass Weight = WeightClass.Normal;
        public const WidthClass Width = WidthClass.Medium;
        public const FontRestrictions Restrictions = FontRestrictions.InstallableEmbedding;
        public const FontSelection Selections = FontSelection.Regular;

        public const string RootUrl = "https://raw.githubusercontent.com/richard-scryber/Scryber.Core.OpenType/main/Scryber.Core.OpenType.UnitTests/";
        public const string UrlPath = "fonts/NotoTC.otf";


        public static void AssertInfo(ITypefaceInfo info, string source, int testIndex)
        {
            Assert.IsNotNull(info);
            Assert.IsTrue(string.IsNullOrEmpty(info.ErrorMessage));

            if (!string.IsNullOrEmpty(source))
            {
                Assert.IsNotNull(info.Path, "Noto info Path was null for test " + testIndex);
                Assert.AreEqual(source, info.Path, "Noto info Path was not equal to " + source + " for test " + testIndex);
            }

            Assert.IsNotNull(info.References, "Noto references was null for " + testIndex);
            Assert.AreEqual(1, info.References.Length, "Noto references was not 1 for " + testIndex);

            var fref = info.References[0];

            Assert.IsNotNull(fref, "Font reference[0] was null for test " + testIndex);
            Assert.AreEqual(FamilyName, fref.FamilyName, "The font names did not match for test " + testIndex);
            Assert.AreEqual(Weight, fref.FontWeight, "The font weights did not match for test " + testIndex);
            Assert.AreEqual(Width, fref.FontWidth, "The font widths did not match for test " + testIndex);
            Assert.AreEqual(Restrictions, fref.Restrictions, "The font restrictions did not match for test " + testIndex);
            Assert.AreEqual(Selections, fref.Selections, "The font selctions did not match for test " + testIndex);
        }

        public static void AssertTypeface(ITypeface typeface)
        {
            Assert.IsNotNull(typeface);
            Assert.AreEqual(FamilyName, typeface.FamilyName, "The font names did not match for the typeface " + typeface);
            Assert.AreEqual(Weight, typeface.FontWeight, "The font weights did not match for test " + typeface);
            Assert.AreEqual(Width, typeface.FontWidth, "The font widths did not match for test " + typeface);
            Assert.AreEqual(Restrictions, typeface.Restrictions, "The font restrictions did not match for test " + typeface);
            Assert.AreEqual(Selections, typeface.Selections, "The font selctions did not match for test " + typeface);
            Assert.AreEqual(DataFormat.TTF, typeface.SourceFormat);
        }
    }

    
}
