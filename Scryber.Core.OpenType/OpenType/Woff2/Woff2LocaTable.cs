using System;
namespace Scryber.OpenType.Woff2
{
    public class Woff2LocaTable : TTF.TrueTypeFontTable
    {
        public Woff2LocaTable(long offset, Version vers)
            : base(offset)
        {
            this.TableVersion = vers;
        }
    }
}
