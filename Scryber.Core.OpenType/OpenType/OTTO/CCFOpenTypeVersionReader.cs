using System;
namespace Scryber.OpenType.OTTO
{
    public class CCFOpenTypeVersionReader : TTF.TrueTypeVersionReader
    {
        

        public CCFOpenTypeVersionReader(string header, byte[] data)
            : base(header, data, DataFormat.TTF)
        {
        }

        public override string ToString()
        {
            return "CCF OpenType " + this.VersionIdentifier;
        }

        public override ITypefaceInfo ReadTypefaceInfoAfterVersion(BigEndianReader reader, string source)
        {
            return base.ReadTypefaceInfoAfterVersion(reader, source);
        }

        internal override TTF.TrueTypeTableFactory GetTableFactory()
        {
            return new OpenTypeTableFactory(false);
        }

    }
}
