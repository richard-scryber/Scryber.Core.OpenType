﻿using System;
namespace Scryber.OpenType
{
    [Flags()]
    public enum FontSelection : ushort
    {
        Italic = 1,
        Underscore = 2,
        Negative = 4,
        Outlined = 8,
        Strikeout = 16,
        Bold = 32,
        Regular = 64,
        UseTypographicSizes = 128
    }

    [Flags()]
    public enum FontRestrictions : ushort
    {
        InstallableEmbedding = 0,
        Reserved0 = 1,
        NoEmbedding = 2,
        PreviewPrintEmbedding = 4,
        EditableEmbedding = 8,
        NoSubsetting = 256,
        BitmapEmbedding = 512,
    }

    public enum WeightClass : ushort
    {
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900
    }

    public enum WidthClass : ushort
    {
        UltraCondensed = 1,
        ExtraCondensed = 2,
        Condensed = 3,
        SemiCondensed = 4,
        Medium = 5,
        SemiExpanded = 6,
        Expanded = 7,
        ExtraExpanded = 8,
        UltraExpanded = 9
    }

    public enum DataFormat : ushort
    {
        TTF = 1,
        OTF = 2,
        TTC = 3,
        Woff = 4,
        Woff2 = 5,
        Other = 100
    }

    /// <summary>
    /// Defines the metrics to use for measurement of lines, ascenders and descenders within a true type font.
    /// </summary>
    public enum FontUnitType : int
    {
        /// <summary>
        /// Defers control to the font program, where version 4 of the OS/2 table can
        /// dictate to use typographic metrics in the fsSelection. Otherwise it is the Head metrics
        /// </summary>
        UseFontPreference = 0,
        /// <summary>
        /// Explicitly dictates the use of the metrics from the head table for the Ascender, Descender and LineGap
        /// </summary>
        UseHeadMetrics,
        /// <summary>
        /// Explicitly dictates the use of the metrics from the oS2 typographic values for Ascender, Descnder and LineGap.
        /// </summary>
        UseTypographicMetrics
    }
}
