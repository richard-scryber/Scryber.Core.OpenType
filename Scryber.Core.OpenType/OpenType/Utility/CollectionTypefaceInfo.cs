using System;
namespace Scryber.OpenType.Utility
{
    public class CollectionTypefaceInfo : ITypefaceInfo
    {
        public CollectionTypefaceInfo(string path, DataFormat format, ITypefaceReference[] references)
        {
            this.References = references;
            this.SourceFormat = format;
            this.Path = path;
        }

        public string Path { get; private set; }

        public int TypefaceCount { get { return this.References.Length; } }

        public ITypefaceReference[] References { get; private set; }

        public DataFormat SourceFormat { get; private set; }

        public string ErrorMessage { get { return string.Empty; } }
    }
}
