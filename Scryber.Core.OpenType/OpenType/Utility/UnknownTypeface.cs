using System;
namespace Scryber.OpenType.Utility
{
    public sealed class UnknownTypefaceInfo : ITypefaceInfo
    {
        private static readonly IFontInfo[] _EMPTY = new IFontInfo[] { };

        public string Source { get; private set; }

        public int FontCount { get { return 0; } }

        public IFontInfo[] Fonts { get { return _EMPTY; } }

        public DataFormat SourceFormat { get { return DataFormat.Other; } }

        public string ErrorMessage { get; private set; }

        public UnknownTypefaceInfo(string sourcePath, string error)
        {
            this.Source = sourcePath;
            this.ErrorMessage = error;
        }
    }
}
