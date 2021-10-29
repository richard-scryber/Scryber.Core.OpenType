using System;
namespace Scryber.OpenType
{
    /// <summary>
    /// Defines the options for measuring string sizes
    /// </summary>
    public class TypeMeasureOptions
    {
        /// <summary>
        /// The optional extra spacing between the characters in the proportional size
        /// </summary>
        public double? CharacterSpacing { get; set; }

        /// <summary>
        /// The optional extra spacing between the words in the proportional size
        /// </summary>
        public double? WordSpacing { get; set; }

        /// <summary>
        /// If true then the closest word will be returned as the required size, rather than all the maximum characters.
        /// </summary>
        public bool BreakOnWordBoundaries { get; set; }

        /// <summary>
        /// Set whether the typographic sizes should be used rather than the Header sizes for line heights, or allow the font to dictate which is best.
        /// </summary>
        public FontUnitType FontUnits { get; set; }

        /// <summary>
        /// If true then any whitespace at the start of the string to measure will be ignored and the returning
        /// FirstCharacter in the line size will be the size of the string after any prepended white space.
        /// </summary>
        public bool IgnoreStartingWhiteSpace { get; set; }


        public TypeMeasureOptions()
        {
            FontUnits = FontUnitType.UseFontPreference;
            BreakOnWordBoundaries = false;
            IgnoreStartingWhiteSpace = false;
        }


        /// <summary>
        /// Returns the default options for measuing text (no spacing and break anywhere)
        /// </summary>
        public static TypeMeasureOptions Default
        {
            get
            {
                return new TypeMeasureOptions() { WordSpacing = null, CharacterSpacing = null, BreakOnWordBoundaries = false };
            }
        }

        /// <summary>
        /// Returns the default options for measuring text but foring breaks on words.
        /// </summary>
        public static TypeMeasureOptions BreakOnWords
        {
            get
            {
                return new TypeMeasureOptions() { WordSpacing = null, CharacterSpacing = null, BreakOnWordBoundaries = true };
            }
        }
    }
}
