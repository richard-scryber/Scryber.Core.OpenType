using System;
using System.Collections;
using Scryber.OpenType.OTTO;

namespace Scryber.OpenType
{
    /// <summary>
    /// Information about the name, weight, width etc.
    /// of a particular font within a typeface.
    /// </summary>
    public interface IFontInfo
    {
        /// <summary>
        /// Gets the family name for the font
        /// </summary>
        string FamilyName { get; }

        /// <summary>
        /// Gets the weight of the font
        /// </summary>
        WeightClass FontWeight { get; }

        /// <summary>
        /// Gets the expected width of the font
        /// </summary>
        WidthClass FontWidth { get; }

        /// <summary>
        /// Gets any restrictions on the usage of the font within applications.
        /// </summary>
        FontRestrictions Restrictions { get; }

        /// <summary>
        /// Gets the standard ont selections (e.g. Bold, Italic, Striketrhough) of the font.
        /// </summary>
        FontSelection Selections { get; }
    }



    /// <summary>
    /// Information about a specific typeface that has been loaded from another source.
    /// Along with the fonts contained within it.
    /// </summary>
    public interface ITypefaceInfo
    {
        /// <summary>
        /// The source the typeface was loaded from
        /// </summary>
        string Source { get; }

        /// <summary>
        /// The number of fonts contained within the typeface
        /// </summary>
        int FontCount { get; }

        /// <summary>
        /// Information about all the fonts contained within this source
        /// </summary>
        IFontInfo[] Fonts { get; }

        /// <summary>
        /// The original format of the Typeface data.
        /// </summary>
        DataFormat SourceFormat { get; }

        /// <summary>
        /// Any errors identified whilst loading the typeface or its associated fonts
        /// </summary>
        string ErrorMessage { get; }
    }



    /// <summary>
    /// A specific single full font from a typeface that has font metrics
    /// and data, along with the font information.
    /// </summary>
    public interface ITypefaceFont : IFontInfo
    {
        /// <summary>
        /// Returns true if the loaded font can be used.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// The data format native to this font itself (which may not
        /// be the same as the source it was loaded from)
        /// </summary>
        /// <remarks>If the font was loaded from a collection file then this
        /// will refer to the format of the font format within the collection,
        /// rather than the outer collection</remarks>
        DataFormat SourceFormat { get; }

        /// <summary>
        /// Gets the binary font data in the type requested. Note not all formats,
        /// will be supported on all types. Use the CanGetFileData to confirm first.
        /// </summary>
        /// <param name="format">The required format of the data</param>
        /// <returns>A byte array of font data in hte required format</returns>
        byte[] GetFileData(DataFormat format);


        /// <summary>
        /// Returns true if this typeface font supports data retrieval in the requested format
        /// </summary>
        /// <param name="format">The format of the typeface font data needed</param>
        /// <returns></returns>
        bool CanGetFileData(DataFormat format);


        /// <summary>
        /// Returns the standard font metrics for this font along with a measurement option.
        /// </summary>
        /// <returns></returns>
        IFontMetrics GetMetrics();

    }


    /// <summary>
    /// Defines the basic sizes of a font in FontUnits
    /// and a the ability to measure strings
    /// </summary>
    public interface IFontMetrics
    {
        /// <summary>
        /// Gets the number of font units in an uppercase M (the basic
        /// bounding box for a character).
        /// </summary>
        int FUnitsPerEm { get; }

        /// <summary>
        /// Gets the ascender height of this font in FontUnits 
        /// </summary>
        int AscenderHeightFU { get; }

        /// <summary>
        /// Gets the descenter height of this font in FontUnits
        /// </summary>
        int DescenderHeightFU { get; }

        /// <summary>
        /// Gets the standard spacing between a descender and the next
        /// asender in Font Units
        /// </summary>
        int LineSpaceingFU { get; }

        /// <summary>
        /// Gets the width of a lowercase x in Font Units (the basic width of a character).
        /// </summary>
        int ExWidthFU { get; }


        /// <summary>
        /// Returns true if this font should be a vertical font (read from
        /// top to bottom, rather than horizontally).
        /// </summary>
        bool Vertical { get; }

        
        /// <summary>
        /// Measures a set of characters from the start ofset on a single line
        /// upto the maximum width available at the specified point size.
        /// </summary>
        /// <param name="chars">The characters to measure</param>
        /// <param name="startOffset">The starting offset within the characters to start measuring</param>
        /// <param name="emSize">The proportional size of the font required e.g. 12 points = 12.0.</param>
        /// <param name="maxWidth">The maximum allowed width for the characters in the proportional size</param>
        /// <param name="options">The word breaking and spacing options for the measurement</param>
        /// <returns></returns>
        LineSize Measure(string chars, int startOffset, double emSize, double maxWidth, TypeMeasureOptions options);
    }

}
