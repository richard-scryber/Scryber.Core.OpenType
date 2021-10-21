using System;
namespace Scryber.OpenType.Utility
{
    public class CollectionTypefaceInfo : ITypefaceInfo
    {
        public CollectionTypefaceInfo(string path, DataFormat format, IFontInfo[] references)
        {
            this.Fonts = references ?? new IFontInfo[] { };
            this.SourceFormat = format;
            this.Source = path;
        }

        public string Source { get; private set; }

        public int FontCount { get { return this.Fonts.Length; } }

        public IFontInfo[] Fonts { get; private set; }

        public DataFormat SourceFormat { get; private set; }

        public string ErrorMessage { get { return string.Empty; } }
    }
}
