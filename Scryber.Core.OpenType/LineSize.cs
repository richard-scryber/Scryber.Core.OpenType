using System;
namespace Scryber.OpenType
{
    /// <summary>
    /// Defines the size of the characters measured and the space needed to fit them.
    /// </summary>
    public struct LineSize
    {
        public double RequiredWidth { get; private set; }

        public double RequiredHeight { get; private set; }

        public int CharsFitted { get; private set; }

        public bool OnWordBoudary { get; set; }

        public LineSize(double width, double height, int charsfitted, bool isWordbreak)
        {
            this.RequiredWidth = width;
            this.RequiredHeight = height;
            this.CharsFitted = charsfitted;
            this.OnWordBoudary = isWordbreak;
        }
    }
}
