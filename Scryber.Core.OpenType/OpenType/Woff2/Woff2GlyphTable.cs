using System;
namespace Scryber.OpenType.Woff2
{
    public class Woff2GlyphTable : TTF.TrueTypeFontTable
    {
        public Woff2GlyphTable(long offset, Version vers)
            : base(offset)
        {
            this.TableVersion = vers;
        }

    }
}
