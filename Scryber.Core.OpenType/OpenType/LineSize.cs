using System;
namespace Scryber.OpenType
{
    /// <summary>
    /// Defines the size of the characters measured and the space needed to fit them from a string measurer
    /// </summary>
    public struct LineSize
    {
        /// <summary>
        /// The width reqired to render the fitted characters 
        /// </summary>
        public double RequiredWidth { get; private set; }

        /// <summary>
        /// The height required to render the fitted characters
        /// </summary>
        public double RequiredHeight { get; private set; }

        /// <summary>
        /// The number of characters that were able to fit within the available width.
        /// </summary>
        public int CharsFitted { get; private set; }

        /// <summary>
        /// If true then the characters have been split on a word boundary.
        /// </summary>
        public bool OnWordBoudary { get; set; }


        /// <summary>
        /// Creates a new line size value
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="charsfitted"></param>
        /// <param name="isWordbreak"></param>
        public LineSize(double width, double height, int charsfitted, bool isWordbreak)
        {
            this.RequiredWidth = width;
            this.RequiredHeight = height;
            this.CharsFitted = charsfitted;
            this.OnWordBoudary = isWordbreak;
        }


        public override string ToString()
        {
            return $"Size: {this.RequiredWidth} x {this.RequiredHeight}";
        }
    }
}
