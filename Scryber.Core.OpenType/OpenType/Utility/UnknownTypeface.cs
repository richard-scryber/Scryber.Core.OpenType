using System;
namespace Scryber.OpenType.Utility
{
    public sealed class UnknownTypefaceInfo : ITypefaceInfo
    {
        private static readonly ITypefaceReference[] _EMPTY = new ITypefaceReference[] { };

        public string Source { get; private set; }

        public int TypefaceCount { get { return 0; } }

        public ITypefaceReference[] References { get { return _EMPTY; } }

        public DataFormat SourceFormat { get { return DataFormat.Other; } }

        public string ErrorMessage { get; private set; }

        public UnknownTypefaceInfo(string sourcePath, string error)
        {
            this.Source = sourcePath;
            this.ErrorMessage = error;
        }
    }
}
