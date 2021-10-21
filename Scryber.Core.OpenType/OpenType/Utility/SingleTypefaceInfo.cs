using System;
namespace Scryber.OpenType.Utility
{

    /// <summary>
    /// Implements the ITypefaceInfo and ITypefaceReference for a sinlge font file with a signle typeface
    /// </summary>
    public class SingleTypefaceInfo : ITypefaceInfo, ITypefaceReference
    {
        private ITypefaceReference[] _thisRef;

        public string Source { get; set; }

        public int TypefaceCount
        {
            get { return 1; }
        }

        public ITypefaceReference[] References { get { return _thisRef; } }

        public DataFormat SourceFormat { get; private set; }

        public string ErrorMessage
        {
            get { return string.Empty; }
        }

        public string FamilyName { get; private set; }

        public WeightClass FontWeight { get; private set; }

        public WidthClass FontWidth { get; private set; }

        public FontRestrictions Restrictions { get; private set; }

        public FontSelection Selections { get; private set; }

        public long OffsetInFile { get; internal set; }

        public SingleTypefaceInfo(string path, DataFormat format, string family, FontRestrictions restrictions, WeightClass weight, WidthClass width, FontSelection selection, long offset)
        {
            this.Source = path;
            this.SourceFormat = format;
            this.FamilyName = family;
            this.Restrictions = restrictions;
            this.FontWeight = weight;
            this.FontWidth = width;
            this.Selections = selection;
            this.OffsetInFile = offset;
            _thisRef = new ITypefaceReference[] { this };
        }

        

        public override string ToString()
        {
            return this.FamilyName + " (weight: " + this.FontWeight.ToString() + ", width: " + this.FontWidth + ", restrictions : " + this.Restrictions + ", selections : " + this.Selections.ToString() + ")";
        }


        internal static bool AreEqual(ITypefaceReference forReference, ITypefaceReference reference)
        {
            return forReference.FamilyName == reference.FamilyName
                && forReference.FontWeight == reference.FontWeight
                && forReference.FontWidth == reference.FontWidth
                && forReference.Restrictions == reference.Restrictions
                && forReference.Selections == reference.Selections;
        }
    }
}
