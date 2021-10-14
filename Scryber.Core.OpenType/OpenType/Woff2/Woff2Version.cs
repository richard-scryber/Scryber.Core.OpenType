using System;
namespace Scryber.OpenType.Woff2
{
    public class Woff2Version : TypefaceVersionReader
    {
        public Woff2Version(string id, byte[] header)
            : base(header, DataFormat.Woff2)
        {
        }

        

        public override ITypeface ReadTypefaceAfterVersion(BigEndianReader reader, ITypefaceReference forReference)
        {
            throw new NotImplementedException();
        }

        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Woff 2 Format";
        }
    }
}
