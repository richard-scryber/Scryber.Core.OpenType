/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public enum CharacterPlatforms : ushort
    {
        Unicode = 0,
        Macintosh= 1,
        Other = 2,
        Windows = 3
    }

    public enum WindowsCharacterEncodings : ushort
    {
        Symbol = 0,
        Unicode = 1,
        ShiftJIS = 2,
        PRC = 3,
        Big5 = 4,
        Wansung = 5,
        Johab = 6,
        Reserved1 = 7,
        Reserved2 = 8,
        Reserverd3 = 9,
        UCS4 = 10
    }

    public enum UnicodeCharacterEncodings : ushort
    {
        Default = 0,
        Version_11 = 1,
        ISO10646 = 2,
        Unicode_20 = 3
    }

    /// <summary>
    /// The character encodings based on the 'Mac' character platform
    /// </summary>
    public enum MacCharacterEncodings : ushort
    {
        Roman = 0,
        Japanese = 1,
        Traditional_Chinese = 2,
        Korean = 3,
        Arabic = 4,
        Hebrew = 5,
        Greek = 6,
        Russian = 7,
        RSymbol = 8,
        Devanagari = 9,
        Gurmukhi = 10,
        Gujarati = 11,
        Orija = 12,
        Bengali = 13,
        Tamil = 14,
        Teluga = 15,
        Kannada = 16,
        Malayalam = 17,
        Sinhalese = 18,
        Burmese = 19,
        Khmer = 20,
        Thai = 21,
        Laotian = 22,
        Georgian = 23,
        Armenian = 24,
        SimplifiedChinese = 25,
        Tibetan = 26,
        Mongolian = 27,
        Geez = 28,
        Slavic = 29,
        Vietnamese = 30,
        Sindhi = 31,
        Uninterpreted = 32
    }

    [Flags()]
    public enum FontHeaderFlags : ushort
    {
        BaseLineAtZero = 1,
        LeftPointAtZero = 2,
        InstructionsBasedOnPoint = 4,
        ForcePPemToInteger = 8,
        InstructionsAlterWidth = 16,
        VerticalLayout = 32,
        Reserved1 = 64,
        ArabicFont = 128,
        IsGXWithMetamorphosis = 256,
        ContainsStrongR2LGlyphs = 512,
        ContainsIndicStyle = 1024,
        FontDataIsLossless = 2048,
        FontIsConverted = 4096,
        FontIsOptimizedForClearType = 8192,
        Reserved2 = 16384,
        Reserved3 = 32768
    }

    [Flags()]
    public enum FontStyleFlags : ushort
    {
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Outline = 8,
        Shadow = 16,
        Condensed = 32,
        Extended = 64
    }

    public enum FontDirectionFlags : short
    {
        StrongR2LAndNeutrals = -2,
        StrongR2L = -1,
        Mixed = 0,
        StrongL2R = 1,
        StrongL2RAndNeutrals = 2
    }

    public enum FontIndexLocationFormat : ushort
    {
        ShortOffsets = 0,
        LongOffsets = 1
    }

    public enum GlyphDataFormat : ushort
    {
        Current = 0
    }

    public enum OS2TableVersion : ushort
    {
        TrueType15 = 0,
        TrueType166 = 1,
        OpenType12 = 2
    }

    

    

    public enum UnicodeRangeBit : int
    {
        BasicLatin = 0,
        Latin1Supplement = 1,
        LatinExtendedA = 2,
        LatinExtendedB = 3,
        IPAExtensions = 4,
        SpacingModifierLetters = 5,
        CombinigDiacriticalMarks = 6,
        GreekAndCoptic = 7,
        Cyrillic = 9,
        Armenian = 10,
        Hebrew = 11,
        Arabic = 13,
        Devanagari = 15,
        Bengali = 16,
        Gurmukhi = 17,
        Gujarati = 18,
        Oriya = 19,
        Tamil = 20,
        Telugu = 21,
        Kannada = 22,
        Malayalam = 23,
        Thai = 24,
        Lao = 25,
        Georgian = 26,
        HangulJamo = 28,
        LatinExtendedAdditional = 29,
        GreekExtended = 30,
        GeneralPunctuation = 31,
        SuperAndSubScripts = 32,
        CurrencySymbols = 33,
        CombiningDiacriticalMarkForSymbols = 34,
        LetterlikeSymbols = 35,
        NumberForms = 36,
        Arrows = 37,
        MathematicalOperators = 38,
        MiscTechnical = 39,
        ControlPictures = 40,
        OpticalCharacterRecognition = 41,
        EnclosedAlphaNumerics = 42,
        BoxDrawing = 43,
        BlockComponents = 44,
        GeometricShapes = 45,
        MiscSymbols = 46,
        Dingbats = 47,
        CJKSymbolsAndPunctuation = 48,
        Hiragana = 49,
        Katakana = 50,
        Bopomofo = 51,
        HangulCompatibility = 52,
        EnclosedCJKLettersAndMonths = 54,
        CJKCompatibility = 55,
        HangulSyllables = 56,
        NonPlane = 57,
        CJKUnifiedIdeographs = 59,
        PrivateUseArea = 60,
        CJKCompatibilityIdeographs = 61,
        AlphabeticPresentationForms = 62,
        ArabicPresentationFormsA = 63,
        CombiningHalfMarks = 64,
        CJKCompatibilityForms = 65,
        SmallFormVariants = 66,
        ArabicPresentationFormsB = 67,
        HalfWidthAndFullWidthForms = 68,
        Specials = 69,
        Tibetan = 70,
        Syriac = 71,
        Thaana = 72,
        Sinhala = 73,
        Myanmar = 74,
        Ethiopic = 75,
        Cherokee = 76,
        UnifiedCanadianAboriginalSyllabics = 77,
        Ogham = 78,
        Runic = 79,
        Khmer = 80,
        Mongolian = 81,
        BrailePatterns = 82,
        YiSyllables = 83,
        Tagalog = 84,
        OldItalic = 85,
        Gothic = 86,
        Deseret = 87,
        MusicalSymbols = 88,
        MathematicalAlphanumericSymbols = 89,
        PrivateUsePlane15 = 90,
        VariationSelectors = 91,
        Tags = 92
    }

    

    public enum CodePageBit : int
    {
        Latin1 = 0,
        Latin2EasternEurope = 1,
        Cyrillic = 2,
        Greek = 3,
        Turkish = 4,
        Hebrew = 5,
        Arabic = 6,
        WindowsBaltic = 7,
        Vietnamese = 8,
        Thai = 16,
        Japan = 17,
        ChineseSimplified = 18,
        KoreanWansung = 19,
        ChineseTraditional = 20,
        KoreanJohab = 21,
        MacintoshUSRoman = 29,
        OEMCharacterset = 30,
        SymbolCharacterSet = 31,
        IBMGreek = 48,
        MSDOSRussian = 49,
        MSDOSNordic = 50,
        Arabic2 = 51,
        MSDOSCanadianFrench = 52,
        Hebrew2 = 53,
        MSDOSIcelandic = 54,
        MSDOSPortuguese = 55,
        IBMTurkish = 56,
        IBMCyrillic = 57,
        Latin2 = 58,
        MSDOSBaltic = 59,
        GreekFormer437G = 60,
        ArabicFormerASMO708 = 61,
        WELatin1 = 62,
        US = 63
    }

    public enum KerningCoverage : byte
    {
        Horizontal = 1,
        Minimum = 2,
        CrossStream = 4,
        Override = 8,
        Reserved4 = 16,
        Reserved5 = 32,
        Reserved6 = 64,
        Reserved7 = 128,
        
    }

    public enum KerningFormat : byte
    {
        Format0 = 1,
        Format3 = 4
    }

    public enum NameTypes : ushort
    {
        CopyrightNotice = 0,
        FontFamily = 1,
        FontSubFamily = 2,
        UniqueFontIdentifier = 3,
        FullFontName = 4,
        Version = 5,
        PostscriptName = 6,
        Trademark = 7,
        ManufacturerName = 8,
        Designer = 9,
        Description = 10,
        VendorURL = 11,
        DesignerURL = 12,
        LicenseDescription = 13,
        LicenseURL = 14,
        Reserved0 = 15,
        PreferredFamily = 16,
        PreferredSubFamily = 17,
        CompatibleFull_MacOnly = 18,
        SampleText = 19,
        PostscriptCID = 20

    }
}
