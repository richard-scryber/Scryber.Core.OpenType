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

        public TypeMeasureOptions()
        {
        }
    }
}
