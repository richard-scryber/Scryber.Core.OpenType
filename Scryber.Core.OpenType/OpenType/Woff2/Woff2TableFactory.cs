using System;
using Scryber.OpenType.TTF;

namespace Scryber.OpenType.Woff2
{
    public class Woff2TableFactory : TTF.TrueTypeTableFactory
    {
        public Woff2TableFactory()
            : base(false, new string[] { })
        {
        }


        protected override TrueTypeFontTable ReadTable(string tag, uint length, TrueTypeTableEntryList list, BigEndianReader reader)
        {
            if (tag == TrueTypeTableNames.GlyphData)
                throw new NotImplementedException("Not done the transformation yet");
            else if (tag == TrueTypeTableNames.LocationIndex)
                throw new NotImplementedException("Not done the transformation yet");
            else
                return base.ReadTable(tag, length, list, reader);
        }
    }
}
